using UnityEngine;

public class PlayerChargeAttackEndState : BaseState
{
    private PlayerStateMachine stateMachine;
    private PlayerManager playerManager;
    private PlayerStateManager stateManager;
    private PlayerInputManager inputManager;
    private PlayerLocomotionManager locomotionManager;
    
    public PlayerChargeAttackEndState(PlayerStateMachine stateMachine)
    {
        this.stateMachine = stateMachine;
        playerManager = stateMachine.playerManager;
        stateManager = playerManager.StateManager;
        inputManager = playerManager.InputManager;
        locomotionManager = playerManager.LocomotionManager;
    }

    
    
    public override void OnEnterState()
    {
        locomotionManager.PlayChargeAttackEndAnimation();
    }
    
    public override void OnUpdateState()
    {
        AnimatorStateInfo stateInfo = playerManager.animator.GetCurrentAnimatorStateInfo(0);

        if (stateInfo.IsTag("ChargeAttackEnd") && stateInfo.normalizedTime >= 0.5f)
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
        playerManager.VFXManager.DisableAllVFX();
    }
}
