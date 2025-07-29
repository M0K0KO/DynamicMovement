using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInputManager : MonoBehaviour
{
    private PlayerManager playerManager;

    [Header("PlayerInputAction")]
    private PlayerInput playerInput;
    private InputAction MoveAction;
    private InputAction RunAction;
    private InputAction JumpAction;
    private InputAction DashAction;
    private InputAction LockOnAction;

    [Header("Input Properties")] 
    public Vector2 moveInput { get; private set; }

    public static event Action OnRunPerformed;
    public static event Action OnRunCanceled;
    public static event Action OnJumpPerformed;
    public static event Action OnDashPerformed;
    public static event Action OnLockOnPerformed;

    private void Awake()
    {
        playerManager = GetComponent<PlayerManager>();
        
        playerInput = new PlayerInput();
        MoveAction = playerInput.Normal.Move;
        RunAction = playerInput.Normal.Run;
        JumpAction = playerInput.Normal.Jump;
        DashAction = playerInput.Normal.Dash;
        LockOnAction = playerInput.Normal.LockOn;
    }

    private void OnEnable()
    {
        playerInput.Enable();
        
        MoveAction.performed += OnMove;
        MoveAction.canceled += OnMove;
        RunAction.performed += OnRun_Performed;
        RunAction.canceled += OnRun_Canceled;
        JumpAction.performed += OnJump_Performed;
        DashAction.performed += OnDash_Performed;
        LockOnAction.performed += OnLockOn_Performed;
    }



    private void OnDisable()
    {
        MoveAction.performed -= OnMove;
        MoveAction.canceled -= OnMove;
        RunAction.performed -= OnRun_Performed;
        RunAction.canceled -= OnRun_Canceled;
        JumpAction.performed -= OnJump_Performed;
        DashAction.performed -= OnDash_Performed;
        LockOnAction.performed -= OnLockOn_Performed;
        
        playerInput.Disable();
    }
    
    private void OnMove(InputAction.CallbackContext ctx)
    {
        moveInput = ctx.ReadValue<Vector2>();
    }

    private void OnRun_Performed(InputAction.CallbackContext ctx)
    {
        OnRunPerformed?.Invoke();
    }

    private void OnRun_Canceled(InputAction.CallbackContext ctx)
    {
        OnRunCanceled?.Invoke();
    }
    
    private void OnJump_Performed(InputAction.CallbackContext ctx)
    {
        OnJumpPerformed?.Invoke();
    }

    private void OnDash_Performed(InputAction.CallbackContext ctx)
    {
        OnDashPerformed?.Invoke();
    }

    private void OnLockOn_Performed(InputAction.CallbackContext ctx)
    {
        OnLockOnPerformed?.Invoke();
    }
}
