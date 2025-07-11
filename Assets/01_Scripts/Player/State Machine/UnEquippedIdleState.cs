using UnityEngine;

public class UnEquippedIdleState : BaseState
{
    public UnEquippedIdleState(StateManager stateManager) : base(stateManager) {}
    public override void EnterState()
    {
        manager.player.animator.CrossFadeInFixedTime("UnEquippedIdle", 0.2f);
    }

    public override void UpdateState()
    {
        if (manager.CheckNearbyEnemy())
        {
            manager.ChangeState(manager.equip);
        }
    }

    public override void FixedUpdateState()
    {
    }
    
    public override void TransitionCheck()
    {
        base.TransitionCheck();
        
        if (manager.CheckMoveInput())
        {
            manager.ChangeState(manager.sprint);
        }
    }
    
    public override void ExitState()
    {
    }
}
