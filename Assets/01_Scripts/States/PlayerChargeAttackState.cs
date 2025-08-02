using UnityEngine;

public class PlayerChargeAttackState : BaseState
{
    private PlayerStateMachine stateMachine;
    private PlayerManager playerManager;
    private PlayerStateManager stateManager;
    private PlayerInputManager inputManager;
    private PlayerLocomotionManager locomotionManager;
    
    public PlayerChargeAttackState(PlayerStateMachine stateMachine)
    {
        this.stateMachine = stateMachine;
        playerManager = stateMachine.playerManager;
        stateManager = playerManager.StateManager;
        inputManager = playerManager.InputManager;
        locomotionManager = playerManager.LocomotionManager;
    }

    private LocomotionParameters locomotionParams;
    
    private float chargeAttackTime = 0f;
    private float chargeAttackTimeThreshold = 2f;

    private bool disableVFX = true;
    
    public override void OnEnterState()
    {
        playerManager.rb.linearVelocity = Vector3.zero;
        
        locomotionParams = new LocomotionParameters
        {
            PerformMovement = false,
            
            PerformRotation = true,
            RotationSpeed = playerManager.walkRotationSpeed,
            InstantRotation = false,
        };
        
        locomotionManager.PlayChargeAttackAnimation();
    }

    public override void OnUpdateState()
    {
        if (chargeAttackTime < chargeAttackTimeThreshold) chargeAttackTime += Time.deltaTime;
        else
        {
            disableVFX = false;
            stateMachine.ChangeState(stateMachine.chargeAttackEndState);
        }
    }

    public override void OnFixedUpdateState()
    {
        locomotionManager.HandleMovement(locomotionParams);
    }

    public override void OnExitState()
    {
        if (disableVFX == true)  playerManager.VFXManager.DisableAllVFX();
        
        playerManager.CombatManager.DisableAllHitBox();
        chargeAttackTime = 0f;
    }
}
