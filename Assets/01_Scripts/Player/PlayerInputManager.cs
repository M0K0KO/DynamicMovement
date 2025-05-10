using UnityEngine;
using UnityEngine.InputSystem;

// Updates values that need preprocessing before being used
public class PlayerInputManager : MonoBehaviour
{
    private PlayerInput playerInput;
    private InputAction moveAction;

    [Header("Movement Values")]
    [field: SerializeField]
    public Vector3 moveInputDirection { get; private set; }
    
    #region Built-In Functions
    private void Awake()
    {
        playerInput = new PlayerInput();
        moveAction = playerInput.Movement.Move;
    }

    private void OnEnable()
    {
        moveAction.Enable();
        moveAction.performed += OnMove;
    }

    private void OnDisable()
    {
        moveAction.Disable();
        moveAction.performed -= OnMove;
    }
    #endregion

    #region Callback Functions
    private void OnMove(InputAction.CallbackContext context)
    {
        Vector2 input = context.ReadValue<Vector2>();
        moveInputDirection = new Vector3(input.x, 0, input.y).normalized;
    }
    #endregion
}
