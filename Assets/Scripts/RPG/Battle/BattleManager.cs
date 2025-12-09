using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using XLua;

/// <summary>
/// 战斗管理器 - 管理即时战斗流程
/// 使用框架的事件系统和对象池
/// </summary>
[XLua.LuaCallCSharp]
public class BattleManager : MonoBehaviour
{
    private static BattleManager m_instance;
    
    public static BattleManager Instance
    {
        get
        {
            if (m_instance == null)
            {
                GameObject obj = new GameObject("BattleManager");
                DontDestroyOnLoad(obj);
                m_instance = obj.AddComponent<BattleManager>();
            }
            return m_instance;
        }
    }
    
    /// <summary>
    /// 当前战斗状态
    /// </summary>
    public enum BattleState
    {
        None,
        Preparing,
        InBattle,
        Victory,
        Defeat
    }
    
    public BattleState CurrentState { get; private set; } = BattleState.None;
    
    /// <summary>
    /// 玩家角色
    /// </summary>
    public CharacterData Player { get; private set; }
    
    /// <summary>
    /// 敌人列表
    /// </summary>
    private List<CharacterData> enemies = new List<CharacterData>();
    
    /// <summary>
    /// 玩家攻击冷却时间
    /// </summary>
    private float playerAttackCooldown = 0f;
    
    /// <summary>
    /// 玩家攻击间隔（秒）
    /// </summary>
    private float playerAttackInterval = 1.0f;
    
    /// <summary>
    /// 敌人攻击冷却时间字典
    /// </summary>
    private Dictionary<int, float> enemyAttackCooldowns = new Dictionary<int, float>();
    
    /// <summary>
    /// 敌人攻击间隔（秒）
    /// </summary>
    private float enemyAttackInterval = 1.5f;
    
    /// <summary>
    /// 战斗更新频率（秒）
    /// </summary>
    private float battleUpdateInterval = 0.1f;
    
