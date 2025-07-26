using UnityEngine;

public class EquippedIdleState : BaseState
{
    private GroundedState groundedState;

    public EquippedIdleState(StateManager stateManager, GroundedState groundedState) : base(stateManager)
    {
        this.groundedState = groundedState;
    }

    private float unEquipTimer;
    private readonly float unEquipThreshold = 5f;
    
    public override void EnterState()
    {
        unEquipTimer = 0f;
        manager.player.animator.CrossFadeInFixedTime("EquippedIdle", 0.2f);
    }

    public override void UpdateState()
    {
        AnimatorStateInfo stateInfo = manager.player.animator.GetCurrentAnimatorStateInfo(0);
        
        bool isEnemyNearby = manager.CheckNearbyEnemy();
        
        if (!isEnemyNearby)
        {
            if (stateInfo.IsName("EquippedIdle"))
                unEquipTimer += Time.deltaTime;
            
            if (unEquipTimer >= unEquipThreshold)
                groundedState.ChangeSubState(groundedState.unEquipState);
        }
        else
        {
            unEquipTimer = 0f;
        }
    }

    public override void FixedUpdateState()
    {
        
    }

    public override void TransitionCheck()
    {
        
        if (manager.CheckMoveInput())
        {
            groundedState.ChangeSubState(groundedState.sprintState);
        }
    }
    
    public override void ExitState()
    {
        unEquipTimer = 0f;
    }
}