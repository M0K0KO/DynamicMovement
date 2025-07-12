using System;
using System.Xml.Schema;
using UnityEngine;
using UnityEngine.UIElements;
using Quaternion = UnityEngine.Quaternion;
using Vector3 = UnityEngine.Vector3;

public class StateManager : MonoBehaviour
{
    public PlayerManager player;

    public UnEquipState unEquip;
    public UnEquippedIdleState unEquippedIdle;
    public EquipState equip;
    public EquippedIdleState equippedIdle;
    public SprintState sprint;
    public DashState dash;
    public NormalAttackState normalAttack;

    public Camera playerCam;
    public LayerMask enemyLayer;

    public BaseState currentState;
    private Collider[] nearByEnemies;

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
    
    public bool isDashing = false;
    
    public bool isAttacking = false;
    public bool bufferedNormalAttackInput = false;

    private bool wasEnemyNearby = false;

    public float desiredDissolveValue = -1f;
    public float currentTargetDissolveValue = -1f;

    private void Awake()
    {
        player = GetComponent<PlayerManager>();
        playerCam = Camera.main;

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
            player.vfxManager.AdjustWeaponDissolveVFX(desiredDissolveValue, (currentState == normalAttack));
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
        unEquip = new UnEquipState(this);
        unEquippedIdle = new UnEquippedIdleState(this);
        equip = new EquipState(this);
        equippedIdle = new EquippedIdleState(this);
        sprint = new SprintState(this);
        dash = new DashState(this);
        
        normalAttack = new NormalAttackState(this);

        currentState = unEquippedIdle;
        currentState.EnterState();
    }
    public void ChangeState(BaseState newState)
    {

        currentState.ExitState();
        currentState = newState;
        currentState.EnterState();
    }

    
    public void MovePlayer(float speed)
    {
        
        Vector3 camForwardVector = new Vector3(playerCam.transform.forward.x, 0, playerCam.transform.forward.z).normalized;
        Vector3 camRightVector = new Vector3(playerCam.transform.right.x, 0, playerCam.transform.right.z).normalized;
        Vector3 moveDirection = camForwardVector * player.playerInputManager.moveInputDirection.z + camRightVector *player.playerInputManager.moveInputDirection.x;

        Quaternion targetRotation = Quaternion.LookRotation(transform.forward);

        if(moveDirection != Vector3.zero)
        {
            targetRotation = Quaternion.LookRotation(moveDirection);
        }
        
        player.transform.rotation = Quaternion.Lerp(player.transform.rotation, targetRotation, player.rotationSpeed * Time.deltaTime);

        Vector3 playerMovement = moveDirection * (speed * Time.deltaTime);
        player.controller.Move(playerMovement);
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
    public void ClearBufferedNormalAttackInput()
    {
        bufferedNormalAttackInput = false;
    }
    
    
    //---------------------------------------------
    
}