    void Awake()
    {
        if (m_instance == null)
        {
            m_instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else if (m_instance != this)
        {
            Destroy(gameObject);
        }
    }
    
    void Update()
    {
        // 即时战斗更新
        if (CurrentState == BattleState.InBattle)
        {
            UpdateBattle(Time.deltaTime);
        }
    }
    
    /// <summary>
    /// 开始战斗
    /// </summary>
    public void StartBattle(CharacterData player, List<CharacterData> enemies)
    {
        if (CurrentState != BattleState.None)
        {
            Debug.LogWarning("[BattleManager] 战斗已在进行中");
            return;
        }
        
        this.Player = player;
        this.enemies = new List<CharacterData>(enemies);
        CurrentState = BattleState.Preparing;
        
        // 初始化冷却时间
        playerAttackCooldown = 0f;
        enemyAttackCooldowns.Clear();
        foreach (var enemy in this.enemies)
        {
            enemyAttackCooldowns[enemy.CharacterId] = Random.Range(0f, enemyAttackInterval);
        }
        
        CurrentState = BattleState.InBattle;
        
        // 触发战斗开始事件
        EventManager.Instance?.TriggerEvent("BATTLE_START", null);
        
        Debug.Log("[BattleManager] 即时战斗开始");
    }
    
    /// <summary>
    /// 即时战斗更新
    /// </summary>
    private void UpdateBattle(float deltaTime)
    {
        // 更新玩家攻击冷却
        if (playerAttackCooldown > 0)
        {
            playerAttackCooldown -= deltaTime;
        }
        
        // 更新敌人攻击冷却
        List<int> enemyIdsToRemove = new List<int>();
        foreach (var kvp in enemyAttackCooldowns)
        {
            int enemyId = kvp.Key;
            float cooldown = kvp.Value;
            
            CharacterData enemy = enemies.Find(e => e.CharacterId == enemyId);
            if (enemy == null || enemy.HP <= 0)
            {
                enemyIdsToRemove.Add(enemyId);
                continue;
            }
            
            cooldown -= deltaTime;
            enemyAttackCooldowns[enemyId] = cooldown;
            
            // 敌人自动攻击
            if (cooldown <= 0)
            {
                EnemyAttack(enemy);
                enemyAttackCooldowns[enemyId] = enemyAttackInterval;
            }
        }
        
        // 移除已死亡的敌人
        foreach (var enemyId in enemyIdsToRemove)
        {
            enemyAttackCooldowns.Remove(enemyId);
        }
        
        // 检查战斗是否结束
        if (CheckBattleEnd())
        {
            OnBattleEnd();
        }
    }
    
    /// <summary>
    /// 玩家攻击（由玩家输入触发）
    /// </summary>
    public void PlayerAttack()
    {
        if (CurrentState != BattleState.InBattle)
        {
            return;
        }
        
        if (playerAttackCooldown > 0)
        {
            Debug.Log($"[BattleManager] 攻击冷却中，剩余时间: {playerAttackCooldown:F2}秒");
            return;
        }
        
        if (Player == null || Player.HP <= 0)
        {
            return;
        }
        
        // 找到最近的敌人
        CharacterData target = FindNearestEnemy();
        if (target == null)
        {
            Debug.Log("[BattleManager] 没有可攻击的敌人");
            return;
        }
        
        // 计算并应用伤害
        int damage = CalculateDamage(Player, target);
        ApplyDamage(target, damage);
        
        // 重置攻击冷却
        playerAttackCooldown = playerAttackInterval;
        
        Debug.Log($"[BattleManager] 玩家攻击敌人，造成 {damage} 点伤害");
        
        // 触发事件
        EventManager.Instance?.TriggerEvent("PLAYER_ATTACK", damage);
    }
    
    /// <summary>
    /// 玩家释放技能（由玩家输入触发）
    /// </summary>
    public void PlayerCastSkill(int skillId, CharacterData target = null)
    {
        if (CurrentState != BattleState.InBattle)
        {
            return;
        }
        
        if (playerAttackCooldown > 0)
        {
            Debug.Log($"[BattleManager] 技能冷却中，剩余时间: {playerAttackCooldown:F2}秒");
            return;
        }
        
        if (Player == null || Player.HP <= 0)
        {
            return;
        }
        
        // TODO: 实现技能系统
        // 这里简化处理，使用普通攻击
        PlayerAttack();
    }
    
    /// <summary>
    /// 敌人攻击
    /// </summary>
    private void EnemyAttack(CharacterData enemy)
    {
        if (Player == null || Player.HP <= 0)
        {
            return;
        }
        
        // 计算并应用伤害
        int damage = CalculateDamage(enemy, Player);
        ApplyDamage(Player, damage);
        
        Debug.Log($"[BattleManager] 敌人攻击玩家，造成 {damage} 点伤害");
        
        // 触发玩家血量变化事件
        EventManager.Instance?.TriggerEvent(GlobalEventNames.PLAYER_HP_CHANGED, Player.HP);
    }
    
    /// <summary>
    /// 找到最近的敌人
    /// </summary>
    private CharacterData FindNearestEnemy()
    {
        CharacterData nearest = null;
        foreach (var enemy in enemies)
        {
            if (enemy.HP > 0)
            {
                if (nearest == null)
                {
                    nearest = enemy;
                }
                // TODO: 如果有位置信息，可以计算距离
            }
        }
        return nearest;
    }
    
    /// <summary>
    /// 获取玩家攻击冷却进度（0-1）
    /// </summary>
    public float GetPlayerAttackCooldownProgress()
    {
        if (playerAttackInterval <= 0) return 1f;
        return Mathf.Clamp01(1f - (playerAttackCooldown / playerAttackInterval));
    }
    
    /// <summary>
    /// 获取玩家是否可以攻击
    /// </summary>
    public bool CanPlayerAttack()
    {
        return CurrentState == BattleState.InBattle && 
               playerAttackCooldown <= 0 && 
               Player != null && 
               Player.HP > 0;
    }
    
    /// <summary>
    /// 计算伤害
    /// </summary>
    private int CalculateDamage(CharacterData attacker, CharacterData defender)
    {
        // 简化伤害计算
        int baseDamage = attacker.Strength;
        int defense = defender.Strength / 2;
        int damage = Mathf.Max(1, baseDamage - defense);
        
        // TODO: 添加暴击、技能等计算
        
        return damage;
    }
    
    /// <summary>
    /// 应用伤害
    /// </summary>
    private void ApplyDamage(CharacterData target, int damage)
    {
        target.HP = Mathf.Max(0, target.HP - damage);
        
        // 触发血量变化事件
        if (target == Player)
        {
            EventManager.Instance?.TriggerEvent(GlobalEventNames.PLAYER_HP_CHANGED, target.HP);
        }
    }
    
    /// <summary>
    /// 检查战斗是否结束
    /// </summary>
    private bool CheckBattleEnd()
    {
        // 玩家死亡
        if (Player.HP <= 0)
        {
            CurrentState = BattleState.Defeat;
            return true;
        }
        
        // 所有敌人死亡
        bool allEnemiesDead = true;
        foreach (var enemy in enemies)
        {
            if (enemy.HP > 0)
            {
                allEnemiesDead = false;
                break;
            }
        }
        
        if (allEnemiesDead)
        {
            CurrentState = BattleState.Victory;
            return true;
        }
        
        return false;
    }
    
    /// <summary>
    /// 战斗结束处理
    /// </summary>
    private void OnBattleEnd()
    {
        if (CurrentState == BattleState.Victory)
        {
            // 战斗胜利
            int expReward = CalculateExpReward();
            int goldReward = CalculateGoldReward();
            
            CharacterManager.Instance.AddExp(Player, expReward);
            Player.Gold += goldReward;
            
            Debug.Log($"[BattleManager] 战斗胜利！获得经验: {expReward}, 金币: {goldReward}");
            
            EventManager.Instance?.TriggerEvent("BATTLE_VICTORY", null);
        }
        else if (CurrentState == BattleState.Defeat)
        {
            // 战斗失败
            Debug.Log("[BattleManager] 战斗失败");
            
            EventManager.Instance?.TriggerEvent("BATTLE_DEFEAT", null);
        }
        
        // 清理
        enemies.Clear();
        enemyAttackCooldowns.Clear();
        playerAttackCooldown = 0f;
        CurrentState = BattleState.None;
    }
    
    /// <summary>
    /// 停止战斗
    /// </summary>
    public void StopBattle()
    {
        if (CurrentState == BattleState.InBattle)
        {
            CurrentState = BattleState.None;
            enemies.Clear();
            enemyAttackCooldowns.Clear();
            playerAttackCooldown = 0f;
            
            Debug.Log("[BattleManager] 战斗已停止");
        }
    }
    
    /// <summary>
    /// 计算经验奖励
    /// </summary>
    private int CalculateExpReward()
    {
        int totalExp = 0;
        foreach (var enemy in enemies)
        {
            totalExp += enemy.Level * 10;
        }
        return totalExp;
    }
    
    /// <summary>
    /// 计算金币奖励
    /// </summary>
    private int CalculateGoldReward()
    {
        int totalGold = 0;
        foreach (var enemy in enemies)
        {
            totalGold += enemy.Level * 5;
        }
        return totalGold;
    }
}

