using System;
using System.Numerics;
using UnityEngine;
using Quaternion = UnityEngine.Quaternion;
using Vector3 = UnityEngine.Vector3;

public class PlayerStateManager : MonoBehaviour
{
    private PlayerManager playerManager;
    
    private Rigidbody rb;
    private CapsuleCollider playerCollider;

    private float originalColliderHeight;

    [Header("GroundCheck Properties")] public float groundCheckerThreshold = 0.2f;
    [SerializeField] private LayerMask groundMask;

    [Header("WallCheck Properties")] public float
        wallCheckerThreshold = 0.8f; // Distance from the player head used to check if the player is touching a wall

    public float wallCheckStartDistance = 0.5f; // Wall checker Distance from the player center

    [Header("Slope & Direction Check Properties")]
    public float slopeCheckerThreshold = 0.5f;
    public float maxClimbableSlopeAngle { get; private set; } = 53.6f;

    [Header("Friction & Multiplier Properties")]
    public float frictionAgainstFloor { get; private set; } = 0.3f;
    public float gravityMultiplier = 6f;
    public float gravityMultiplyerOnSlideChange = 3f;
    public float gravityMultiplierIfUnclimbableSlope = 30f;
    public float frictionAgainstWall = 0.839f;

    
    public AnimationCurve speedMultiplierOnAngle = AnimationCurve.EaseInOut(0f, 0f, 1f, 1f);
    [Range(0.01f, 1f)] public float canSlideMultiplierCurve = 0.061f;
    [Range(0.01f, 1f)] public float cantSlideMultiplierCurve = 0.039f;
    


    public bool prevGrounded { get; private set; } // prevGrounded 와 isGrounded가 다를 때 착지 or 점프를 감지할 때 사용
    public bool isGrounded { get; private set; }
    public bool isTouchingWall { get; private set; }
    public Vector3 wallNormal { get; private set; }
    public Vector3 prevGroundNormal { get; private set; } // prevGroundNormal과 groundNormal이 다를 때, 다른 경사면으로 간 것
    public Vector3 groundNormal { get; private set; }
    public float targetAngle { get; private set; }
    public bool currentLockOnSlope { get; private set; } // 현재 경사면에 lock되어있어야하나?
    public bool lockOnSlope;
    public float currentSurfaceAngle { get; private set; }
    public bool isTouchingSlope { get; private set; }

    private Vector3 forward;
    private Vector3 globalForward;
    private Vector3 reactionForward;
    private Vector3 down;
    private Vector3 globalDown;
    private Vector3 reactionGlobalDown;

    private void Awake()
    {
        playerManager = GetComponent<PlayerManager>();

        rb = playerManager.rb;
        playerCollider = playerManager.playerCollider;
        originalColliderHeight = playerCollider.height;
    }

    private void FixedUpdate()
    {
        CheckGrounded();
        CheckSlopeAndDirection();
        CheckWall();
        
        ApplyGravity();
    }


