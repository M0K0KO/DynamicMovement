using System.Collections;
using UnityEngine;

public class PlayerFallState : BaseState
{
    private PlayerStateMachine stateMachine;
    private PlayerManager playerManager;
    private PlayerStateManager stateManager;
    private PlayerInputManager inputManager;
    private PlayerLocomotionManager locomotionManager;

    private float fallTime = 0f;

    public PlayerFallState(PlayerStateMachine stateMachine)
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
        locomotionParams = new LocomotionParameters
        {
            PerformMovement = false,
            
            PerformRotation = true,
            RotationSpeed = playerManager.airRotationSpeed,
            InstantRotation = false,
        };
        
        ApplyStoredVelocity();

        locomotionManager.PlayFallAnimation();
    }
    
    public override void OnUpdateState()
    {
        fallTime += Time.deltaTime;
        
        if (stateManager.isGrounded)
        {
            if (fallTime >= 0.3f)
            {
                stateMachine.ChangeState(stateMachine.landState);
                return;
            }
            else
            {
                if (inputManager.moveInput == Vector2.zero)
                {
                    stateMachine.ChangeState(stateMachine.idleState);
                    return;
                }
                else
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
        }
    }
    
    public override void OnFixedUpdateState()
    {
        locomotionManager.ApplyAirControlForce(playerManager.airAcceleration);
        locomotionManager.HandleMovement(locomotionParams);
    }
    
    public override void OnExitState()
    {
        fallTime = 0;
    }

    private void ApplyStoredVelocity()
    {
        playerManager.rb.linearVelocity = stateMachine.storedVelocityBeforeFall;
    }
}
