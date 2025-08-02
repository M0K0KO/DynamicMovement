using UnityEngine;

public class PlayerLandState : BaseState
{
    private PlayerStateMachine stateMachine;
    private PlayerManager playerManager;
    private PlayerStateManager stateManager;
    private PlayerInputManager inputManager;
    private PlayerLocomotionManager locomotionManager;
    
    public PlayerLandState(PlayerStateMachine stateMachine)
    {
        this.stateMachine = stateMachine;
        playerManager = stateMachine.playerManager;
        stateManager = playerManager.StateManager;
        inputManager = playerManager.InputManager;
        locomotionManager = playerManager.LocomotionManager;
    }

    public override void OnEnterState()
    {
        playerManager.rb.linearVelocity = new Vector3(
            playerManager.rb.linearVelocity.x * 0.4f, 
            playerManager.rb.linearVelocity.y,
            playerManager.rb.linearVelocity.z * 0.4f);
        locomotionManager.PlayJumpEndAnimation();
    }

    public override void OnUpdateState()
    {
        AnimatorStateInfo stateInfo = playerManager.animator.GetCurrentAnimatorStateInfo(0);

        if (stateInfo.IsTag("JumpEnd") && stateInfo.normalizedTime >= 0.3f)
        {
            if (inputManager.moveInput == Vector2.zero)
            {
                stateMachine.ChangeState(stateMachine.idleState);
            }
            else
            {
                if (stateMachine.isRunning)
                {
                    stateMachine.ChangeState(stateMachine.runState);
                }
                else
                {
                    stateMachine.ChangeState(stateMachine.walkState);
                }
            }
        }
    }

    public override void OnFixedUpdateState()
    {
    }

    public override void OnExitState()
    {
    }
}
