using System.Collections;
using UnityEngine;

public class PlayerWalkState : BaseState
{
    private PlayerStateMachine stateMachine;
    private PlayerManager playerManager;
    private PlayerStateManager stateManager;
    private PlayerInputManager inputManager;
    
    public PlayerWalkState(PlayerStateMachine stateMachine)
    {
        this.stateMachine = stateMachine;
        playerManager = stateMachine.playerManager;
        stateManager = playerManager.StateManager;
        inputManager = playerManager.InputManager;
    }

    private Coroutine coroutine;

    private float currentHorizontal;
    private float currentVertical;
    
    public override void OnEnterState()
    {
        if (stateMachine.isLockedOn)
        {
            stateMachine.PlayAnimation(stateMachine.COMBAT_WALK);
        }
        else
        {
            if (stateMachine.previousState == stateMachine.fallState && stateMachine.shouldPlayJumpEnd)
            {
                stateMachine.shouldPlayJumpEnd = false;
                coroutine = stateMachine.StartCoroutine(PlayJumpEndAndWalk());
            }
            else
            {
                stateMachine.PlayAnimation(stateMachine.WALK_START_F_0, 0.3f);
            }
        }
    }

    public override void OnUpdateState()
    {
        if (stateMachine.isLockedOn)
        {
            float horizontalParam = playerManager.animator.GetFloat(stateMachine.HORIZONTAL_PARAM);
            float verticalParam = playerManager.animator.GetFloat(stateMachine.VERTICAL_PARAM);

            float smoothedHorizontal = Mathf.SmoothDamp(horizontalParam, inputManager.moveInput.x, ref currentHorizontal, 0.1f);
            float smoothedVertical = Mathf.SmoothDamp(verticalParam, inputManager.moveInput.y, ref currentVertical, 0.1f);
            
            playerManager.animator.SetFloat(stateMachine.HORIZONTAL_PARAM, smoothedHorizontal);
            playerManager.animator.SetFloat(stateMachine.VERTICAL_PARAM, smoothedVertical);
        }
        
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
        if (stateMachine.isLockedOn)
        {
            HandleLockOnRotation();
        }
        else
        {
            HandleNormalRotation();
        }           
        HandleNormalMovement();

    }

    public override void OnExitState()
    {
        stateMachine.StopCoroutine(PlayJumpEndAndWalk());
    }

    private void HandleNormalRotation()
    {
        float targetAngle = Mathf.Atan2(
            inputManager.moveInput.x,
            inputManager.moveInput.y
        ) * Mathf.Rad2Deg + playerManager.playerCam.transform.eulerAngles.y;
        
        stateManager.targetAngle = targetAngle;

        Quaternion targetRotation = Quaternion.Euler(0f, targetAngle, 0f);

        Quaternion newRotation = Quaternion.Slerp(
            playerManager.rb.rotation, 
            targetRotation,            
            playerManager.walkRotationSpeed * Time.fixedDeltaTime 
        );

        playerManager.rb.MoveRotation(newRotation);
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
            playerManager.walkRotationSpeed * Time.fixedDeltaTime 
        );

        playerManager.rb.MoveRotation(newRotation);
    }
    
    private void HandleNormalMovement()
    {
        Vector3 finalMoveDirection = stateManager.forward;
        Vector3 targetVelocity = finalMoveDirection * playerManager.walkSpeed;
        Vector3 currentVelocity = playerManager.rb.linearVelocity;

        Vector3 newVelocity = Vector3.Lerp(
            currentVelocity,
            targetVelocity,
            playerManager.moveLerpSpeed * Time.fixedDeltaTime // 보간 속도
        );

        playerManager.rb.linearVelocity = newVelocity;
    }
    
    private IEnumerator PlayJumpEndAndWalk()
    {
        stateMachine.PlayAnimation(stateMachine.JUMP_0_END);
        
        playerManager.rb.linearVelocity =
            new Vector3(playerManager.rb.linearVelocity.x * 0.4f, playerManager.rb.linearVelocity.z, playerManager.rb.linearVelocity.z * 0.4f);
        playerManager.moveLerpSpeed = 3f;
        
        float stopAnimDuration = 0f; // 실제 멈춤 애니메이션 길이에 맞춰 조절
        yield return new WaitForSeconds(stopAnimDuration);

        playerManager.moveLerpSpeed = 40f;
        
        stateMachine.PlayAnimation(stateMachine.WALK_START_F_0);
    }
}