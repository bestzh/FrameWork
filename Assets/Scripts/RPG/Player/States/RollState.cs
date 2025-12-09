using UnityEngine;

/// <summary>
/// 翻滚状态
/// </summary>
public class RollState : PlayerState
{
    private float rollDuration = 0.5f;
    private float timer;
    private Vector3 rollDirection;

    public RollState(PlayerController player) : base(player) { }

    public override void Enter()
    {
        base.Enter();
        timer = rollDuration;
        player.SetIsRolling(true);
        
        // 确定翻滚方向
        if (player.InputDir.magnitude > 0.1f)
        {
            rollDirection = new Vector3(player.InputDir.x, 0, player.InputDir.y);
            rollDirection = player.transform.rotation * rollDirection;
        }
        else
        {
            rollDirection = player.transform.forward;
        }
        
        if (player.animator != null)
        {
            player.animator.SetTrigger("Roll");
        }
    }

    public override void FixedUpdate()
    {
        if (player.rb != null)
        {
            player.rb.linearVelocity = rollDirection.normalized * player.RollForce;
        }
    }

    public override void Update()
    {
        timer -= Time.deltaTime;
        if (timer <= 0)
        {
            player.SetIsRolling(false);
            player.StateMachine.ChangeState(new IdleState(player));
        }
    }

    public override void Exit()
    {
        base.Exit();
        player.SetIsRolling(false);
    }
}

