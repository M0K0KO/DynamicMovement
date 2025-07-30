using UnityEngine;

public class PlayerChargeAttackState : BaseState
{
    private PlayerStateMachine stateMachine;
    private PlayerManager playerManager;
    private PlayerStateManager stateManager;
    private PlayerInputManager inputManager;
    
    public PlayerChargeAttackState(PlayerStateMachine stateMachine)
    {
        this.stateMachine = stateMachine;
        playerManager = stateMachine.playerManager;
        stateManager = playerManager.StateManager;
        inputManager = playerManager.InputManager;
    }

    private float chargeAttackTime = 0f;
    private float chargeAttackTimeThreshold = 3f;
    
    public override void OnEnterState()
    {
        
        stateMachine.PlayAnimation(stateMachine.PLAYER_CHARGE_ATTACK_START);
    }

    public override void OnUpdateState()
    {
        if (chargeAttackTime < chargeAttackTimeThreshold) chargeAttackTime += Time.deltaTime;
        else
        {
            stateMachine.shouldPlayJumpEnd = true;
            stateMachine.ChangeState(stateMachine.idleState);
        }
    }

    public override void OnFixedUpdateState()
    {
        if (stateMachine.isLockedOn) HandleLockOnRotation();
    }

    public override void OnExitState()
    {
        chargeAttackTime = 0f;
    }
    
    private void HandleLockOnRotation()
    {
        Vector3 directionToTarget = stateMachine.lockOnTarget.transform.position - playerManager.transform.position;
        directionToTarget.y = 0;
        directionToTarget.Normalize(); 
        
        Quaternion targetRotation = Quaternion.LookRotation(directionToTarget);

        Quaternion newRotation = Quaternion.Slerp(
            playerManager.rb.rotation,
            targetRotation,
            playerManager.runRotationSpeed * Time.fixedDeltaTime 
        );

        playerManager.rb.MoveRotation(newRotation);
    }
}
