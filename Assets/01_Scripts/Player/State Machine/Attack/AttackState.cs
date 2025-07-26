using UnityEngine;

public class AttackState : BaseState
{
    public BaseState currentSubState;

    public NormalAttackState normalAttackState;

    public AttackState(StateManager manager) : base(manager)
    {
        normalAttackState = new NormalAttackState(manager, this);
    }

    public bool normalAttackTrigger = false;
    
    public bool isAttacking = false;
    public bool bufferedNormalAttackInput = false;

    public override void EnterState()
    {
        if (normalAttackTrigger)
        {
            normalAttackTrigger = false;
            ChangeSubState(normalAttackState);
        }
    }

    public override void UpdateState()
    {
        currentSubState.UpdateState();
    }

    public override void FixedUpdateState()
    {
        currentSubState.FixedUpdateState();
    }

    public override void TransitionCheck()
    {
        if (manager.CheckDashInput() && !manager.groundedState.isDashing && manager.player.playerPhysics.IsGrounded())
        {
            manager.groundedState.dashTrigger = true;
            manager.ChangeState(manager.groundedState);
            return;
        }
        
        AnimatorStateInfo stateInfo = manager.player.animator.GetCurrentAnimatorStateInfo(0);
        
        if (stateInfo.normalizedTime >= 0.8f)
        {
            if (manager.player.playerPhysics.IsGrounded())
            {
                manager.ChangeState(manager.groundedState);
            }
            else
            {
                manager.ChangeState(manager.aerialState);
            }
        }
        
    }

    public override void ExitState()
    {
        
    }
    
    public void ClearBufferedNormalAttackInput()
    {
        bufferedNormalAttackInput = false;
    }
    
    public void ChangeSubState(BaseState newSubState)
    {
        currentSubState?.ExitState();
        currentSubState = newSubState;
        currentSubState.EnterState();
    }
}
