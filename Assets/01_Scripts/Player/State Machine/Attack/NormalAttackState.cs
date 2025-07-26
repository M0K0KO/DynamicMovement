using UnityEngine;

public class NormalAttackState : BaseState
{
    private AttackState attackState;
    
    private int comboCounter;

    public NormalAttackState(StateManager stateManager, AttackState attackState) : base(stateManager)
    {
        this.attackState = attackState;
    }

    public override void EnterState()
    {
        manager.desiredDissolveValue = 0f;
        
        attackState.isAttacking = true;
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
                attackState.bufferedNormalAttackInput = true;
                manager.player.playerInputManager.ClearNormalAttackInput();
            }
        }

        if (stateInfo.normalizedTime >= 0.5f)
        {
            if (comboCounter < 3 && attackState.bufferedNormalAttackInput)
            {
                comboCounter++;
                attackState.ClearBufferedNormalAttackInput();

                if (comboCounter == 2)
                {
                    manager.player.animator.CrossFadeInFixedTime("NormalAttack" + comboCounter, 0.128f, 0, 0.20f);
                }
                else if (comboCounter == 3)
                {
                    manager.player.animator.CrossFadeInFixedTime("NormalAttack" + comboCounter, 0.2f, 0, 0f);
                }
            }
        }
    }

    public override void ExitState()
    {
        attackState.isAttacking = false;
        attackState.ClearBufferedNormalAttackInput();
        comboCounter = 1; 
    }
    
    public override void FixedUpdateState() { }

    public override void TransitionCheck()
    {
    }
}