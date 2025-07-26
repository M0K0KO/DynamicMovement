using UnityEngine;

public class UnEquipState : BaseState
{
    private GroundedState groundedState;

    public UnEquipState(StateManager manager, GroundedState groundedState) : base(manager)
    {
        this.groundedState = groundedState;
    }

    public override void EnterState()
    {
        manager.desiredDissolveValue = 1f;
        manager.player.animator.CrossFadeInFixedTime("UnEquip", 0.2f);
    }

    public override void UpdateState()
    {
        AnimatorStateInfo stateInfo = manager.player.animator.GetCurrentAnimatorStateInfo(0);

        if (stateInfo.IsName("UnEquip") && stateInfo.normalizedTime >= 0.8f)
        {
            groundedState.ChangeSubState(groundedState.unEquippedIdleState);
        }
    }

    public override void FixedUpdateState()
    {
    }

    public override void TransitionCheck()
    {
        if (manager.CheckMoveInput())
        {
            groundedState.ChangeSubState(groundedState.sprintState);
        }
    }

    public override void ExitState()    
    {
        
    }
}