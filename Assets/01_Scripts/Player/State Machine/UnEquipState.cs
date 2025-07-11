using UnityEngine;

public class UnEquipState : BaseState
{
    public UnEquipState(StateManager manager) : base(manager) { }

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
            manager.ChangeState(manager.unEquippedIdle);
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