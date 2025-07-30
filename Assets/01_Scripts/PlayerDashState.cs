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

    public Coroutine dashCoroutine;
    
    private Vector3 moveDirection;
    private bool dashed = false;
    private bool isDashing = false;

    public override void OnEnterState()
    {
        playerManager.maxAirSpeed = playerManager.dashSpeed;
        stateManager.lockOnSlope = false;
        
        if (inputManager.moveInput != Vector2.zero && !stateMachine.isLockedOn)
            HandleNormalRotation();
        moveDirection = stateManager.forward; //?

        if (stateMachine.isLockedOn)
        {
            playerManager.animator.SetFloat(stateMachine.DASHHORIZONTAL_PARAM, Mathf.Sign(inputManager.moveInput.x));
            if (Mathf.Approximately(inputManager.moveInput.x, 0f))
                playerManager.animator.SetFloat(stateMachine.DASHHORIZONTAL_PARAM, 0f);
            playerManager.animator.SetFloat(stateMachine.DASHVERTICAL_PARAM, Mathf.Sign(inputManager.moveInput.y));
            if (Mathf.Approximately(inputManager.moveInput.y, 0f))
                playerManager.animator.SetFloat(stateMachine.DASHVERTICAL_PARAM, 0f);
            
            stateMachine.PlayAnimation(stateMachine.COMBAT_DASH, 0.1f, 0.1f);
        }
        else
        {
            if (stateManager.isGrounded) stateMachine.PlayAnimation(stateMachine.DASH_F_0, 0.1f, 0.1f);
            else stateMachine.PlayAnimation(stateMachine.DASH_AIR_F_0);
        }

        if (stateMachine.isLockedOn) dashCoroutine = stateMachine.StartCoroutine(StartDash(playerManager.lockOnDashSpeed, playerManager.lockOnDashDuration));
        else dashCoroutine = stateMachine.StartCoroutine(StartDash(playerManager.dashSpeed, playerManager.dashDuration));
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
        //if (stateMachine.isLockedOn) HandleLockOnRotation();
    }
    
    public override void OnExitState()
    {
        stateManager.lockOnSlope = true;
    }

    
    private void HandleNormalRotation()
    {
        float targetAngle = Mathf.Atan2(
            inputManager.moveInput.x,
            inputManager.moveInput.y
        ) * Mathf.Rad2Deg + playerManager.playerCam.transform.eulerAngles.y;
        
        stateManager.targetAngle = targetAngle;

        Quaternion targetRotation = Quaternion.Euler(0f, targetAngle, 0f);
        if (!stateMachine.isLockedOn) playerManager.rb.MoveRotation(targetRotation);
    }
    
    private void HandleLockOnRotation()
    {
        Vector3 directionToTarget = stateMachine.lockOnTarget.transform.position - playerManager.transform.position;
        directionToTarget.y = 0;
        directionToTarget.Normalize(); 
        
        Quaternion targetRotation = Quaternion.LookRotation(directionToTarget);

        Quaternion newRotation = Quaternion.Slerp(
            playerManager.rb.rotation,
            targetRotation,
            playerManager.runRotationSpeed * Time.fixedDeltaTime 
        );

        playerManager.rb.MoveRotation(newRotation);
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
            
            if (Vector3.Distance(moveDirection, Vector2.zero) < 0.5f) moveDirection = playerManager.transform.forward;
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
