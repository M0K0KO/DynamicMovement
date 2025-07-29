using System;
using System.Collections;
using Unity.VisualScripting;
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

    public override void OnEnterState()
    {
        if (stateMachine.previousState == stateMachine.fallState && stateMachine.shouldPlayJumpEnd)
        {
            stateMachine.shouldPlayJumpEnd = false;
            coroutine = stateMachine.StartCoroutine(PlayJumpEndAndWalk());
        }
        else
        {
            stateMachine.PlayAnimation(stateMachine.WALK_LOOP_F_0, 0.3f);
        }

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
        HandleRotation();
        HandleMovement();
    }

    public override void OnExitState()
    {
        stateMachine.StopCoroutine(PlayJumpEndAndWalk());
    }

    private void HandleRotation()
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

    private void HandleMovement()
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
        
        float stopAnimDuration = 0.3f; // 실제 멈춤 애니메이션 길이에 맞춰 조절
        yield return new WaitForSeconds(stopAnimDuration);

        playerManager.moveLerpSpeed = 40f;
        
        stateMachine.PlayAnimation(stateMachine.WALK_START_F_0, 0.3f);
    }
}