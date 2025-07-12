using UnityEngine;

public class BaseState
{
    protected StateManager manager;

    public BaseState(StateManager manager)
    {
        this.manager = manager;
    }
    
    public virtual void EnterState() {}
    public virtual void UpdateState() {}
    public virtual void FixedUpdateState() {}

    public virtual void TransitionCheck()
    {
        if (manager.CheckDashInput() && !manager.isDashing && manager.player.playerPhysics.IsGrounded())
        {
            manager.ChangeState(manager.dash);
            return;
        }

        if (manager.CheckNormalAttackInput())
        {
            if (manager.currentState != manager.normalAttack && !manager.isDashing) 
            {
                manager.ChangeState(manager.normalAttack);
                manager.player.playerInputManager.ClearNormalAttackInput();
            }
        }
    }
    public virtual void ExitState() {}
}
