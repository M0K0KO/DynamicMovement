using System.Collections;
using UnityEngine;

public class PlayerDashAttackState : BaseState
{
    private PlayerStateMachine stateMachine;
    private PlayerManager playerManager;
    private PlayerStateManager stateManager;
    private PlayerInputManager inputManager;
    private PlayerLocomotionManager locomotionManager;

    public PlayerDashAttackState(PlayerStateMachine stateMachine)
    {
        this.stateMachine = stateMachine;
        playerManager = stateMachine.playerManager;
        stateManager = playerManager.StateManager;
        inputManager = playerManager.InputManager;
        locomotionManager = playerManager.LocomotionManager;
    }

    private bool dashAttackEnd;
    
    public override void OnEnterState()
    {
        locomotionManager.PlayDashAttackAnimation();
        locomotionManager.HandleLockOnRotation(0f, true);
        stateMachine.StartCoroutine(DashAttack());
    }
    
    public override void OnUpdateState()
    {
        AnimatorStateInfo stateInfo = playerManager.animator.GetCurrentAnimatorStateInfo(0);
        if (dashAttackEnd && stateInfo.IsTag("DashAttack") && stateInfo.normalizedTime > 0.8f)
            stateMachine.ChangeState(stateMachine.idleState);
    }
    
    public override void OnFixedUpdateState()
    {
        
    }
    
    public override void OnExitState()
    {
        stateMachine.StopCoroutine(DashAttack());
        dashAttackEnd = false;
    }

    IEnumerator DashAttack()
    {
        while (true)
        {
            float targetDistance = Vector3.Distance(playerManager.transform.position,
                stateMachine.lockOnTarget.transform.position);
            if (targetDistance < 4f)
            {
                playerManager.rb.linearVelocity = Vector3.zero;
                dashAttackEnd = true;
                break;
            }
            Vector3 targetDirection = (stateMachine.lockOnTarget.transform.position - playerManager.transform.position)
                .normalized;
            playerManager.rb.linearVelocity = targetDirection * playerManager.dashAttackSpeed;

            yield return null;
        }
    }
}
