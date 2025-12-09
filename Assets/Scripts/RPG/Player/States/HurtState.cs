using UnityEngine;

/// <summary>
/// 受伤状态
/// </summary>
public class HurtState : PlayerState
{
    private float hurtDuration = 0.3f;
    private float timer;

    public HurtState(PlayerController player) : base(player) { }

    public override void Enter()
    {
        base.Enter();
        timer = hurtDuration;
        
        if (player.animator != null)
        {
            player.animator.SetTrigger("Hurt");
        }
    }

    public override void Update()
    {
        timer -= Time.deltaTime;
        if (timer <= 0)
        {
            player.StateMachine.ChangeState(new IdleState(player));
        }
    }
}

