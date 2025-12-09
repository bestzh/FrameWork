using UnityEngine;

/// <summary>
/// 攻击状态
/// </summary>
public class AttackState : PlayerState
{
    private float attackTime = 0.5f;  // 攻击持续时间
    private float timer;

    public AttackState(PlayerController player) : base(player) { }

    public override void Enter()
    {
        base.Enter();
        Debug.Log("[PlayerController] 攻击！");
        timer = attackTime;

        if (player.animator != null)
        {
            player.animator.SetTrigger("Attack"); // 播放攻击动画
        }
        
        DetectEnemies(); // 执行敌人检测
        
        // 触发战斗事件
        EventManager.Instance?.TriggerEvent("PLAYER_ATTACK", null);
    }

    public override void Update()
    {
        timer -= Time.deltaTime;
        if (timer <= 0)
        {
            player.StateMachine.ChangeState(new IdleState(player));
        }
    }

    /// <summary>
    /// 检测敌人并造成伤害
    /// </summary>
    private void DetectEnemies()
    {
        // 检测攻击范围内的敌人
        Collider[] hits = Physics.OverlapSphere(player.transform.position + player.transform.forward, 1.5f);
        foreach (var hit in hits)
        {
            if (hit.CompareTag("Enemy"))
            {
                var enemyController = hit.GetComponent<EnemyController>();
                if (enemyController != null)
                {
                    int damage = player.CharacterData != null ? player.CharacterData.Strength : 20;
                    enemyController.TakeDamage(damage);
                }
            }
        }
    }
}

