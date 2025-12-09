using UnityEngine;
using System.Collections.Generic;
using XLua;

/// <summary>
/// 技能系统 - 管理技能释放、冷却等
/// 适配到框架的即时战斗系统
/// </summary>
[XLua.LuaCallCSharp]
public class SkillSystem : MonoBehaviour
{
    private PlayerAnimationController animationController;
    private PlayerController playerController;

    private bool isCasting = false;
    private float castTimer = 0f;
    private float castDuration = 0f;
    private SkillData currentCastingSkill;

    private Dictionary<string, float> cooldownTimers = new Dictionary<string, float>();

    [Header("技能表")]
    public List<SkillData> skillList = new List<SkillData>();

    private void Awake()
    {
        animationController = GetComponent<PlayerAnimationController>();
        if (animationController == null)
        {
            animationController = gameObject.AddComponent<PlayerAnimationController>();
        }
        
        playerController = GetComponent<PlayerController>();
        
        // 初始化冷却时间
        foreach (var skill in skillList)
        {
            if (skill != null && !cooldownTimers.ContainsKey(skill.SkillName))
            {
                cooldownTimers.Add(skill.SkillName, 0f);
            }
        }
    }

    private void Update()
    {
        // 冷却计时
        List<string> keys = new List<string>(cooldownTimers.Keys);
        foreach (var key in keys)
        {
            if (cooldownTimers[key] > 0f)
                cooldownTimers[key] -= Time.deltaTime;
        }

        // 施法计时
        if (isCasting)
        {
            castTimer += Time.deltaTime;
            if (castTimer >= castDuration)
            {
                CompleteSkillCast();
            }
        }
    }

    /// <summary>
    /// 尝试释放技能
    /// </summary>
    public bool TryCastSkill(string skillName)
    {
        if (isCasting) return false; // 正在施法中
        if (!cooldownTimers.ContainsKey(skillName)) return false;
        if (cooldownTimers[skillName] > 0f) return false; // 冷却中

        var skill = GetSkillData(skillName);
        if (skill == null)
        {
            Debug.LogWarning($"[SkillSystem] 技能 {skillName} 没找到！");
            return false;
        }

        // 播放动画
        if (animationController != null && skill.Animation != null)
        {
            animationController.PlayAnimation(skill.Animation.AnimationName);
        }

        currentCastingSkill = skill;
        castDuration = skill.CastTime;
        castTimer = 0f;
        isCasting = true;

        Debug.Log($"[SkillSystem] 开始施法：{skill.SkillName}，吟唱时间：{skill.CastTime}秒");
        
        // 触发事件
        EventManager.Instance?.TriggerEvent("SKILL_CAST_START", skillName);
        
        return true;
    }

    /// <summary>
    /// 完成技能释放
    /// </summary>
    private void CompleteSkillCast()
    {
        if (currentCastingSkill == null) return;
        
        Debug.Log($"[SkillSystem] 技能释放成功：{currentCastingSkill.SkillName}");

        SpawnSkillEffect(currentCastingSkill);

        cooldownTimers[currentCastingSkill.SkillName] = currentCastingSkill.CooldownTime;
        
        // 触发事件
        EventManager.Instance?.TriggerEvent("SKILL_CAST_COMPLETE", currentCastingSkill.SkillName);
        
        // 如果是战斗中的技能，触发战斗攻击
        if (BattleManager.Instance != null && BattleManager.Instance.CurrentState == BattleManager.BattleState.InBattle)
        {
            BattleManager.Instance.PlayerAttack();
        }
        
        ResetCastingState();
    }

    /// <summary>
    /// 生成技能特效（使用对象池）
    /// </summary>
    private void SpawnSkillEffect(SkillData skill)
    {
        if (skill.EffectPrefab == null)
            return;

        Vector3 spawnPos = transform.position + transform.rotation * skill.EffectOffset;
        Quaternion spawnRot = transform.rotation;

        // 使用对象池管理特效
        GameObject effectObj = ObjectPool.Instance.Get("SkillEffect", skill.EffectPrefab);
        if (effectObj != null)
        {
            effectObj.transform.position = spawnPos;
            effectObj.transform.rotation = spawnRot;
            
            // 5秒后自动回收
            ObjectPool.Instance.AutoRelease("SkillEffect", effectObj, 5f);
        }
        else
        {
            // 如果对象池没有，直接实例化
            GameObject effect = Instantiate(skill.EffectPrefab, spawnPos, spawnRot);
            Destroy(effect, 5f);
        }
    }
    
