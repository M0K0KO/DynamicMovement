using UnityEngine;

public class PlayerAttackState : BaseState
{
    private PlayerStateMachine stateMachine;
    private PlayerManager playerManager;
    private PlayerStateManager stateManager;
    private PlayerInputManager inputManager;

    public PlayerAttackState(PlayerStateMachine stateMachine)
    {
        this.stateMachine = stateMachine;
        playerManager = stateMachine.playerManager;
        stateManager = playerManager.StateManager;
        inputManager = playerManager.InputManager;
    }

    public bool bufferedInput = false;
    public bool isComboWindowOpen = false;
    
    private int comboCount = 0;
    private int maxCombo = 4;

    public override void OnEnterState()
    {
        playerManager.rb.linearVelocity = Vector3.zero;
        isComboWindowOpen = false;
        
        comboCount++;
        stateMachine.PlayAnimation(stateMachine.PLAYER_COMBO_01_1);
    }

    public override void OnUpdateState()
    {
        if (stateMachine.isLockedOn) HandleLockOnRotation();
        
        if (isComboWindowOpen && bufferedInput)
        {
            if (comboCount < maxCombo)
            {
                bufferedInput = false;
                isComboWindowOpen = false;
                comboCount++;
                playerManager.animator.SetTrigger(stateMachine.NEXT_COMBO_TRIGGER);
                
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
        
    }

    public override void OnExitState()
    {
        comboCount = 0;
        bufferedInput = false;
        playerManager.animator.ResetTrigger(stateMachine.NEXT_COMBO_TRIGGER);
        playerManager.rb.linearVelocity = Vector3.zero;
    }

    private bool CheckStateInfo()
    {
        AnimatorStateInfo stateInfo = playerManager.animator.GetCurrentAnimatorStateInfo(0);
        if (stateInfo.IsTag("ComboAttack") && stateInfo.normalizedTime > 0.7f) return true;
        else return false;
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
            playerManager.walkRotationSpeed * Time.fixedDeltaTime 
        );

        playerManager.rb.MoveRotation(newRotation);
    }
}
