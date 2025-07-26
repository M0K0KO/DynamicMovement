using UnityEngine;

public class EquipState : BaseState
{
    private GroundedState groundedState;
    
    public EquipState(StateManager manager, GroundedState groundedState) : base(manager)
    {
        this.groundedState = groundedState;
    }

    public override void EnterState()
    {
        manager.desiredDissolveValue = 0f;
        manager.player.animator.CrossFadeInFixedTime("Equip", 0.2f);
    }

    public override void UpdateState()
    {
        AnimatorStateInfo stateInfo = manager.player.animator.GetCurrentAnimatorStateInfo(0);

        if (stateInfo.IsName("Equip") && stateInfo.normalizedTime >= 0.8f)
        {
            groundedState.ChangeSubState(groundedState.equippedIdleState);
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