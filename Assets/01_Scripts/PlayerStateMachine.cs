using System;
using System.Collections;
using Moko;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Serialization;

public class PlayerStateMachine : MonoBehaviour
{
    public PlayerManager playerManager { get; private set; }
    
    public PlayerIdleState idleState { get; private set; }
    public PlayerWalkState walkState { get; private set; }
    public PlayerSlideState slideState { get; private set; }
    public PlayerRunState runState { get; private set; }
    public PlayerFallState fallState { get; private set; }
    public PlayerJumpState jumpState { get; private set; }
    public PlayerDashState dashState { get; private set; }
    public PlayerAttackState attackState { get; private set; }
    public PlayerChargeAttackState chargeAttackState { get; private set; }


    public bool isRunning;
    public bool shouldPlayJumpEnd;
    public bool shouldPlayChargeAttackEnd;

    private float fallTime = 0;
    public float fallThreshold = 0.3f;
    public Vector3 storedVelocityBeforeFall;

    public LayerMask enemyMask;
    public bool isLockedOn;
    public GameObject lockOnTarget;


    public readonly int IDLE_ANIMATION = Animator.StringToHash("Player_Idle");
    public readonly int IDLE_COMBAT_ANIMATION = Animator.StringToHash("Player_Idle_Combat");
    
    public readonly int WALK_START_F_0 = Animator.StringToHash("Player_Walk_Start_F_0");
    public readonly int WALK_LOOP_F_0 = Animator.StringToHash("Player_Walk_Loop_F_0");
    public readonly int WALK_STOP_F_0 = Animator.StringToHash("Player_Walk_Stop_F_0");
    
    public readonly int RUN_START_F_0 = Animator.StringToHash("Player_Run_Start_F_0");
    public readonly int RUN_LOOP_F_0 = Animator.StringToHash("Player_Run_Loop_F_0");
    public readonly int RUN_STOP_F_0 = Animator.StringToHash("Player_Run_Stop_F_0");
    
    public readonly int JUMP_0_START = Animator.StringToHash("Player_Jump_0_Start");
    public readonly int JUMP_0_LOOP= Animator.StringToHash("Player_Jump_0_Loop");
    public readonly int JUMP_0_END = Animator.StringToHash("Player_Jump_0_End");
    
    public readonly int DASH_F_0 = Animator.StringToHash("Player_Dash_F_0");
    public readonly int DASH_TO_RUN_F_0 = Animator.StringToHash("Player_Dash_to_Run_F_0");
    public readonly int DASH_AIR_F_0 = Animator.StringToHash("Player_Dash_Air_F_0");
    
    
    public readonly int COMBAT_WALK = Animator.StringToHash("Player_Combat_Walk");
    public readonly int COMBAT_RUN = Animator.StringToHash("Player_Combat_Run");
    public readonly int HORIZONTAL_PARAM = Animator.StringToHash("Horizontal");
    public readonly int VERTICAL_PARAM = Animator.StringToHash("Vertical");

    public readonly int COMBAT_DASH = Animator.StringToHash("Player_Combat_Dash");
    public readonly int DASHHORIZONTAL_PARAM = Animator.StringToHash("DashHorizontal");
    public readonly int DASHVERTICAL_PARAM = Animator.StringToHash("DashVertical");
    
    public readonly int PLAYER_COMBO_01_1 = Animator.StringToHash("Player_Combo_01-1");
    public readonly int PLAYER_COMBO_01_2 = Animator.StringToHash("Player_Combo_01-2");
    public readonly int PLAYER_COMBO_01_3 = Animator.StringToHash("Player_Combo_01-3");
    public readonly int PLAYER_COMBO_01_4 = Animator.StringToHash("Player_Combo_01-4");
    public readonly int NEXT_COMBO_TRIGGER = Animator.StringToHash("NextCombo");

    public readonly int PLAYER_CHARGE_ATTACK_START = Animator.StringToHash("Player_Charge_Attack_Start");
    public readonly int PLAYER_CHARGE_ATTACK_LOOP = Animator.StringToHash("Player_Charge_Attack_Loop");
    public readonly int PLAYER_CHARGE_ATTACK_END = Animator.StringToHash("Player_Charge_Attack_End");
    
    public BaseState previousState;
    public BaseState currentState;

    private void Awake()
    {
        playerManager = GetComponent<PlayerManager>();
    }

    private void Start()
    {
        InitializeStateMachine();
    }

