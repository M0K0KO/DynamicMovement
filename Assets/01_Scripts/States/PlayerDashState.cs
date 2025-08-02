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
    private PlayerLocomotionManager locomotionManager;

    public PlayerDashState(PlayerStateMachine stateMachine)
    {
        this.stateMachine = stateMachine;
        playerManager = stateMachine.playerManager;
        stateManager = playerManager.StateManager;
        inputManager = playerManager.InputManager;
        locomotionManager = playerManager.LocomotionManager;
    }

    private Vector3 moveDirection;
    private bool dashed = false;
    private bool isDashing = false;

    public override void OnEnterState()
    {
        playerManager.maxAirSpeed = playerManager.dashSpeed;
        //stateManager.lockOnSlope = false;

        if (inputManager.moveInput != Vector2.zero && !stateMachine.isLockedOn) locomotionManager.HandleNormalRotation(0f, true);
        if (stateMachine.isLockedOn) locomotionManager.HandleLockOnRotation(0f, true);
        
        
        moveDirection = stateManager.forward; 
        
        locomotionManager.PlayDashAnimation();
        stateMachine.StartCoroutine(
            StartDash(stateMachine.isLockedOn ? playerManager.lockOnDashSpeed : playerManager.dashSpeed,
                stateMachine.isLockedOn ? playerManager.lockOnDashDuration : playerManager.dashDuration));
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
    
    IEnumerator StartDash(float dashSpeed, float dashDuration)
    {
        
        dashed = true;
        isDashing = true;

        float elapsedTime = 0f;
        while (true)
        {
            elapsedTime += Time.deltaTime;
            if (elapsedTime >= dashDuration) break;

            if (Vector3.Distance(moveDirection, Vector2.zero) < 0.5f) moveDirection = stateManager.forward;
            Vector3 velocity = moveDirection * dashSpeed;

            Vector3 finalVelocity = new Vector3(velocity.x, playerManager.rb.linearVelocity.y, velocity.z);
            
            if (!stateManager.isGrounded && playerManager.rb.linearVelocity.y < 0f)
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
