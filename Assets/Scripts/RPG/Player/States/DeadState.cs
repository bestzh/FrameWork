using UnityEngine;

/// <summary>
/// 死亡状态
/// </summary>
public class DeadState : PlayerState
{
    public DeadState(PlayerController player) : base(player) { }

    public override void Enter()
    {
        base.Enter();
        
        if (player.animator != null)
        {
            player.animator.SetTrigger("Dead");
        }
        
        Debug.Log("[PlayerController] 玩家死亡");
    }

    public override void Update()
    {
        // 死亡状态不更新
    }
}