    private void Update()
    {
        currentState.OnUpdateState();

        AdjustMaxSpeed();
        
        CheckFall();
    }

    private void FixedUpdate()
    {
        currentState.OnFixedUpdateState();
    }

    private void OnEnable()
    {
        PlayerInputManager.OnRunPerformed += HandleRunStart;
        PlayerInputManager.OnRunCanceled += HandleRunStop;
        PlayerInputManager.OnJumpPerformed += HandleJumpInput;
        PlayerInputManager.OnDashPerformed += HandleDashInput;
        PlayerInputManager.OnLockOnPerformed += HandleLockOnInput;
        PlayerInputManager.OnAttackPerformed += HandleAttackInput;
        PlayerInputManager.OnChargeAttackPerformed += HandleChargeAttackStart;
        PlayerInputManager.OnChargeAttackCanceled += HandleChargeAttackStop;
    }
    
    private void OnDisable()
    {
        PlayerInputManager.OnRunPerformed -= HandleRunStart;
        PlayerInputManager.OnRunCanceled -= HandleRunStop;
        PlayerInputManager.OnJumpPerformed -= HandleJumpInput;
        PlayerInputManager.OnDashPerformed -= HandleDashInput;
        PlayerInputManager.OnLockOnPerformed -= HandleLockOnInput;
        PlayerInputManager.OnAttackPerformed -= HandleAttackInput;
        PlayerInputManager.OnChargeAttackPerformed -= HandleChargeAttackStart;
        PlayerInputManager.OnChargeAttackCanceled -= HandleChargeAttackStop;
    }
    
    private void HandleRunStart()
    {
        isRunning = true;
    }

    private void HandleRunStop()
    {
        isRunning = false;
    }

    private void HandleJumpInput()
    {
        if (playerManager.StateManager.isGrounded && currentState != jumpState && currentState != dashState
            && ((playerManager.StateManager.isTouchingSlope 
                 && playerManager.StateManager.currentSurfaceAngle <= playerManager.StateManager.maxClimbableSlopeAngle) 
                || !playerManager.StateManager.isTouchingSlope))
        {
            ChangeState(jumpState);
        }
    }

    private void HandleDashInput()
    {
        if (currentState == dashState) return;
        ChangeState(dashState);
    }

    private void HandleLockOnInput()
    {
        if (isLockedOn) // 이미 lock on 중인 경우
        {
            isLockedOn = false;
            lockOnTarget = null;
            if (currentState == idleState) PlayAnimation(IDLE_ANIMATION);
            else if (currentState == walkState) PlayAnimation(WALK_LOOP_F_0);
            else if (currentState == runState) PlayAnimation(RUN_LOOP_F_0);
        }
        else // lock on 을 안하고 있는 경우
        {
            if (CheckNearbyEnemies(out GameObject targetEnemy))
            {
                isLockedOn = true;
                lockOnTarget = targetEnemy;
                
                if (currentState == idleState) PlayAnimation(IDLE_COMBAT_ANIMATION);
                else if (currentState == walkState) PlayAnimation(COMBAT_WALK);
                else if (currentState == runState) PlayAnimation(COMBAT_RUN);
            }
        }
    }

    private void HandleAttackInput()
    {
        if (playerManager.StateManager.isGrounded &&
            ((playerManager.StateManager.isTouchingSlope 
              && playerManager.StateManager.currentSurfaceAngle <= playerManager.StateManager.maxClimbableSlopeAngle) 
             || !playerManager.StateManager.isTouchingSlope)
            )
        {
            if (currentState == attackState)
                attackState.bufferedInput = true;
            else
                ChangeState(attackState);
        }
    }

    private void HandleChargeAttackStart()
    {
        if (playerManager.StateManager.isGrounded &&
            ((playerManager.StateManager.isTouchingSlope
              && playerManager.StateManager.currentSurfaceAngle <= playerManager.StateManager.maxClimbableSlopeAngle)
             || !playerManager.StateManager.isTouchingSlope) &&
            currentState != attackState
           )
        {
            ChangeState(chargeAttackState);
        }
    }

    private void HandleChargeAttackStop()
    {
        if (currentState == chargeAttackState) ChangeState(idleState);
    }

    public void PlayAnimation(int animationHash, float durationTime = 0.2f, float timeOffset = 0f)
    {
        playerManager.animator.CrossFadeInFixedTime(animationHash, durationTime, 0, timeOffset);
    }
    
