using UnityEngine;

public class NormalAttackState : BaseState
{
    private int comboCounter; 

    public NormalAttackState(StateManager stateManager) : base(stateManager) {}

    public override void EnterState()
    {
        manager.desiredDissolveValue = 0f;
        
        manager.isAttacking = true;
        comboCounter = 1; 
        manager.player.animator.CrossFadeInFixedTime("NormalAttack" + comboCounter, 0.1f);
    }

    public override void UpdateState()
    {
        AnimatorStateInfo stateInfo = manager.player.animator.GetCurrentAnimatorStateInfo(0);

        if (stateInfo.normalizedTime >= 0f && stateInfo.normalizedTime < 0.5f)
        {
            if (manager.player.playerInputManager.normalAttackInput)
            {
                manager.bufferedNormalAttackInput = true;
                manager.player.playerInputManager.ClearNormalAttackInput();
            }
        }

        if (stateInfo.normalizedTime >= 0.5f)
        {
            if (comboCounter < 3 && manager.bufferedNormalAttackInput)
            {
                comboCounter++;
                manager.ClearBufferedNormalAttackInput();

                if (comboCounter == 2)
                {
                    manager.player.animator.CrossFadeInFixedTime("NormalAttack" + comboCounter, 0.128f, 0, 0.20f);
                }
                else if (comboCounter == 3)
                {
                    manager.player.animator.CrossFadeInFixedTime("NormalAttack" + comboCounter, 0.2f, 0, 0f);
                }

                return;
            }
        }

        if (stateInfo.normalizedTime >= 0.8f)
        {
            if (manager.CheckMoveInput())
            {
                manager.ChangeState(manager.sprint);
            }
            else
            {
                manager.ChangeState(manager.equippedIdle);
            }
        }
    }

    public override void ExitState()
    {
        manager.isAttacking = false;
        manager.ClearBufferedNormalAttackInput();
        comboCounter = 1; 
    }
    
    public override void FixedUpdateState() { }

    public override void TransitionCheck()
    {
        base.TransitionCheck();
    }
}