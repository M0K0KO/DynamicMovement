using System;
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


    public bool isRunning;
    public bool shouldPlayJumpEnd;

    
    private float fallTime = 0;
    public float fallThreshold = 0.3f;
    public Vector3 storedVelocityBeforeFall;


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
        DebugState();
        
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
    }
    
    private void OnDisable()
    {
        PlayerInputManager.OnRunPerformed -= HandleRunStart;
        PlayerInputManager.OnRunCanceled -= HandleRunStop;
        PlayerInputManager.OnJumpPerformed -= HandleJumpInput;
        PlayerInputManager.OnDashPerformed -= HandleDashInput;
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
        if (playerManager.StateManager.isGrounded && currentState != jumpState
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
        if (playerManager.StateManager.isGrounded && currentState != dashState) playerManager.maxAirSpeed = playerManager.runSpeed;
    }

    #if UNITY_EDITOR
    private void DebugState()
    {
        switch (currentState)
        {
            case PlayerIdleState:
                Debug.Log($"Current State : IdleState");
                break;
            case PlayerWalkState:
                Debug.Log($"Current State : WalkState");
                break;
            case PlayerRunState:
                Debug.Log($"Current State : RunState");
                break;
            case PlayerFallState:
                Debug.Log($"Current State : FallState");
                break;
            case PlayerJumpState:
                Debug.Log($"Current State : JumpState");
                break;
            case PlayerDashState:
                Debug.Log($"Current State : DashState");
                break;
        }
    }
    #endif
}
