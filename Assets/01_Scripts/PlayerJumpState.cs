using System.Collections;
using UnityEngine;

public class PlayerJumpState : BaseState
{
    private PlayerStateMachine stateMachine;
    private PlayerManager playerManager;
    private PlayerStateManager stateManager;
    private PlayerInputManager inputManager;

    private float jumpBufferTime = 0f;
    private float jumpBufferThreshold = 0.2f;
    
    public PlayerJumpState(PlayerStateMachine stateMachine)
    {
        this.stateMachine = stateMachine;
        playerManager = stateMachine.playerManager;
        stateManager = playerManager.StateManager;
        inputManager = playerManager.InputManager;
    }
    
    public override void OnEnterState()
    {
        jumpBufferTime = 0;
        
        Debug.Log("Player Jump State");
        
        stateMachine.PlayAnimation(stateMachine.JUMP_0_START);
        ApplyJumpForce();
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
        HandleAirControl();
    }
    
    public override void OnExitState()
    {
        stateMachine.storedVelocityBeforeFall = playerManager.rb.linearVelocity;

        jumpBufferTime = 0f;
    }

    private void ApplyJumpForce()
    {
        if (inputManager.moveInput == Vector2.zero)
        {
            playerManager.rb.linearVelocity = new Vector3(0f, 12f, 0f);
        }
        else
        {
            float horizontalVelocityX = playerManager.rb.linearVelocity.x;
            float horizontalVelocityZ = playerManager.rb.linearVelocity.z;

            playerManager.rb.linearVelocity = new Vector3(horizontalVelocityX, 12f, horizontalVelocityZ);
        }
    }
    
    private void HandleAirControl()
    {
        float targetAngle = Mathf.Atan2(
            inputManager.moveInput.x,
            inputManager.moveInput.y
        ) * Mathf.Rad2Deg + playerManager.playerCam.transform.eulerAngles.y;
    
        Quaternion targetRotation = Quaternion.Euler(0f, targetAngle, 0f);

        Quaternion newRotation = Quaternion.Slerp(
            playerManager.rb.rotation, 
            targetRotation,            
            playerManager.airRotationSpeed * Time.fixedDeltaTime 
        );

        playerManager.rb.MoveRotation(newRotation);

        Vector3 camForward = playerManager.playerCam.transform.forward;
        Vector3 camRight = playerManager.playerCam.transform.right;
        camForward.y = 0;
        camRight.y = 0;
        camForward.Normalize();
        camRight.Normalize();
    
        Vector3 moveDirection = (camForward * inputManager.moveInput.y + camRight * inputManager.moveInput.x).normalized;

        playerManager.rb.AddForce(moveDirection * playerManager.airAcceleration, ForceMode.Acceleration);

        Vector3 horizontalVelocity = new Vector3(playerManager.rb.linearVelocity.x, 0, playerManager.rb.linearVelocity.z);
        if (horizontalVelocity.magnitude > playerManager.maxAirSpeed)
        {
            Vector3 clampedVelocity = horizontalVelocity.normalized * playerManager.maxAirSpeed;
            playerManager.rb.linearVelocity = new Vector3(clampedVelocity.x, playerManager.rb.linearVelocity.y, clampedVelocity.z);
        }
    }
}