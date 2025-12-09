using UnityEngine;
using XLua;

/// <summary>
/// 玩家控制器 - 管理玩家移动、攻击、状态等
/// 适配到框架的即时战斗系统
/// </summary>
[XLua.LuaCallCSharp]
public class PlayerController : MonoBehaviour
{
    public PlayerStateMachine StateMachine { get; private set; }
    
    /// <summary>
    /// 移动输入（从InputManager获取）
    /// </summary>
    public Vector2 InputDir { get; private set; }

    public Animator animator;
    public bool IsRunning => InputDir.magnitude > 0.5f;

    [Header("移动设置")]
    public float MoveSpeed = 5f;
    public bool IsGrounded => Physics.Raycast(transform.position, Vector3.down, 1.1f);

    [Header("生命值设置")]
    public int MaxHealth = 100;
    public int CurrentHealth { get; private set; }

    public Rigidbody rb;

    [Header("相机控制")]
    private float yaw;   // 水平旋转
    private float pitch; // 垂直旋转
    public float mouseSensitivity = 3f;
    
    [Header("动作设置")]
    public float RollForce = 5f;
    public bool IsJumping { get; private set; } = false;
    public bool IsRolling { get; private set; } = false;
    
    /// <summary>
    /// 动画控制器
    /// </summary>
    public PlayerAnimationController AnimationController { get; private set; }
    
    /// <summary>
    /// 技能释放管理器
    /// </summary>
    public SkillSystem SkillSystem { get; private set; }
    
    /// <summary>
    /// 角色数据（关联到CharacterData）
    /// </summary>
    public CharacterData CharacterData { get; set; }

    private void Awake()
    {
        StateMachine = new PlayerStateMachine();
        AnimationController = GetComponent<PlayerAnimationController>();
        if (AnimationController == null)
        {
            AnimationController = gameObject.AddComponent<PlayerAnimationController>();
        }
        
        SkillSystem = GetComponent<SkillSystem>();
        if (SkillSystem == null)
        {
            SkillSystem = gameObject.AddComponent<SkillSystem>();
        }
        
        rb = GetComponent<Rigidbody>();
        if (rb == null)
        {
            rb = gameObject.AddComponent<Rigidbody>();
        }
        
        animator = GetComponent<Animator>();
        CurrentHealth = MaxHealth;
    }

    private void Start()
    {
        StateMachine.ChangeState(new IdleState(this));
        
        // 关联角色数据
        if (CharacterData == null && CharacterManager.Instance != null)
        {
            CharacterData = CharacterManager.Instance.PlayerCharacter;
        }
        
        // 同步生命值
        if (CharacterData != null)
        {
            CurrentHealth = CharacterData.HP;
            MaxHealth = CharacterData.MaxHP;
        }
    }

    private void Update()
    {
        // 更新输入
        UpdateInput();
        
        // 更新状态机
        StateMachine.Update();

        // 处理攻击输入
        if (Input.GetMouseButtonDown(0))
        {
            if (!IsRolling && !IsJumping) // 防止翻滚/跳跃中攻击
            {
                // 使用技能系统攻击
                if (SkillSystem != null)
                {
                    SkillSystem.TryCastSkill("Attack");
                }
                else
                {
                    StateMachine.ChangeState(new AttackState(this));
                }
            }
        }
        
        // 翻滚
        if(Input.GetKeyDown(KeyCode.LeftShift))
        {
            StateMachine.ChangeState(new RollState(this));
        }
        
        // 跳跃
        if (Input.GetKeyDown(KeyCode.Space))
        {
            StateMachine.ChangeState(new JumpState(this));
        }

        // 相机旋转
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity;
        yaw += mouseX;
        pitch -= mouseY;
        pitch = Mathf.Clamp(pitch, -35f, 60f);
        this.transform.rotation = Quaternion.Euler(0f, yaw, 0f);
    }
    
    private void FixedUpdate()
    {
        StateMachine.FixedUpdate();
    }
    
    /// <summary>
    /// 更新输入
    /// </summary>
    private void UpdateInput()
    {
        float h = Input.GetAxisRaw("Horizontal");
        float v = Input.GetAxisRaw("Vertical");
        InputDir = new Vector2(h, v).normalized;
    }
    
    public void SetIsJumping(bool jumping)
    {
        IsJumping = jumping;
    }
    
    public void SetIsRolling(bool rolling)
    {
        IsRolling = rolling;
    }

    /// <summary>
    /// 受到伤害
    /// </summary>
    public void TakeDamage(int damage)
    {
        if (CurrentHealth <= 0) return;

        CurrentHealth -= damage;
        CurrentHealth = Mathf.Max(0, CurrentHealth);
        
        // 同步到角色数据
        if (CharacterData != null)
        {
            CharacterData.HP = CurrentHealth;
        }

        // 触发事件
        EventManager.Instance?.TriggerEvent(GlobalEventNames.PLAYER_HP_CHANGED, CurrentHealth);

        if (CurrentHealth <= 0)
        {
            StateMachine.ChangeState(new DeadState(this));
            EventManager.Instance?.TriggerEvent(GlobalEventNames.PLAYER_DEATH, null);
        }
        else
        {
            StateMachine.ChangeState(new HurtState(this));
        }
    }
    
    /// <summary>
    /// 治疗
    /// </summary>
    public void Heal(int amount)
    {
        CurrentHealth = Mathf.Min(MaxHealth, CurrentHealth + amount);
        
        // 同步到角色数据
        if (CharacterData != null)
        {
            CharacterData.HP = CurrentHealth;
        }
        
        // 触发事件
        EventManager.Instance?.TriggerEvent(GlobalEventNames.PLAYER_HP_CHANGED, CurrentHealth);
    }
    
    private void OnGetHit()
    {
        SkillSystem?.InterruptSkill();
    }
}

