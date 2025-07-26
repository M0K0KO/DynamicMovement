public class JumpState : BaseState
{
    private AerialState aerialState;

    public JumpState(StateManager manager, AerialState aerialState) : base(manager)
    {
        this.aerialState = aerialState;
    }
    
    
    public override void EnterState()
    {
    }

    public override void UpdateState()
    {
    }

    public override void FixedUpdateState()
    {
    }

    public override void TransitionCheck()
    {
    }

    public override void ExitState()
    {
    }
}