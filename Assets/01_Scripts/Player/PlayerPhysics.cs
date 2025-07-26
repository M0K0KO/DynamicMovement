using System;
using System.Numerics;
using Unity.Cinemachine;
using UnityEngine;
using Matrix4x4 = UnityEngine.Matrix4x4;
using Quaternion = UnityEngine.Quaternion;
using Vector3 = UnityEngine.Vector3;

public class PlayerPhysics : MonoBehaviour
{
    private PlayerManager player;


    [Header("Boxcast Property")] 
    [SerializeField] private Vector3 offset;
    [SerializeField] private Vector3 boxSize;
    [SerializeField] private float maxDistance;
    [SerializeField] private LayerMask groundLayer;
    
    
    [Header("Debug")]
    [SerializeField] private bool drawGizmo;
    
    public bool IsGrounded()
    {
        return Physics.BoxCast(transform.position + offset, boxSize, Vector3.down, transform.rotation, maxDistance, groundLayer);
    }
    
    public void MovePlayer(float speed, Vector3 additiveSpeed = default(Vector3))
    {
        
        Vector3 camForwardVector = new Vector3(player.playerCam.transform.forward.x, 0, player.playerCam.transform.forward.z).normalized;
        Vector3 camRightVector = new Vector3(player.playerCam.transform.right.x, 0, player.playerCam.transform.right.z).normalized;
        Vector3 moveDirection = camForwardVector * player.playerInputManager.moveInputDirection.z + camRightVector *player.playerInputManager.moveInputDirection.x;

        Quaternion targetRotation = Quaternion.LookRotation(transform.forward);

        if(moveDirection != Vector3.zero)
        {
            targetRotation = Quaternion.LookRotation(moveDirection);
        }
        
        player.transform.rotation = Quaternion.Lerp(player.transform.rotation, targetRotation, player.rotationSpeed * Time.deltaTime);

        Vector3 horizontalMovement = moveDirection * speed;
        player.controller.Move(horizontalMovement);
    }

    public Vector3 groundNormal()
    {
        RaycastHit groundHit;
        if (Physics.Raycast(transform.position + offset, Vector3.down, out groundHit, maxDistance, groundLayer))
        {
            return groundHit.normal;
        }
        else
        {
            return Vector3.zero;
        }
    }

    public Vector3 projectedDirection(Vector3 direction)
    {
        if (groundNormal() != Vector3.zero) return Vector3.ProjectOnPlane(direction, groundNormal());
        else return direction;
    }
    
    
    //-------------------------------------------------------------------------------------------------------------------------------
    
    private void OnDrawGizmos()
    {
        // BoxCast의 경로와 최종 위치를 계산
        Vector3 boxOrigin = transform.position;
        Vector3 castDirection = -transform.up;
        RaycastHit hit;

        // isGrounded와 동일한 조건으로 BoxCast를 실행해서 충돌 정보를 얻음
        bool isHit = Physics.BoxCast(boxOrigin, boxSize / 2, castDirection, out hit, transform.rotation, maxDistance, groundLayer);

        // 기즈모 색상 설정: 감지되었을 때는 초록색, 아닐 때는 빨간색
        Gizmos.color = isHit ? Color.green : Color.red;

        // 기즈모의 위치, 회전, 크기를 BoxCast와 일치시키기 위한 매트릭스 설정
        // BoxCast는 중심이 아닌 전체 크기를 사용하지만, 기즈모 큐브는 중심과 크기를 사용하므로 boxSize를 2로 나눔
        Matrix4x4 rotationMatrix = Matrix4x4.TRS(transform.position, transform.rotation, Vector3.one);
        Gizmos.matrix = rotationMatrix;

        // BoxCast가 이동한 최종 위치에 와이어 큐브를 그림
        // 충돌했다면 충돌 지점에, 아니라면 최대 거리에 그림
        Vector3 finalPosition = castDirection.normalized * (isHit ? hit.distance : maxDistance);
        Gizmos.DrawWireCube(finalPosition, boxSize);
    }
}
