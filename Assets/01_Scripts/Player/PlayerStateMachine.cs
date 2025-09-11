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
    public PlayerRunState runState { get; private set; }
    public PlayerFallState fallState { get; private set; }
    public PlayerLandState landState { get; private set; }
    public PlayerJumpState jumpState { get; private set; }
    public PlayerDashState dashState { get; private set; }
    public PlayerAttackState attackState { get; private set; }
    public PlayerChargeAttackState chargeAttackState { get; private set; }
    public PlayerChargeAttackEndState chargeAttackEndState { get; private set; }
    public PlayerDashAttackState dashAttackState { get; private set; }


    public bool isRunning;

    private float fallTime = 0;
    public float fallThreshold = 0.3f;
    public Vector3 storedVelocityBeforeFall;

    public LayerMask enemyMask;
    public bool isLockedOn;
    public GameObject lockOnTarget;
    
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
        PlayerInputManager.OnRunPerformed += HandleRunStartInput;
        PlayerInputManager.OnRunCanceled += HandleRunStopInput;
        PlayerInputManager.OnJumpPerformed += HandleJumpInput;
        PlayerInputManager.OnDashPerformed += HandleDashInput;
        PlayerInputManager.OnLockOnPerformed += HandleLockOnInput;
        PlayerInputManager.OnAttackPerformed += HandleAttackInput;
        PlayerInputManager.OnChargeAttackPerformed += HandleChargeAttackStart;
        PlayerInputManager.OnChargeAttackCanceled += HandleChargeAttackStop;
        PlayerInputManager.OnDashAttackPerformed += HandleDashAttackInput;
    }
    private void OnDisable()
    {
        PlayerInputManager.OnRunPerformed -= HandleRunStartInput;
        PlayerInputManager.OnRunCanceled -= HandleRunStopInput;
        PlayerInputManager.OnJumpPerformed -= HandleJumpInput;
        PlayerInputManager.OnDashPerformed -= HandleDashInput;
        PlayerInputManager.OnLockOnPerformed -= HandleLockOnInput;
        PlayerInputManager.OnAttackPerformed -= HandleAttackInput;
        PlayerInputManager.OnChargeAttackPerformed -= HandleChargeAttackStart;
        PlayerInputManager.OnChargeAttackCanceled -= HandleChargeAttackStop;
        PlayerInputManager.OnDashAttackPerformed -= HandleDashAttackInput;
    }
    
    
    // Input Handling via CurrentState
    private void HandleRunStartInput()
    {
        isRunning = true;
    }
    
    private void HandleRunStopInput()
    {
        isRunning = false;
    }
    
    private void HandleJumpInput()
    {
        if (playerManager.StateManager.isGrounded
            && currentState != jumpState 
            && currentState != dashState
            && currentState != landState
            && (playerManager.StateManager.isTouchingClimbableSlope || !playerManager.StateManager.isTouchingSlope))
        {
            ChangeState(jumpState);
        }
    }
    
    private void HandleDashInput()
    {
        if (currentState != dashState) ChangeState(dashState);
    }
    
    private void HandleLockOnInput()
    {
        if (isLockedOn)
        {
            isLockedOn = false;
            lockOnTarget = null;
        }
        else 
        {
            if (CheckNearbyEnemies(out GameObject targetEnemy))
            {
                isLockedOn = true;
                lockOnTarget = targetEnemy;
            }
        }
        
        playerManager.playerCameraManager.HandleDefaultCameraState();
    }
    
    private void HandleAttackInput()
    {
        if (playerManager.StateManager.isGrounded && currentState != dashState &&
            (playerManager.StateManager.isTouchingClimbableSlope || !playerManager.StateManager.isTouchingSlope))
        {
            if (currentState == attackState)
                attackState.bufferedInput = true;
            else
                ChangeState(attackState);
        }
    }
    
    private void HandleChargeAttackStart()
    {
        if (playerManager.StateManager.isGrounded && currentState != dashState &&
            (playerManager.StateManager.isTouchingClimbableSlope || !playerManager.StateManager.isTouchingSlope)
           )
        {
            ChangeState(chargeAttackState);
        }
    }
    
    private void HandleChargeAttackStop()
    {
        if (currentState == chargeAttackState) ChangeState(chargeAttackEndState);
    }

    private void HandleDashAttackInput()
    {
        if (playerManager.StateManager.isGrounded
            && isLockedOn
            && currentState != dashState
            && currentState != attackState
            && currentState != chargeAttackState
            && currentState != dashAttackState)
        {
            ChangeState(dashAttackState);
        }
    }
    
    
    public void ChangeState(BaseState newState)
    {
        currentState.OnExitState();
        currentState = newState;
        currentState.OnEnterState();
    }
    private void InitializeStateMachine()
    {
        idleState = new PlayerIdleState(this);
        walkState = new PlayerWalkState(this);
        runState = new PlayerRunState(this);
        landState = new PlayerLandState(this);
        fallState = new PlayerFallState(this);
        jumpState = new PlayerJumpState(this);
        dashState = new PlayerDashState(this);
        attackState = new PlayerAttackState(this);
        chargeAttackState = new PlayerChargeAttackState(this);
        chargeAttackEndState = new PlayerChargeAttackEndState(this);
        dashAttackState = new PlayerDashAttackState(this);
        
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
    } // Check Falling Based on FallTime
    private void AdjustMaxSpeed()
    {
        if (playerManager.StateManager.isGrounded && currentState != dashState && currentState != jumpState) 
            playerManager.maxAirSpeed = playerManager.runSpeed;
    } 
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
    } // Lock On Helper Function
    public void OpenComboWindow()
    {
        if (currentState == attackState)
        {
            attackState.isComboWindowOpen = true;
        }
    } // Anim Event
    public void CloseComboWindow()
    {
        if (currentState == attackState)
        {
            attackState.isComboWindowOpen = false;
        }
    } // Anim Event
    public void Lunge(float force)
    {
        ResetLinearVelocity();

        Vector3 dir = transform.forward.normalized;
        playerManager.rb.AddForce(dir * force, ForceMode.Impulse);
    } // Anim Event
    public void AttackJump(float force)
    {
        ResetLinearVelocity();
        
        Vector3 dir = (transform.forward + (Vector3.up * 0.5f)).normalized;
        playerManager.rb.AddForce(dir * force, ForceMode.Impulse);
    } // Anim Event
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
