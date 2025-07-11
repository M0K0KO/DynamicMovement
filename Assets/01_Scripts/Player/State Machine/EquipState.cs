using UnityEngine;

public class EquipState : BaseState
{
    public EquipState(StateManager manager) : base(manager)
    {
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
            manager.ChangeState(manager.equippedIdle);
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