    private void CheckGrounded()
    {
        prevGrounded = isGrounded;
        isGrounded = Physics.CheckSphere(transform.position - new Vector3(0, originalColliderHeight / 2f, 0),
            groundCheckerThreshold, groundMask);
    }
    private void CheckWall()
    {
        bool tempWall = false;
        Vector3 tempWallNormal = Vector3.zero;
        Vector3 topWallPos = new Vector3(transform.position.x, transform.position.y + wallCheckStartDistance,
            transform.position.z);

        for (int i = 0; i < 8; i++)
        {
            var angle = i * 45f;
            var direction = Quaternion.AngleAxis(angle, transform.up) * globalForward;

            if (Physics.Raycast(topWallPos, direction, out RaycastHit wallHit, wallCheckerThreshold, groundMask))
            {
                tempWallNormal = wallHit.normal;
                tempWall = true;
                break;
            }
        }

        isTouchingWall = tempWall;
        wallNormal = tempWallNormal;
    }
    private void CheckSlopeAndDirection()
    {
        prevGroundNormal = groundNormal;
        Vector3 desiredForward = transform.forward;
        
        if (Physics.SphereCast(transform.position, slopeCheckerThreshold, Vector3.down, out RaycastHit slopeHit,
                originalColliderHeight / 2f + 0.5f, groundMask))
        {
            groundNormal = slopeHit.normal;
            
            if (Mathf.Approximately(groundNormal.y, 1f)) // 평지
            {
                forward = Quaternion.Euler(0f, targetAngle, 0f) * Vector3.forward;
                globalForward = forward;
                reactionForward = forward;

                SetFriction(frictionAgainstFloor, true);
                currentLockOnSlope = lockOnSlope;

                currentSurfaceAngle = 0f;
                isTouchingSlope = false;
            }
            else
            {
                Vector3 tempGlobalForward = transform.forward.normalized;
                Vector3 tempForward = new Vector3(tempGlobalForward.x,
                    Vector3.ProjectOnPlane(tempGlobalForward, slopeHit.normal).normalized.y,
                    tempGlobalForward.z);
                Vector3 tmpReactionForward =
                    new Vector3(tempForward.x, tempGlobalForward.y - tempForward.y, tempForward.z);

                if (currentSurfaceAngle <= maxClimbableSlopeAngle && !isTouchingSlope)
                {
                    forward = tempForward * ((speedMultiplierOnAngle.Evaluate(currentSurfaceAngle / 90f) *
                                              canSlideMultiplierCurve) + 1f);
                    globalForward = tempGlobalForward *
                                    ((speedMultiplierOnAngle.Evaluate(currentSurfaceAngle / 90f) *
                                      canSlideMultiplierCurve) + 1f);
                    reactionForward = tmpReactionForward *
                                      ((speedMultiplierOnAngle.Evaluate(currentSurfaceAngle / 90f) *
                                        canSlideMultiplierCurve) + 1f);

                    SetFriction(frictionAgainstFloor, true);
                    currentLockOnSlope = lockOnSlope;
                }
                else
                {
                    forward = tempForward * ((speedMultiplierOnAngle.Evaluate(currentSurfaceAngle / 90f) *
                                              cantSlideMultiplierCurve) + 1f);
                    globalForward = tempGlobalForward * ((speedMultiplierOnAngle.Evaluate(currentSurfaceAngle / 90f) *
                                                          cantSlideMultiplierCurve) + 1f);
                    reactionForward = tmpReactionForward *
                                      ((speedMultiplierOnAngle.Evaluate(currentSurfaceAngle / 90f) *
                                        cantSlideMultiplierCurve) + 1f);

                    SetFriction(0f, true);
                    currentLockOnSlope = lockOnSlope;
                }

                currentSurfaceAngle = Vector3.Angle(Vector3.up, slopeHit.normal);
                isTouchingSlope = true;
            }

            down = Vector3.Project(Vector3.down, slopeHit.normal);
            globalDown = Vector3.down.normalized;
            reactionGlobalDown = Vector3.up.normalized;
        }
        else
        {
            groundNormal = Vector3.zero;

            forward = Vector3.ProjectOnPlane(transform.forward, slopeHit.normal).normalized;
            globalForward = forward;
            reactionForward = forward;

            down = Vector3.down.normalized;
            globalDown = Vector3.down.normalized;
            reactionGlobalDown = Vector3.up.normalized;

            SetFriction(frictionAgainstFloor, true);
            currentLockOnSlope = lockOnSlope;
        }
    }

    private void SetFriction(float frictionWall, bool isMinimum)
    {
        playerCollider.material.dynamicFriction = 0.6f * frictionWall;
        playerCollider.material.staticFriction = 0.6f * frictionWall;

        if (isMinimum) playerCollider.material.frictionCombine = PhysicsMaterialCombine.Minimum;
        else playerCollider.material.frictionCombine = PhysicsMaterialCombine.Maximum;
    }


