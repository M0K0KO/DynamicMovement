using UnityEngine;
using UnityEngine.XR;
using Quaternion = UnityEngine.Quaternion;
using Vector3 = UnityEngine.Vector3;

public class PlayerStateManager : MonoBehaviour
{
    private PlayerManager playerManager;

    private float originalColliderHeight;

    [Header("GroundCheck Properties")] 
    public Vector3 groundCheckOffset;
    public float groundCheckerThreshold = 0.2f;
    [SerializeField] public LayerMask groundMask;

    [Header("WallCheck Properties")] public float
        wallCheckerThreshold = 0.8f; // Distance from the player head used to check if the player is touching a wall

    public float wallCheckStartDistance = 0.5f; // Wall checker Distance from the player center

    [Header("SlopeCheck Properties")]
    public float forwardRayOffset;
    public float maxClimbableSlopeAngle { get; private set; } = 53.6f;

    [Header("Gravity Properties")] 
    public float planeGravity;
    public float climbableSlopeGravity;
    public float unclimbableSlopeGravity;


    public bool isGrounded { get; private set; }
    public float currentSurfaceAngle { get; private set; }
    public bool isTouchingSlope { get; private set; }
    public bool isTouchingClimbableSlope { get; private set; }


    public Vector3 desiredForward;
    public Vector3 forward;
    public Vector3 down;
    public Vector3 baseGroundNormal;
    public Vector3 forwardGroundNormal;

    public bool lockOnSlope { get; set; } = true;

    private void Awake()
    {
        playerManager = GetComponent<PlayerManager>();

        originalColliderHeight = playerManager.playerCollider.height;
    }

    private void FixedUpdate()
    {
        CalculateRelativeDirection();
        
        CheckGrounded();

        UpdateGroundNormals();
        CheckSlope();

        ApplyGravity();
    }

    private void CalculateRelativeDirection() // calculate desired Direction 
    {
        
        if (playerManager.InputManager.moveInput != Vector2.zero)
        {
            Vector2 input = playerManager.InputManager.moveInput;
            Vector3 camForward = playerManager.playerCam.transform.forward;
            Vector3 camRight = playerManager.playerCam.transform.right;
            camForward.y = 0;
            camForward.Normalize();
            camRight.y = 0;
            camRight.Normalize();
            desiredForward = (camForward * input.y) + (camRight * input.x);
        }
        else
        {
            desiredForward = transform.forward;
        }
    }

    private void CheckGrounded() // Physics CheckSphere
    {
        isGrounded = Physics.CheckSphere(transform.position - new Vector3(0, originalColliderHeight / 2f, 0) - groundCheckOffset,
            groundCheckerThreshold, groundMask);
    }

    // 캐릭터의 transform.forward방향 기준 정면에서 밑으로 Raycast
    // 안정적인 ForwardGroundNormal을 얻어냄.
    // 현재 캐릭터의 Vector3.down Ray의 normal과 다르다면, 정면 방향에 Slope가 있는 것으로 생각할 수 있음.
    // Slope를 감지했다면, projectOnPlane을 통해 진행방향을 Slope에 Lock
    // Slope를 빠져나오는 것은 마찬가지로 ForwardGroundNormal이 0이고, Vector.down groundNormal과 다를 때를 기준으로 함.
    // Slope를 빠져나온다면, projectOnPlane을 통해 진향방향을 Plane에 Lock
    // Slope를 감지하면, 해당 Slope가 Climbable인지 확인함. Climbable이라면, Slope에 Lock. 

    private void UpdateGroundNormals()
    {
        Vector3 currentPos = transform.position + playerManager.playerCollider.center;
        Vector3 forwardPos = currentPos + new Vector3(desiredForward.x, 0, desiredForward.z).normalized * forwardRayOffset;
        
        // baseGroundNormal
        if (Physics.Raycast(currentPos,
                Vector3.down, out RaycastHit hit, Mathf.Infinity, groundMask))
        {
            baseGroundNormal = hit.normal.normalized;
        }

        
        // forwardGroundNormal
        if (Physics.Raycast(forwardPos, Vector3.down, out hit, Mathf.Infinity, groundMask))
        {
            forwardGroundNormal = hit.normal.normalized;
        }
    } // Updates BaseNormal, GroundNormal via Raycast

