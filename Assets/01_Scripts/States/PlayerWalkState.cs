using System.Collections;
using UnityEngine;

public class PlayerWalkState : BaseState
{
    private PlayerStateMachine stateMachine;
    private PlayerManager playerManager;
    private PlayerStateManager stateManager;
    private PlayerInputManager inputManager;
    private PlayerLocomotionManager locomotionManager;
    
    public PlayerWalkState(PlayerStateMachine stateMachine)
    {
        this.stateMachine = stateMachine;
        playerManager = stateMachine.playerManager;
        stateManager = playerManager.StateManager;
        inputManager = playerManager.InputManager;
        locomotionManager = playerManager.LocomotionManager;
    }

    private LocomotionParameters locomotionParams;
    
    public override void OnEnterState()
    {
        locomotionManager.PlayGroundedAnimation();
        
        locomotionParams = new LocomotionParameters
        {
            PerformRotation = true,
            InstantRotation = false,
            RotationSpeed = playerManager.walkRotationSpeed,
            
            PerformMovement = true,
            MoveSpeed = playerManager.walkSpeed,
        };
    }

    public override void OnUpdateState()
    {
        if (stateMachine.isRunning)
        {
            stateMachine.ChangeState(stateMachine.idleState);
            return;
        }

        if (inputManager.moveInput == Vector2.zero)
        {
            stateMachine.ChangeState(stateMachine.idleState);
            return;
        }
    }

    public override void OnFixedUpdateState()
    {
        locomotionManager.HandleMovement(locomotionParams);
    }

    public override void OnExitState()
    {
    }
}