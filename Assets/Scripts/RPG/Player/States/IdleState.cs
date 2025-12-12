using UnityEngine;

/// <summary>
/// 待机状态
/// </summary>
public class IdleState : PlayerState
{
    public IdleState(PlayerController player) : base(player) { }

    public override void Enter()
    {
        base.Enter();
        // 注意：Enter时不需要立即设置，让Update中的平滑过渡处理
    }
    
    public override void Update()
    {
        // 如果有输入，进入移动状态
        if (player.InputDir.magnitude > 0.1f)
        {
            player.StateMachine.ChangeState(new MoveState(player));
            return;
        }
        
        // 平滑过渡到待机动画
        if (player.animator != null)
        {
            float dampTime = player.animationDampTime;
            player.animator.SetFloat("Horizontal", 0, dampTime, Time.deltaTime);
            player.animator.SetFloat("Vertical", 0, dampTime, Time.deltaTime);
            player.animator.SetFloat("Speed", 0, dampTime, Time.deltaTime);
        }
    }


    public override void Exit()
    {
        base.Exit();
    }
}