    private void CheckSlope()
    {
        // base, forward 둘 다 평지라면 현재 평지, 둘 중 하나라도 slope면 현재 slope로....
        isTouchingSlope = !(
            (baseGroundNormal - Vector3.up).sqrMagnitude < 0.05f // base가 평지?
            && (forwardGroundNormal - Vector3.up).sqrMagnitude < 0.05f // forward가 평지?
            );

        isTouchingClimbableSlope =
            isTouchingSlope && Vector3.Angle(forwardGroundNormal, Vector3.up) < maxClimbableSlopeAngle;
        
        Vector3 horizontalDirection = new Vector3(desiredForward.x, 0, desiredForward.z).normalized;
        forward = horizontalDirection; 
        down = -transform.up.normalized; 
        
        if (isGrounded) // Slope 감지는 땅에 있을 때만
        {
            if (isTouchingSlope) // 경사가 달라진다!
            {
                if (Vector3.Angle(forwardGroundNormal, Vector3.up) > maxClimbableSlopeAngle) // 오를 수 없는 경사의 Slope
                {
                    forward = Vector3.zero;
                    down = -transform.up.normalized;
                }
                else if (lockOnSlope)
                {
                    Vector3 referenceNormal;
                    if ((baseGroundNormal - Vector3.up).sqrMagnitude < 0.05f || (baseGroundNormal - forwardGroundNormal).sqrMagnitude > 0.05f)
                    {
                        referenceNormal = baseGroundNormal;
                    }
                    else
                    {
                        referenceNormal = forwardGroundNormal;
                    }

                    forward = Vector3.ProjectOnPlane(horizontalDirection, referenceNormal).normalized;
                    down = -referenceNormal.normalized;
                }
            }
        }
        else // 공중이라면?
        {
            isTouchingSlope = false;
            isTouchingClimbableSlope = false;
        }
        
    }

    private void ApplyGravity()
    {
        Vector3 gravity = Vector3.zero;

        if (isTouchingSlope) // 경사로
        {
            if (isTouchingClimbableSlope) // 오를 수 있는 경사의 Slope
            {
                gravity = down * (-Physics.gravity.y * climbableSlopeGravity);
            }
            else // 오를 수 없는 경사의 Slope
            {
                gravity = down * (-Physics.gravity.y * unclimbableSlopeGravity);
            }
        }
        else // 평지
        {
            gravity = down * (-Physics.gravity.y * planeGravity);
        }

        playerManager.rb.AddForce(gravity);
    }

    private void OnDrawGizmos()
    {
        if (Application.isPlaying)
        {
            Vector3 currentPos = transform.position + playerManager.playerCollider.center;
            Vector3 forwardPos = currentPos +
                                 new Vector3(desiredForward.x, 0, desiredForward.z).normalized * forwardRayOffset;

            Gizmos.color = Physics.CheckSphere(
                transform.position - new Vector3(0, originalColliderHeight / 2f, 0) - groundCheckOffset,
                groundCheckerThreshold, groundMask)
                ? Color.green
                : Color.red;
            Gizmos.DrawWireSphere(transform.position - new Vector3(0, originalColliderHeight / 2f, 0) - groundCheckOffset,
                groundCheckerThreshold);
            
            
            Gizmos.color = Color.red;

            // --- 1. forwardPos에서 아래로 쏘는 Raycast ---
            // Raycast를 실제로 실행하여 충돌 정보를 얻음
            if (Physics.Raycast(forwardPos, Vector3.down, out RaycastHit hitForward, 100f, groundMask))
            {
                // Raycast가 무언가에 맞았다면: 녹색 선으로 표시
                Gizmos.color = Color.green;
                Gizmos.DrawLine(forwardPos, hitForward.point);
                Gizmos.DrawWireSphere(hitForward.point, 0.1f); // 맞은 위치에 작은 구체 표시
            }
            else
            {
                // 맞지 않았다면: 빨간색 선으로 최대 길이까지 표시
                Gizmos.color = Color.red;
                Gizmos.DrawLine(forwardPos, forwardPos + Vector3.down * 100f);
            }


            // --- 2. currentPos에서 아래로 쏘는 Raycast ---
            // Raycast를 실제로 실행하여 충돌 정보를 얻음
            if (Physics.Raycast(currentPos, Vector3.down, out RaycastHit hitCurrent, 100f, groundMask))
            {
                // Raycast가 무언가에 맞았다면: 녹색 선으로 표시
                Gizmos.color = Color.green;
                Gizmos.DrawLine(currentPos, hitCurrent.point);
                Gizmos.DrawWireSphere(hitCurrent.point, 0.1f); // 맞은 위치에 작은 구체 표시
            }
            else
            {
                // 맞지 않았다면: 빨간색 선으로 최대 길이까지 표시
                Gizmos.color = Color.red;
                Gizmos.DrawLine(currentPos, currentPos + Vector3.down * 100f);
            }

            Gizmos.color = Color.cyan;
            Gizmos.DrawLine(currentPos, currentPos + forward);

            Gizmos.color = Color.magenta;
            Gizmos.DrawLine(currentPos, currentPos + down);
        }
    }
}















