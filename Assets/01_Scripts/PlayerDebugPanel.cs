using TMPro;
using UnityEngine;

public class PlayerDebugPanel : MonoBehaviour
{
    [SerializeField] 
    private TextMeshProUGUI display;

    [SerializeField] 
    private PlayerManager playerManager;

    void Update()
    {
        Vector3 horizontalVelocity = new Vector3(playerManager.rb.linearVelocity.x, 0, playerManager.rb.linearVelocity.z);
        float verticalVelocity = playerManager.rb.linearVelocity.y;
        
        display.SetText($" Grounded : {playerManager.StateManager.isGrounded}\n" +
                        $" TouchingWall : {playerManager.StateManager.isTouchingWall}\n" +
                        $" TouchingSlope : {playerManager.StateManager.isTouchingSlope}\n" +
                        $" horizontalVelocity : " + horizontalVelocity.magnitude.ToString("F2") + "\n" +
                        $" verticalVelocity : " + verticalVelocity.ToString("F2") + "\n" +
                        $" maxAirVelocity : " + playerManager.maxAirSpeed.ToString("F2") + "\n" +
                        $" Velocity : " + playerManager.rb.linearVelocity.magnitude.ToString("F2") + "\n" +
                        $" State : {DebugState(playerManager.StateMachine.currentState)}\n");
    }
    
    private string DebugState(BaseState state)
    {
        switch (state)
        {
            case PlayerIdleState:
                return "Idle";
            case PlayerWalkState:
                return "Walk";
            case PlayerRunState:
                return "Run";
            case PlayerFallState:
                return "Fall";
            case PlayerJumpState:
                return "Jump";
            case PlayerDashState:
                return "Dash";
        }

        return "NULL";
    }
}
