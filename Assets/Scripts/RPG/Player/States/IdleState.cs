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
        if (player.animator != null)
        {
            player.animator.SetFloat("Horizontal", 0);
            player.animator.SetFloat("Vertical", 0);
        }
    }

    public override void Update()
    {
        // 如果有输入，进入移动状态
        if (player.InputDir.magnitude > 0.1f)
        {
            player.StateMachine.ChangeState(new MoveState(player));
        }
    }

    public override void Exit()
    {
        base.Exit();
    }
}

