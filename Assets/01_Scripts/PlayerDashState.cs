using System.Collections;
using System.Numerics;
using UnityEngine;
using Quaternion = UnityEngine.Quaternion;
using Vector2 = UnityEngine.Vector2;
using Vector3 = UnityEngine.Vector3;

public class PlayerDashState : BaseState
{
    private PlayerStateMachine stateMachine;
    private PlayerManager playerManager;
    private PlayerStateManager stateManager;
    private PlayerInputManager inputManager;

    public PlayerDashState(PlayerStateMachine stateMachine)
    {
        this.stateMachine = stateMachine;
        playerManager = stateMachine.playerManager;
        stateManager = playerManager.StateManager;
        inputManager = playerManager.InputManager;
    }

    private Vector3 moveDirection;
    private bool dashed = false;
    private bool isDashing = false;

    public override void OnEnterState()
    {
        playerManager.maxAirSpeed = playerManager.dashSpeed;
        stateManager.lockOnSlope = false;
        
        if (inputManager.moveInput != Vector2.zero)
            HandleRotation();
        moveDirection = stateManager.forward;
        
        if (stateManager.isGrounded) stateMachine.PlayAnimation(stateMachine.DASH_F_0);
        else stateMachine.PlayAnimation(stateMachine.DASH_AIR_F_0);

        stateMachine.StartCoroutine(StartDash(playerManager.dashDuration));
    }
    
    public override void OnUpdateState()
    {
        if (!isDashing && dashed)
        {
            if (stateManager.isGrounded)
            {
                if (inputManager.moveInput != Vector2.zero)
                {
                    if (stateMachine.isRunning)
                    {
                        stateMachine.ChangeState(stateMachine.runState);
                    }
                    else
                    {
                        stateMachine.ChangeState(stateMachine.walkState);
                    }
                }
                else
                {
                    stateMachine.ChangeState(stateMachine.idleState);
                }
            }
            else
            {
                stateMachine.ChangeState(stateMachine.fallState);
            }
        }
    }
    
    public override void OnFixedUpdateState()
    {
        
    }
    
    public override void OnExitState()
    {
        stateManager.lockOnSlope = true;
    }

    
    private void HandleRotation()
    {
        float targetAngle = Mathf.Atan2(
            inputManager.moveInput.x,
            inputManager.moveInput.y
        ) * Mathf.Rad2Deg + playerManager.playerCam.transform.eulerAngles.y;
        
        stateManager.targetAngle = targetAngle;

        Quaternion targetRotation = Quaternion.Euler(0f, targetAngle, 0f);
        playerManager.rb.MoveRotation(targetRotation);
    }
    
    IEnumerator StartDash(float dashDuration)
    {
        dashed = true;
        isDashing = true;

        float elapsedTime = 0f;
        while (true)
        {
            elapsedTime += Time.deltaTime;
            if (elapsedTime >= dashDuration) break;
            
            moveDirection = stateManager.forward;
            Vector3 velocity = moveDirection * playerManager.dashSpeed;

            Vector3 finalVelocity = new Vector3(velocity.x, playerManager.rb.linearVelocity.y, velocity.z);
            
            if (!stateManager.isGrounded)
            {
                finalVelocity += Vector3.up * 0.1f;
            }
            
            playerManager.rb.linearVelocity = finalVelocity;

            yield return null;
        }
        
        stateMachine.storedVelocityBeforeFall = new Vector3(playerManager.rb.linearVelocity.x, 0f, playerManager.rb.linearVelocity.z);

        isDashing = false;
    }
}
