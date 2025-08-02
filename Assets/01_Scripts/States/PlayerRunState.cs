using System;
using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerRunState : BaseState
{
    private PlayerStateMachine stateMachine;
    private PlayerManager playerManager;
    private PlayerStateManager stateManager;
    private PlayerInputManager inputManager;
    private PlayerLocomotionManager locomotionManager;
    
    public PlayerRunState(PlayerStateMachine stateMachine)
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
            RotationSpeed = playerManager.runRotationSpeed,
            
            PerformMovement = true,
            MoveSpeed = playerManager.runSpeed
        };
    }

    public override void OnUpdateState()
    {
        locomotionParams.MoveSpeed = stateMachine.isLockedOn ? playerManager.lockOnRunSpeed : playerManager.runSpeed;
        
        if (stateMachine.isRunning == false)
        {
            stateMachine.ChangeState(stateMachine.walkState);
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