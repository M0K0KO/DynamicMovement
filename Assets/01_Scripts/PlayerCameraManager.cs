using Unity.Cinemachine;
using UnityEngine;

public class PlayerCameraManager : MonoBehaviour
{
    [SerializeField] private CinemachineCamera playerCam;
    [SerializeField] private PlayerManager playerManager;

    [SerializeField] private float minFov;
    [SerializeField] private float maxFov;

    [SerializeField] private float fovLerpSpeed;

    private float maxSpeed = 17f;
    
    private void Update()
    {
        Vector3 velocity = new Vector3(playerManager.rb.linearVelocity.x , 0, playerManager.rb.linearVelocity.z);
        float currentSpeed = velocity.magnitude;
        float speedRatio = Mathf.Clamp01(currentSpeed / maxSpeed);

        float targetFov = Mathf.Lerp(minFov, maxFov, speedRatio);

        playerCam.Lens.FieldOfView = Mathf.Lerp(playerCam.Lens.FieldOfView, targetFov, fovLerpSpeed * Time.deltaTime);

    }
}