    public void ChangeState(BaseState newState)
    {
        currentState.OnExitState();
        previousState = currentState;
        currentState = newState;
        currentState.OnEnterState();
    }
    private void InitializeStateMachine()
    {
        idleState = new PlayerIdleState(this);
        walkState = new PlayerWalkState(this);
        slideState = new PlayerSlideState(this);
        runState = new PlayerRunState(this);
        fallState = new PlayerFallState(this);
        jumpState = new PlayerJumpState(this);
        dashState = new PlayerDashState(this);
        attackState = new PlayerAttackState(this);
        chargeAttackState = new PlayerChargeAttackState(this);
        
        currentState = idleState;
        currentState.OnEnterState();
    }

    private void CheckFall()
    {
        if (!playerManager.StateManager.isGrounded) fallTime += Time.deltaTime;
        else fallTime = 0f;
        
        if (!playerManager.StateManager.isGrounded 
            && currentState != jumpState 
            && currentState != fallState
            && currentState != dashState
            && fallTime >= fallThreshold)
        {
            storedVelocityBeforeFall = playerManager.rb.linearVelocity;
            ChangeState(fallState);
        }
    }

    private void AdjustMaxSpeed()
    {
        if (playerManager.StateManager.isGrounded && currentState != dashState && currentState != jumpState) playerManager.maxAirSpeed = playerManager.runSpeed;
    }

    // 화면 정중앙과 가장 가까운 enemy gameObject를 반환해줌
    private bool CheckNearbyEnemies(out GameObject enemy)
    {
        Collider[] nearbyEnemies = new Collider[20];
        int enemyCount = Physics.OverlapSphereNonAlloc(transform.position + playerManager.playerCollider.center,
            playerManager.enemyDetectionRange, nearbyEnemies, enemyMask);

        if (enemyCount > 0)
        {
            Vector2 screenCenter = new Vector2(Screen.width / 2, Screen.height / 2);
            Vector2 targetScreenPosition = Camera.main.WorldToScreenPoint(nearbyEnemies[0].transform.position);
            float nearestDistance = Vector2.Distance(screenCenter, targetScreenPosition);
            Collider nearestEnemy = nearbyEnemies[0];
            
            for (int i = 1; i < enemyCount; i++)
            {
                screenCenter = new Vector2(Screen.width / 2, Screen.height / 2);
                targetScreenPosition = Camera.main.WorldToScreenPoint(nearbyEnemies[i].transform.position);
                float distance = Vector2.Distance(screenCenter, targetScreenPosition);

                if (distance < nearestDistance)
                {
                    nearestEnemy = nearbyEnemies[i];
                    nearestDistance = distance;
                }
            }

            enemy = nearestEnemy.gameObject;
            return true;
        }

        enemy = null;
        return false;
    }

    public void OpenComboWindow()
    {
        if (currentState == attackState)
        {
            attackState.isComboWindowOpen = true;
        }
    }
    
    public void CloseComboWindow()
    {
        if (currentState == attackState)
        {
            attackState.isComboWindowOpen = false;
        }
    }

    public void Lunge(float force)
    {
        ResetLinearVelocity();

        Vector3 dir = transform.forward.normalized;
        playerManager.rb.AddForce(dir * force, ForceMode.Impulse);
    }

    public void AttackJump(float force)
    {
        ResetLinearVelocity();
        
        // 정면 대각선 위 방향으로 힘을 '한 번'만 가합니다.
        Vector3 dir = (transform.forward + (Vector3.up * 0.5f)).normalized;
        playerManager.rb.AddForce(dir * force, ForceMode.Impulse);
    }

    public void ResetLinearVelocity()
    {
        playerManager.rb.linearVelocity = Vector3.zero;
    }

    #if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        if (Application.isPlaying)
        {
            Gizmos.color = CheckNearbyEnemies(out GameObject targetEnemy) ? Color.red : Color.green;
            Gizmos.DrawWireSphere(transform.position + playerManager.playerCollider.center, playerManager.enemyDetectionRange);

            if (lockOnTarget != null && isLockedOn)
            {
                Gizmos.color = Color.cyan;
                Gizmos.DrawWireSphere(lockOnTarget.transform.position + Vector3.up * 2f, 0.3f);
            }

            Gizmos.color = Color.blue;
            Gizmos.DrawLine(transform.position + playerManager.playerCollider.center, 
                transform.position + playerManager.playerCollider.center + transform.forward);
        }
    }
    #endif
}
