using UnityEngine;

public class PlayerSlideState : BaseState
{
    private PlayerStateMachine stateMachine;
    private PlayerManager playerManager;
    private PlayerStateManager stateManager;
    private PlayerInputManager inputManager;
    
    public PlayerSlideState(PlayerStateMachine stateMachine)
    {
        this.stateMachine = stateMachine;
        playerManager = stateMachine.playerManager;
        stateManager = playerManager.StateManager;
        inputManager = playerManager.InputManager;
    }
    
    public override void OnEnterState()
    {
        Debug.Log("Player Starts Sliding");
        
        playerManager.rb.linearVelocity = Vector3.zero;
        stateMachine.PlayAnimation(stateMachine.IDLE_ANIMATION);
    }
    
    public override void OnUpdateState()
    {
        if (stateManager.currentSurfaceAngle <= stateManager.maxClimbableSlopeAngle)
        {
            stateMachine.ChangeState(stateMachine.idleState);
            return;
        }
        
        if (!stateManager.isGrounded)
        {
            // FallState가 있다면 아래 코드를 사용
            // stateMachine.ChangeState(stateMachine.fallState); 
        }
    }
    
    public override void OnFixedUpdateState()
    {
        
    }
    
    public override void OnExitState()
    {
        Debug.Log("Player Starts Sliding");
    }
}