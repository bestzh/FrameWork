using UnityEngine;
using XLua;

/// <summary>
/// 敌人控制器 - 管理敌人AI和行为
/// 适配到框架的即时战斗系统
/// </summary>
[XLua.LuaCallCSharp]
public class EnemyController : MonoBehaviour
{
    [Header("移动设置")]
    public float moveSpeed = 3f;
    public float attackDistance = 2f;
    
    [Header("攻击设置")]
    public float attackCooldown = 2f;
    public int damage = 10;
    
    [Header("生命值")]
    public int MaxHP = 50;
    public int CurrentHP { get; private set; }
    
    /// <summary>
    /// 关联的角色数据
    /// </summary>
    public CharacterData CharacterData { get; set; }

    private Transform target;
    private float attackTimer;
    private bool isDead = false;

    void Start()
    {
        // 查找玩家目标
        GameObject playerObj = GameObject.FindWithTag("Player");
        if (playerObj != null)
        {
            target = playerObj.transform;
        }
        
        // 从CharacterData获取数据
        if (CharacterData != null)
        {
            CurrentHP = CharacterData.HP;
            MaxHP = CharacterData.MaxHP;
            damage = CharacterData.Strength;
        }
        else
        {
            CurrentHP = MaxHP;
        }
        
        attackTimer = attackCooldown;
        
        // 创建角色数据（如果没有）
        if (CharacterData == null)
        {
            CharacterData = new CharacterData
            {
                CharacterId = GetInstanceID(),
                Name = gameObject.name,
                HP = CurrentHP,
                MaxHP = MaxHP,
                Strength = damage
            };
        }
    }

    void Update()
    {
        if (isDead || target == null) return;

        Vector3 direction = target.position - transform.position;
        direction.y = 0f; // 水平方向

        float distance = direction.magnitude;
        
        if (distance > attackDistance)
        {
            // 移动向玩家
            if (direction != Vector3.zero)
            {
                Quaternion toRotation = Quaternion.LookRotation(direction);
                transform.rotation = Quaternion.Slerp(transform.rotation, toRotation, Time.deltaTime * 5f);
            }

            Vector3 moveDir = direction.normalized;
            transform.position += moveDir * moveSpeed * Time.deltaTime;
        }
        else
        {
            // 在攻击范围内，等待冷却后攻击
            attackTimer -= Time.deltaTime;
            if (attackTimer <= 0f)
            {
                Attack();
                attackTimer = attackCooldown;
            }
        }
    }

    /// <summary>
    /// 攻击玩家
    /// </summary>
    void Attack()
    {
        if (target == null) return;
        
        Debug.Log($"[EnemyController] {gameObject.name} 攻击了玩家！");
        
        var playerController = target.GetComponent<PlayerController>();
        if (playerController != null)
        {
            playerController.TakeDamage(damage);
        }
        
        // 触发事件
        EventManager.Instance?.TriggerEvent("ENEMY_ATTACK", damage);
    }
    
    /// <summary>
    /// 受到伤害
    /// </summary>
    public void TakeDamage(int damage)
    {
        if (isDead) return;
        
        CurrentHP -= damage;
        CurrentHP = Mathf.Max(0, CurrentHP);
        
        // 同步到角色数据
        if (CharacterData != null)
        {
            CharacterData.HP = CurrentHP;
        }
        
        Debug.Log($"[EnemyController] {gameObject.name} 受到 {damage} 点伤害，剩余HP: {CurrentHP}");
        
        // 触发事件
        EventManager.Instance?.TriggerEvent("ENEMY_TAKE_DAMAGE", damage);
        
        if (CurrentHP <= 0)
        {
            Die();
        }
    }
    
    /// <summary>
    /// 死亡
    /// </summary>
    private void Die()
    {
        if (isDead) return;
        
        isDead = true;
        Debug.Log($"[EnemyController] {gameObject.name} 死亡");
        
        // 触发事件
        EventManager.Instance?.TriggerEvent("ENEMY_DEATH", CharacterData);
        
        // 延迟销毁（可以播放死亡动画）
        Destroy(gameObject, 2f);
    }
    
    /// <summary>
    /// 治疗
    /// </summary>
    public void Heal(int amount)
    {
        CurrentHP = Mathf.Min(MaxHP, CurrentHP + amount);
        
        if (CharacterData != null)
        {
            CharacterData.HP = CurrentHP;
        }
    }
}

