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

        if (stateMachine.previousState == stateMachine.walkState)
            stopCoroutine = stateMachine.StartCoroutine(PlayWalkStopAndIdle(stateMachine.isLockedOn));
        else if (stateMachine.previousState == stateMachine.runState)
        {
            stopCoroutine = stateMachine.StartCoroutine(PlayRunStopAndIdle(stateMachine.isLockedOn));
        }
        else if (stateMachine.previousState == stateMachine.fallState && stateMachine.shouldPlayJumpEnd)
        {
            stateMachine.shouldPlayJumpEnd = false;
            stopCoroutine = stateMachine.StartCoroutine(PlayJumpEndAndIdle(stateMachine.isLockedOn));
        }
        else if (stateMachine.previousState == stateMachine.chargeAttackState)
        {
            stateMachine.shouldPlayChargeAttackEnd = false;
            stopCoroutine = stateMachine.StartCoroutine(PlayChargeAttackEndAndIdle(stateMachine.isLockedOn));
        }
        else
        {
            if (stateMachine.isLockedOn) stateMachine.PlayAnimation(stateMachine.IDLE_COMBAT_ANIMATION);
            else stateMachine.PlayAnimation(stateMachine.IDLE_ANIMATION);
        }

        playerManager.rb.linearVelocity = Vector3.zero;
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
    
    private IEnumerator PlayWalkStopAndIdle(bool isLockedOn)
    {
        stateMachine.PlayAnimation(stateMachine.WALK_STOP_F_0);

        float stopAnimDuration = 0.7f; // 실제 멈춤 애니메이션 길이에 맞춰 조절
        yield return new WaitForSeconds(stopAnimDuration);

        if (isLockedOn) stateMachine.PlayAnimation(stateMachine.IDLE_COMBAT_ANIMATION);
        else stateMachine.PlayAnimation(stateMachine.IDLE_ANIMATION);
    }
    
    private IEnumerator PlayRunStopAndIdle(bool isLockedOn)
    {
        stateMachine.PlayAnimation(stateMachine.RUN_STOP_F_0);

        float stopAnimDuration = 0.6f; // 실제 멈춤 애니메이션 길이에 맞춰 조절
        yield return new WaitForSeconds(stopAnimDuration);

        if (isLockedOn) stateMachine.PlayAnimation(stateMachine.IDLE_COMBAT_ANIMATION);
        else stateMachine.PlayAnimation(stateMachine.IDLE_ANIMATION);
    }
    
    private IEnumerator PlayJumpEndAndIdle(bool isLockedOn)
    {
        stateMachine.PlayAnimation(stateMachine.JUMP_0_END, 0.1f);

        float stopAnimDuration = 0f; 
        yield return new WaitForSeconds(stopAnimDuration);

        if (isLockedOn) stateMachine.PlayAnimation(stateMachine.IDLE_COMBAT_ANIMATION, 0.1f);
        else stateMachine.PlayAnimation(stateMachine.IDLE_ANIMATION, 0.1f);
    }

    private IEnumerator PlayChargeAttackEndAndIdle(bool isLockedOn)
    {
        stateMachine.PlayAnimation(stateMachine.PLAYER_CHARGE_ATTACK_END);

        float stopAnimDuration = isLockedOn ? 0.53f * 0.6f : 0.2f; 
        yield return new WaitForSeconds(stopAnimDuration);

        if (isLockedOn) stateMachine.PlayAnimation(stateMachine.IDLE_COMBAT_ANIMATION, 0.1f);
        else stateMachine.PlayAnimation(stateMachine.IDLE_ANIMATION, 0.2f);
    }
}
