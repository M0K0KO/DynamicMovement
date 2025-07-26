public class FallState : BaseState
{
    private AerialState aerialState;

    public FallState(StateManager manager, AerialState aerialState) : base(manager)
    {
        this.aerialState = aerialState;
    }
    
    
    public override void EnterState()
    {
        manager.player.animator.CrossFadeInFixedTime("Fall", 0.2f);
    }

    public override void UpdateState()
    {
        manager.player.playerPhysics.MovePlayer(manager.player.aerialAdditiveSpeed);
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
