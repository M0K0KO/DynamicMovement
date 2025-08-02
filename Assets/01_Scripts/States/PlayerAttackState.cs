using UnityEngine;

public class PlayerAttackState : BaseState
{
    private PlayerStateMachine stateMachine;
    private PlayerManager playerManager;
    private PlayerStateManager stateManager;
    private PlayerInputManager inputManager;
    private PlayerLocomotionManager locomotionManager;

    public PlayerAttackState(PlayerStateMachine stateMachine)
    {
        this.stateMachine = stateMachine;
        playerManager = stateMachine.playerManager;
        stateManager = playerManager.StateManager;
        inputManager = playerManager.InputManager;
        locomotionManager = playerManager.LocomotionManager;
    }

    private LocomotionParameters locomotionParams;
    
    public bool bufferedInput = false;
    public bool isComboWindowOpen = false;
    
    private int comboCount = 0;
    private int maxCombo = 4;

    public override void OnEnterState()
    {
        locomotionParams = new LocomotionParameters
        {
            PerformRotation = true,
            InstantRotation = false,
            RotationSpeed = playerManager.runRotationSpeed,
            PerformMovement = false,
        };
        
        playerManager.rb.linearVelocity = Vector3.zero;
        isComboWindowOpen = false;
        
        comboCount++;
        locomotionManager.PlayAttackAnimation();
    }

    public override void OnUpdateState()
    {
        if (isComboWindowOpen && bufferedInput)
        {
            if (comboCount < maxCombo)
            {
                bufferedInput = false;
                isComboWindowOpen = false;
                comboCount++;
                playerManager.animator.SetTrigger(locomotionManager.NEXT_COMBO_TRIGGER);
                
                return;
            }
        }
        
        if (CheckStateInfo())
        {
            stateMachine.ChangeState(stateMachine.idleState);
        }
    }

    public override void OnFixedUpdateState()
    {
        if (stateMachine.isLockedOn) locomotionManager.HandleMovement(locomotionParams);
    }

    public override void OnExitState()
    {
        playerManager.VFXManager.DisableAllVFX();
        
        comboCount = 0;
        bufferedInput = false;
        playerManager.animator.ResetTrigger(locomotionManager.NEXT_COMBO_TRIGGER);
        playerManager.CombatManager.DisableAllHitBox();
        playerManager.rb.linearVelocity = Vector3.zero;
    }

    private bool CheckStateInfo()
    {
        AnimatorStateInfo stateInfo = playerManager.animator.GetCurrentAnimatorStateInfo(0);
        if (stateInfo.IsTag("ComboAttack") && stateInfo.normalizedTime > 0.7f) return true;
        else return false;
    }
}
