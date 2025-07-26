public class SprintState : BaseState
{
    private GroundedState groundedState;

    public SprintState(StateManager stateManager, GroundedState groundedState) : base(stateManager)
    {
        this.groundedState = groundedState;
    }
    public override void EnterState()
    {
        manager.desiredDissolveValue = 0f;
        
        if (manager.player.playerPhysics.IsGrounded()) 
            manager.player.playerPhysics.MovePlayer(manager.player.sprintSpeed);
        manager.player.animator.CrossFadeInFixedTime("Sprint", 0.1f);
    }

    public override void UpdateState()
    {
        if (manager.player.playerPhysics.IsGrounded() && 
            manager.player.animator.GetCurrentAnimatorStateInfo(0).IsName("Sprint"))
        {
            manager.player.playerPhysics.MovePlayer(manager.player.sprintSpeed);
        }
    }

    public override void FixedUpdateState()
    {
        
    }
    
    public override void TransitionCheck()
    {
        if (!manager.CheckMoveInput())
        {
            groundedState.ChangeSubState(groundedState.equippedIdleState);
        }
    }
    
    public override void ExitState()
    {
    }
}