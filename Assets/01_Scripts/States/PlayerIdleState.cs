using System.Collections;
using UnityEngine;

public class PlayerIdleState : BaseState
{
    private PlayerStateMachine stateMachine;
    private PlayerManager playerManager;
    private PlayerStateManager stateManager;
    private PlayerInputManager inputManager;
    private PlayerLocomotionManager locomotionManager;

    private Coroutine stopCoroutine;
    
    public PlayerIdleState(PlayerStateMachine stateMachine)
    {
        this.stateMachine = stateMachine;
        playerManager = stateMachine.playerManager;
        stateManager = playerManager.StateManager;
        inputManager = playerManager.InputManager;
        locomotionManager = playerManager.LocomotionManager;
    }

    public override void OnEnterState()
    {
        locomotionManager.PlayGroundedAnimation();
        playerManager.rb.linearVelocity = Vector3.zero;
    }
    
    public override void OnUpdateState()
    {
        playerManager.rb.linearVelocity = Vector3.zero;
        
        if (stateMachine.playerManager.InputManager.moveInput != Vector2.zero)
        {
            if (stateMachine.isRunning)
            {
                stateMachine.ChangeState(stateMachine.runState);
                return;
            }
            else
            {
                stateMachine.ChangeState(stateMachine.walkState);
                return;
            }
        }
    }
    
    public override void OnFixedUpdateState()
    {
        
    }
    
    public override void OnExitState()
    {
        if (stopCoroutine != null)
        {
            stateMachine.StopCoroutine(stopCoroutine);
        }
    }
}
