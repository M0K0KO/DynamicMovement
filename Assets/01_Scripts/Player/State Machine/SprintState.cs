public class SprintState : BaseState
{
    public SprintState(StateManager stateManager) : base(stateManager) {}
    public override void EnterState()
    {
        manager.desiredDissolveValue = 0f;
        
        if (manager.player.controller.isGrounded) manager.MovePlayer(manager.player.sprintSpeed);
        manager.player.animator.CrossFadeInFixedTime("Sprint", 0.1f);
    }

    public override void UpdateState()
    {
        if (manager.player.controller.isGrounded && 
            manager.player.animator.GetCurrentAnimatorStateInfo(0).IsName("Sprint"))
        {
            manager.MovePlayer(manager.player.sprintSpeed);
        }
    }

    public override void FixedUpdateState()
    {
        
    }
    
    public override void TransitionCheck()
    {
        base.TransitionCheck();
        
        if (!manager.CheckMoveInput())
        {
            manager.ChangeState(manager.equippedIdle);
        }
    }
    
    public override void ExitState()
    {
    }
}