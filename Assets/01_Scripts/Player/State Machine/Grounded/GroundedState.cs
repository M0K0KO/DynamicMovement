using System.Data;
using UnityEngine;

public class GroundedState : BaseState
{
    public BaseState currentSubState;

    public EquipState equipState;
    public EquippedIdleState equippedIdleState;
    public UnEquipState unEquipState;
    public UnEquippedIdleState unEquippedIdleState;
    public SprintState sprintState;
    public DashState dashState;
    
    public GroundedState(StateManager manager) : base(manager)
    {
        equipState = new EquipState(manager, this);
        equippedIdleState = new EquippedIdleState(manager, this);
        unEquipState = new UnEquipState(manager, this);
        unEquippedIdleState = new UnEquippedIdleState(manager, this);
        sprintState = new SprintState(manager, this);
        dashState = new DashState(manager, this);
    }

    public bool dashTrigger = false;
    
    public bool isDashing = false;
    
    
    public override void EnterState()
    {
        if (dashTrigger)
        {
            dashTrigger = false;
            ChangeSubState(dashState);
            return;
        }
        
        if (manager.player.equipped)
            ChangeSubState(equippedIdleState);
        else
            ChangeSubState(unEquippedIdleState);
    }

    public override void UpdateState()
    {
        currentSubState.UpdateState();
        
        if (!manager.player.playerPhysics.IsGrounded())
        {
            manager.ChangeState(manager.aerialState);
        }
    }

    public override void FixedUpdateState()
    {
        currentSubState.FixedUpdateState();
    }

    public override void TransitionCheck()
    {
        if (manager.CheckDashInput() && !isDashing && manager.player.playerPhysics.IsGrounded())
        {
            ChangeSubState(dashState);
            return;
        }
        
        if (manager.CheckNormalAttackInput())
        {
            if (!isDashing)
            {
                manager.attackState.normalAttackTrigger = true;
                manager.ChangeState(manager.attackState);
                manager.player.playerInputManager.ClearNormalAttackInput();
            }
        }
        
        currentSubState.TransitionCheck();
    }

    public override void ExitState()
    {

    }

    public void ChangeSubState(BaseState newSubState)
    {
        currentSubState?.ExitState();
        currentSubState = newSubState;
        currentSubState.EnterState();
    }
}
