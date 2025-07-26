using System;
using System.Xml.Schema;
using UnityEngine;
using UnityEngine.UIElements;
using Quaternion = UnityEngine.Quaternion;
using Vector3 = UnityEngine.Vector3;

public class StateManager : MonoBehaviour
{
    public PlayerManager player;

    public GroundedState groundedState;
    public AerialState aerialState;
    public AttackState attackState;

    public LayerMask enemyLayer;

    public BaseState currentState;
    private Collider[] nearByEnemies;

    private Vector3 playerVelocity;

    public event Action OnDashStarted;
    public event Action OnDashEnded;

    public void TriggerOnDashStartedEvent()
    {
        OnDashStarted?.Invoke();
    }
    public void TriggerOnDashEndedEvent()
    {
        OnDashEnded?.Invoke();
    }
    
    private bool wasEnemyNearby = false;
    public float desiredDissolveValue = -1f;
    public float currentTargetDissolveValue = -1f;

    private void Awake()
    {
        player = GetComponent<PlayerManager>();

        nearByEnemies = new Collider[10];
    }
    private void Start()
    {
        InitializeStateManager();
    }
    private void Update()
    {
        currentState.UpdateState();
        currentState.TransitionCheck();

        if (!Mathf.Approximately(desiredDissolveValue, currentTargetDissolveValue))
        {
            player.vfxManager.AdjustWeaponDissolveVFX(desiredDissolveValue, (currentState == attackState));
            currentTargetDissolveValue = desiredDissolveValue;
        }
        
        DebugExtension.ColorLog($"주변 적 스캔 : {CheckNearbyEnemy()}", "red");
        DebugExtension.ColorLog($"{currentState}", "cyan");
        DebugExtension.ColorLog($"isGrounded : {player.playerPhysics.IsGrounded()}", "green");
    }
    private void FixedUpdate()
    {
        currentState.FixedUpdateState();
    }
    private void InitializeStateManager()
    {
        groundedState = new GroundedState(this);
        aerialState = new AerialState(this);
        attackState = new AttackState(this);
        
        currentState = groundedState;
        currentState.EnterState();
    }
    public void ChangeState(BaseState newState)
    {

        currentState.ExitState();
        currentState = newState;
        currentState.EnterState();
    }

    


    public bool CheckNearbyEnemy()
    {
        int enemyCount = Physics.OverlapSphereNonAlloc(
            player.transform.position,
            player.detectionRadius,
            nearByEnemies,
            enemyLayer
        );

        return enemyCount > 0;
    }

    
    public bool CheckMoveInput() => player.playerInputManager.moveInputDirection != Vector3.zero;
    public bool CheckDashInput() => player.playerInputManager.dashInput;
    public bool CheckNormalAttackInput() => player.playerInputManager.normalAttackInput;
    


    
    //------------------------------------------------------------------------------------------------------------------------

    
    
}