    private void ApplyGravity()
    {
        Vector3 gravity = Vector3.zero;
        if (currentLockOnSlope) gravity = down * (gravityMultiplier * -Physics.gravity.y);
        else gravity = globalDown * (gravityMultiplier * -Physics.gravity.y);
        
        if (!Mathf.Approximately(groundNormal.y, 1) && groundNormal.y != 0 && isTouchingSlope && prevGroundNormal != groundNormal)
        {
            Debug.Log("Added correction jump on slope");
            gravity *= gravityMultiplyerOnSlideChange;
        }
        
        if (!Mathf.Approximately(groundNormal.y, 1) && groundNormal.y != 0 && currentSurfaceAngle > maxClimbableSlopeAngle)
        {
            Debug.Log("Slope angle too high, character is sliding");
            if (currentSurfaceAngle > 0f && currentSurfaceAngle <= 30f) gravity = globalDown * (gravityMultiplierIfUnclimbableSlope * -Physics.gravity.y);
            else if (currentSurfaceAngle > 30f && currentSurfaceAngle <= 89f) gravity = globalDown * gravityMultiplierIfUnclimbableSlope / 2f * -Physics.gravity.y;
        }
        
        if (isTouchingWall && rb.linearVelocity.y < 0) gravity *= frictionAgainstWall;
        
        rb.AddForce(gravity);
    }

    private void OnDrawGizmos()
    {
        Vector3 bottomPos = transform.position - new Vector3(0, originalColliderHeight / 2f, 0);
        Vector3 topWallPos = new Vector3(transform.position.x, transform.position.y + wallCheckStartDistance,
            transform.position.z);

        // --- 지면 & 경사면 체크 ---
        // 지면 체크: 평소엔 파란색, 감지 시 노란색
        Gizmos.color = isGrounded ? Color.yellow : Color.blue;
        Gizmos.DrawWireSphere(bottomPos, groundCheckerThreshold);

        // 경사면 체크: 평소엔 초록색, 감지 시 빨간색
        Gizmos.color = isTouchingSlope ? Color.red : Color.green;
        Gizmos.DrawWireSphere(bottomPos, slopeCheckerThreshold);

        // --- 방향 벡터 시각화 ---
        // 이 벡터들은 항상 상태를 나타내므로 색상을 고정합니다.
        Gizmos.color = Color.blue; // Local Forward
        Gizmos.DrawLine(transform.position, transform.position + forward * 2f);
        Gizmos.color = Color.cyan; // Global Forward
        Gizmos.DrawLine(transform.position, transform.position + globalForward * 2f);
        Gizmos.color = Color.red; // Local Down
        Gizmos.DrawLine(transform.position, transform.position + down * 2f);
        Gizmos.color = Color.magenta; // Global Down
        Gizmos.DrawLine(transform.position, transform.position + globalDown * 2f);

        // --- 벽 체크 ---
        // 8방향으로 레이캐스트를 직접 그려서 감지된 방향만 빨간색으로 표시
        for (int i = 0; i < 8; i++)
        {
            float angle = i * 45f;
            Vector3 direction = Quaternion.AngleAxis(angle, transform.up) * globalForward;

            RaycastHit wallHit;
            if (Physics.Raycast(topWallPos, direction, out wallHit, wallCheckerThreshold, groundMask))
            {
                // 벽 감지 시: 빨간색으로 충돌 지점까지만 표시
                Gizmos.color = Color.red;
                Gizmos.DrawLine(topWallPos, wallHit.point);
            }
            else
            {
                // 미감지 시: 검은색으로 최대 거리까지 표시
                Gizmos.color = Color.black;
                Gizmos.DrawLine(topWallPos, topWallPos + direction * wallCheckerThreshold);
            }
        }
    }
}