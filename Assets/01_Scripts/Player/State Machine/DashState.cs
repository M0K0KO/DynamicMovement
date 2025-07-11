using UnityEngine;

public class DashState : BaseState
{
    public DashState(StateManager stateManager) : base(stateManager) {}

    private float dashTimer;
    private Vector3 dashVelocity;
    
    public override void EnterState()
    {
        StartDash();
        manager.player.animator.CrossFadeInFixedTime("Dash_Front", 0.1f);
    }

    public override void UpdateState()
    {
        HandleDash();
    }

    public override void FixedUpdateState()
    {
        
    }
    
    public override void TransitionCheck()
    {
        base.TransitionCheck();
    }
    
    public override void ExitState()
    {
        manager.player.playerInputManager.ClearDashInput();
    }

    #region DashLogic
    void StartDash()
    {
        manager.isDashing = true;
        dashTimer = 0f;
        manager.TriggerOnDashStartedEvent();
        manager.player.dashTrail.ActivateForDuration(manager.player.dashDuration - 0.15f);
        
        Vector3 camForward = new Vector3(manager.playerCam.transform.forward.x, 0, manager.playerCam.transform.forward.z).normalized;
        Vector3 camRight = new Vector3(manager.playerCam.transform.right.x, 0, manager.playerCam.transform.right.z).normalized;
        Vector3 moveDirection = camForward * manager.player.playerInputManager.moveInputDirection.z + camRight * manager.player.playerInputManager.moveInputDirection.x;

        if (moveDirection != Vector3.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(moveDirection);
            manager.player.transform.rotation = targetRotation; 
        }
            
        dashVelocity = manager.player.transform.forward * manager.player.dashForce;
    }

    void HandleDash()
    {
        dashTimer += Time.deltaTime;
        if (dashTimer >= manager.player.dashDuration)
        {
            EndDash();
            return;
        }
        
        float normalizedTime = dashTimer / manager.player.dashDuration;
        float speedMultiplier = (1f - normalizedTime); 
        
        Vector3 movement = dashVelocity * (speedMultiplier * Time.deltaTime);

        manager.player.controller.Move(movement);
    }

    void EndDash()
    {
        manager.isDashing = false;
        manager.TriggerOnDashEndedEvent();
        
        if (manager.CheckMoveInput())
        {
            manager.ChangeState(manager.sprint);
        }
        else
        {
            manager.ChangeState(manager.CheckNearbyEnemy() ? manager.equippedIdle : manager.unEquippedIdle);
        }
    }
    #endregion
}