using System.Collections;
using UnityEngine;

public class PlayerIdleState : BaseState
{
    private PlayerStateMachine stateMachine;
    private PlayerManager playerManager;
    private PlayerStateManager stateManager;
    private PlayerInputManager inputManager;

    private Coroutine stopCoroutine;
    
    public PlayerIdleState(PlayerStateMachine stateMachine)
    {
        this.stateMachine = stateMachine;
        playerManager = stateMachine.playerManager;
        stateManager = playerManager.StateManager;
        inputManager = playerManager.InputManager;
    }
    
    public override void OnEnterState()
    {
        stateMachine.playerManager.rb.linearVelocity = Vector3.zero;

        if (stateMachine.isLockedOn)
        {
            stateMachine.PlayAnimation(stateMachine.IDLE_COMBAT_ANIMATION);
        }
        else
        {
            if (stateMachine.previousState == stateMachine.walkState)
                stopCoroutine = stateMachine.StartCoroutine(PlayWalkStopAndIdle());
            else if (stateMachine.previousState == stateMachine.runState)
            {
                stopCoroutine = stateMachine.StartCoroutine(PlayRunStopAndIdle());
            }
            else if (stateMachine.previousState == stateMachine.fallState && stateMachine.shouldPlayJumpEnd)
            {
                stateMachine.shouldPlayJumpEnd = false;
                stopCoroutine = stateMachine.StartCoroutine(PlayJumpEndAndIdle());
            }
            else
            {
                stateMachine.PlayAnimation(stateMachine.IDLE_ANIMATION);
            }

            playerManager.rb.linearVelocity = Vector3.zero;
        }
    }
    
    public override void OnUpdateState()
    {
        playerManager.rb.linearVelocity = Vector3.zero;

        
        if (stateMachine.playerManager.InputManager.moveInput != Vector2.zero)
        {
            if (stateMachine.isRunning)
            {
                stateMachine.ChangeState(stateMachine.runState);
            }
            else
            {
                stateMachine.ChangeState(stateMachine.walkState);
            }

            return;
        }
    }
    
    public override void OnFixedUpdateState()
    {
        
    }
    
    public override void OnExitState()
    {
        if (stopCoroutine != null)
        {
            stateMachine.StopCoroutine(stopCoroutine);
        }
    }
    
    private IEnumerator PlayWalkStopAndIdle()
    {
        stateMachine.PlayAnimation(stateMachine.WALK_STOP_F_0);

        float stopAnimDuration = 0.7f; // 실제 멈춤 애니메이션 길이에 맞춰 조절
        yield return new WaitForSeconds(stopAnimDuration);

        stateMachine.PlayAnimation(stateMachine.IDLE_ANIMATION);
    }
    
    private IEnumerator PlayRunStopAndIdle()
    {
        stateMachine.PlayAnimation(stateMachine.RUN_STOP_F_0);

        float stopAnimDuration = 0.6f; // 실제 멈춤 애니메이션 길이에 맞춰 조절
        yield return new WaitForSeconds(stopAnimDuration);

        stateMachine.PlayAnimation(stateMachine.IDLE_ANIMATION);
    }
    
    private IEnumerator PlayJumpEndAndIdle()
    {
        stateMachine.PlayAnimation(stateMachine.JUMP_0_END);

        float stopAnimDuration = 0.35f; 
        yield return new WaitForSeconds(stopAnimDuration);

        stateMachine.PlayAnimation(stateMachine.IDLE_ANIMATION);
    }
}
