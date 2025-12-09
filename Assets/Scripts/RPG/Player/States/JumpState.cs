using UnityEngine;

/// <summary>
/// 跳跃状态
/// </summary>
public class JumpState : PlayerState
{
    private float jumpForce = 5f;
    private bool hasJumped = false;

    public JumpState(PlayerController player) : base(player) { }

    public override void Enter()
    {
        base.Enter();
        hasJumped = false;
        player.SetIsJumping(true);
        
        if (player.animator != null)
        {
            player.animator.SetTrigger("Jump");
        }
    }

    public override void FixedUpdate()
    {
        if (!hasJumped && player.IsGrounded && player.rb != null)
        {
            player.rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
            hasJumped = true;
        }
    }

    public override void Update()
    {
        // 落地后回到待机状态
        if (hasJumped && player.IsGrounded)
        {
            player.SetIsJumping(false);
            player.StateMachine.ChangeState(new IdleState(player));
        }
    }

    public override void Exit()
    {
        base.Exit();
        player.SetIsJumping(false);
    }
}

