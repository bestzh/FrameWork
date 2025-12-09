/// <summary>
/// 玩家状态机 - 管理玩家状态切换
/// </summary>
public class PlayerStateMachine
{
    private PlayerState currentState;

    public void ChangeState(PlayerState newState)
    {
        if (newState.GetType() == currentState?.GetType())
        {
            return;
        }
        currentState?.Exit();
        currentState = newState;
        currentState.Enter();
    }

    public void Update()
    {
        currentState?.Update();
    }
    
    public void FixedUpdate()
    {
        currentState?.FixedUpdate();
    }
    
    public PlayerState GetCurrentState()
    {
        return currentState;
    }
}

