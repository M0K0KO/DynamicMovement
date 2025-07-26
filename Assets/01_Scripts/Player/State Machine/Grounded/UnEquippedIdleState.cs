using UnityEngine;

public class UnEquippedIdleState : BaseState
{
    private GroundedState groundedState;

    public UnEquippedIdleState(StateManager stateManager, GroundedState groundedState) : base(stateManager)
    {
        this.groundedState = groundedState;
    }
    public override void EnterState()
    {
        manager.player.animator.CrossFadeInFixedTime("UnEquippedIdle", 0.2f);
    }

    public override void UpdateState()
    {
        if (manager.CheckNearbyEnemy())
        {
            groundedState.ChangeSubState(groundedState.equipState);
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