    /// <summary>
    /// 中断技能
    /// </summary>
    public void InterruptSkill()
    {
        if (isCasting && currentCastingSkill != null && currentCastingSkill.CanBeInterrupted)
        {
            Debug.Log($"[SkillSystem] 施法中断：{currentCastingSkill.SkillName}");
            ResetCastingState();
        }
    }
    
    /// <summary>
    /// 处理技能命中
    /// </summary>
    public void HandleSkillHit(GameObject target, SkillData skill)
    {
        if (skill.HitEffectPrefab != null)
        {
            Vector3 hitPos = target.transform.position + target.transform.rotation * skill.HitEffectOffset;
            Quaternion hitRot = Quaternion.LookRotation(transform.forward);
            
            // 使用对象池
            GameObject hitEffectObj = ObjectPool.Instance.Get("HitEffect", skill.HitEffectPrefab);
            if (hitEffectObj != null)
            {
                hitEffectObj.transform.position = hitPos;
                hitEffectObj.transform.rotation = hitRot;
                ObjectPool.Instance.AutoRelease("HitEffect", hitEffectObj, 5f);
            }
        }

        if (skill.HasChainEffect)
        {
            TriggerChainEffect(target, skill);
        }
    }
    
    /// <summary>
    /// 触发连锁效果
    /// </summary>
    private void TriggerChainEffect(GameObject startTarget, SkillData skill)
    {
        GameObject currentTarget = startTarget;
        int chainTimes = skill.ChainCount;

        for (int i = 0; i < chainTimes; i++)
        {
            Collider[] hits = Physics.OverlapSphere(currentTarget.transform.position, skill.ChainRadius, LayerMask.GetMask("Enemy"));

            GameObject nextTarget = null;
            float minDistance = float.MaxValue;

            foreach (var hit in hits)
            {
                if (hit.gameObject != currentTarget)
                {
                    float dist = Vector3.Distance(currentTarget.transform.position, hit.transform.position);
                    if (dist < minDistance)
                    {
                        minDistance = dist;
                        nextTarget = hit.gameObject;
                    }
                }
            }

            if (nextTarget != null)
            {
                // 生成连锁特效
                Vector3 from = currentTarget.transform.position;
                Vector3 to = nextTarget.transform.position;
                Vector3 midPoint = (from + to) / 2;

                if (skill.ChainEffectPrefab != null)
                {
                    GameObject chainObj = ObjectPool.Instance.Get("ChainEffect", skill.ChainEffectPrefab);
                    if (chainObj != null)
                    {
                        chainObj.transform.position = midPoint;
                        chainObj.transform.rotation = Quaternion.identity;
                        ObjectPool.Instance.AutoRelease("ChainEffect", chainObj, 5f);
                    }
                }

                currentTarget = nextTarget;
            }
            else
            {
                break;
            }
        }
    }
    
    private void ResetCastingState()
    {
        currentCastingSkill = null;
        isCasting = false;
        castTimer = 0f;
        castDuration = 0f;
    }

    private SkillData GetSkillData(string skillName)
    {
        return skillList.Find(skill => skill != null && skill.SkillName == skillName);
    }

    public bool IsSkillOnCooldown(string skillName)
    {
        return cooldownTimers.ContainsKey(skillName) && cooldownTimers[skillName] > 0f;
    }
    
    /// <summary>
    /// 获取技能冷却进度（0-1）
    /// </summary>
    public float GetSkillCooldownProgress(string skillName)
    {
        if (!cooldownTimers.ContainsKey(skillName)) return 1f;
        var skill = GetSkillData(skillName);
        if (skill == null || skill.CooldownTime <= 0) return 1f;
        return Mathf.Clamp01(1f - (cooldownTimers[skillName] / skill.CooldownTime));
    }
}

