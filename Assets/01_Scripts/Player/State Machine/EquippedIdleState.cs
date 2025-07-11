using UnityEngine;

public class EquippedIdleState : BaseState
{
    private float unEquipTimer;
    private readonly float unEquipThreshold = 5f;
    
    public EquippedIdleState(StateManager stateManager) : base(stateManager) {}

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
                manager.ChangeState(manager.unEquip);
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
        base.TransitionCheck();
        
        if (manager.CheckMoveInput())
        {
            manager.ChangeState(manager.sprint);
        }
    }
    
    public override void ExitState()
    {
        unEquipTimer = 0f;
    }
}