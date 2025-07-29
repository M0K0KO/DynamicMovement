using System.Collections;
using UnityEngine;

public class PlayerFallState : BaseState
{
    private PlayerStateMachine stateMachine;
    private PlayerManager playerManager;
    private PlayerStateManager stateManager;
    private PlayerInputManager inputManager;

    private float fallTime = 0f;
    
    public PlayerFallState(PlayerStateMachine stateMachine)
    {
        this.stateMachine = stateMachine;
        playerManager = stateMachine.playerManager;
        stateManager = playerManager.StateManager;
        inputManager = playerManager.InputManager;
    }
    
    public override void OnEnterState()
    {
        ApplyStoredVelocity();
        
        stateMachine.PlayAnimation(stateMachine.JUMP_0_LOOP);
    }
    
    public override void OnUpdateState()
    {
        fallTime += Time.deltaTime;
        
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
    
    public override void OnFixedUpdateState()
    {
        HandleNormalAirControl();
    }
    
    public override void OnExitState()
    {
        if (fallTime >= 0.3f) stateMachine.shouldPlayJumpEnd = true;
        fallTime = 0;
    }

    private void ApplyStoredVelocity()
    {
        playerManager.rb.linearVelocity = stateMachine.storedVelocityBeforeFall;
    }
    
    private void HandleNormalAirControl()
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
