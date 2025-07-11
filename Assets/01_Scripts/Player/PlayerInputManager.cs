using UnityEngine;
using UnityEngine.InputSystem;

// Updates values that need preprocessing before being used
public class PlayerInputManager : MonoBehaviour
{
    private PlayerInput playerInput;
    private InputAction moveAction;
    private InputAction dashAction;
    private InputAction normalAttackAction;

    [Header("Movement Values")]
    [field: SerializeField] public Vector3 moveInputDirection { get; private set; }
    [field: SerializeField] public bool dashInput { get; private set; }
    [field: SerializeField] public bool normalAttackInput { get; private set; }

    #region Built-In Functions
    private void Awake()
    {
        playerInput = new PlayerInput();
        moveAction = playerInput.Movement.Move;
        dashAction = playerInput.Movement.Dash;
        normalAttackAction = playerInput.Movement.NormalAttack;
    }

    private void OnEnable()
    {
        playerInput.Enable();
        moveAction.performed += OnMove;
        dashAction.performed += OnDash;
        normalAttackAction.performed += OnNormalAttack;
    }

    private void OnDisable()
    {
        playerInput.Disable();
        moveAction.performed -= OnMove;
        dashAction.performed -= OnDash;
        normalAttackAction.performed -= OnNormalAttack;
    }
    #endregion

    
    
    #region Callback Functions
    private void OnMove(InputAction.CallbackContext context)
    {
        Vector2 input = context.ReadValue<Vector2>();
        moveInputDirection = new Vector3(input.x, 0, input.y).normalized;
    }

    private void OnDash(InputAction.CallbackContext context)
    {
        dashInput = true;
    }

    private void OnNormalAttack(InputAction.CallbackContext context)
    {
        normalAttackInput = true;
    }
    #endregion

    public void ClearDashInput()
    {
        dashInput = false;
    }

    public void ClearNormalAttackInput()
    {
        normalAttackInput = false;
    }
}
