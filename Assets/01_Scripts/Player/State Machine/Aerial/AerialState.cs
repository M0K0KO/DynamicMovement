using UnityEngine;

public class AerialState : BaseState
{
    public BaseState currentSubState;

    public FallState fallState;
    public JumpState jumpState;

    public AerialState(StateManager manager) : base(manager)
    {
        fallState = new FallState(manager, this);
        jumpState = new JumpState(manager, this);
    }
    
    public bool jumpTrigger;
    
    public override void EnterState()
    {
        if (jumpTrigger)
        {
            jumpTrigger = false;
            ChangeSubState(jumpState);
        }
        else
        {
            ChangeSubState(fallState);
        }
    }

    public override void UpdateState()
    {
        currentSubState.UpdateState();
    }

    public override void FixedUpdateState()
    {
        currentSubState.FixedUpdateState();
    }

    public override void TransitionCheck()
    {
        if (manager.player.playerPhysics.IsGrounded())
        {
            manager.ChangeState(manager.groundedState);
            return;
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
