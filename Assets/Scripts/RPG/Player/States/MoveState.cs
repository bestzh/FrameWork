using UnityEngine;

/// <summary>
/// 移动状态
/// </summary>
public class MoveState : PlayerState
{
    public MoveState(PlayerController player) : base(player) { }

    public override void Enter()
    {
        base.Enter();
    }

    public override void Update()
    {
        // 如果没有输入，进入待机状态
        if (player.InputDir.magnitude < 0.1f)
        {
            player.StateMachine.ChangeState(new IdleState(player));
            return;
        }
        
        // 更新动画（使用平滑过渡）
        if (player.animator != null)
        {
            float dampTime = player.animationDampTime;
            
            // 使用平滑过渡设置动画参数
            player.animator.SetFloat("Horizontal", player.SmoothedInputDir.x, dampTime, Time.deltaTime);
            player.animator.SetFloat("Vertical", player.SmoothedInputDir.y, dampTime, Time.deltaTime);
            
            // 设置速度参数（用于控制动画播放速度）
            float speed = player.InputDir.magnitude;
            player.animator.SetFloat("Speed", speed, dampTime, Time.deltaTime);
        }
    }
    
    public override void FixedUpdate()
    {
        // 移动逻辑
        if (player.rb != null && player.IsGrounded)
        {
            Vector3 moveDirection = new Vector3(player.InputDir.x, 0, player.InputDir.y);
            moveDirection = player.transform.rotation * moveDirection;
            player.rb.linearVelocity = new Vector3(moveDirection.x * player.MoveSpeed, player.rb.linearVelocity.y, moveDirection.z * player.MoveSpeed);
        }
    }

    public override void Exit()
    {
        base.Exit();
    }
}

