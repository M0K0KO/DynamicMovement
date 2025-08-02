using System.Collections;
using UnityEngine;

public class PlayerJumpState : BaseState
{
    private PlayerStateMachine stateMachine;
    private PlayerManager playerManager;
    private PlayerStateManager stateManager;
    private PlayerInputManager inputManager;
    private PlayerLocomotionManager locomotionManager;

    private float jumpBufferTime = 0f;
    private float jumpBufferThreshold = 0.2f;
    
    private LocomotionParameters locomotionParams;
    
    public PlayerJumpState(PlayerStateMachine stateMachine)
    {
        this.stateMachine = stateMachine;
        playerManager = stateMachine.playerManager;
        stateManager = playerManager.StateManager;
        inputManager = playerManager.InputManager;
        locomotionManager = playerManager.LocomotionManager;
    }
    
    public override void OnEnterState()
    {
        locomotionParams = new LocomotionParameters
        {
            PerformMovement = false,
            
            PerformRotation = true,
            RotationSpeed = playerManager.airRotationSpeed,
            InstantRotation = false,
        };
        
        jumpBufferTime = 0;
        stateManager.lockOnSlope = false;

        locomotionManager.PlayJumpAnimation();
        stateMachine.StartCoroutine(StartJump(playerManager.jumpDuration));
    }
    
    public override void OnUpdateState()
    {
        if (playerManager.rb.linearVelocity.y <= 0f)
        {
            stateMachine.ChangeState(stateMachine.fallState);
            return;
        }


        if (jumpBufferTime < jumpBufferThreshold)
        {
            jumpBufferTime += Time.deltaTime;
        }
        else
        {
            if (stateManager.isGrounded)
            {
                if (inputManager.moveInput == Vector2.zero)
                    stateMachine.ChangeState(stateMachine.idleState);
                else
                {
                    if (stateMachine.isRunning)
                        stateMachine.ChangeState(stateMachine.runState);
                    else
                        stateMachine.ChangeState(stateMachine.walkState);
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
        stateMachine.storedVelocityBeforeFall = playerManager.rb.linearVelocity;
        playerManager.maxAirSpeed = playerManager.runSpeed;
        stateManager.lockOnSlope = true;
        jumpBufferTime = 0f;
    }

    IEnumerator StartJump(float jumpDuration)
    {
        float elapsedTime = 0f;
        while (true)
        {
            elapsedTime += Time.deltaTime;
            if (elapsedTime >= jumpDuration) break;

            float horizontalVelocityX = playerManager.rb.linearVelocity.x;
            float horizontalVelocityZ = playerManager.rb.linearVelocity.z;
            Vector3 finalVelocity = new Vector3(horizontalVelocityX, playerManager.jumpSpeed, horizontalVelocityZ);
            playerManager.rb.linearVelocity = finalVelocity;
            yield return null;
        }
    }
}