using System;
using System.Collections;
using Unity.Cinemachine;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerCameraManager : MonoBehaviour
{
    [SerializeField] private CinemachineStateDrivenCamera playerCam;
    [SerializeField] private CinemachineCamera normalCam;
    [SerializeField] private CinemachineCamera lockOnCam;
    [SerializeField] private Animator playerCamController;
    [SerializeField] private PlayerManager playerManager;

    [SerializeField] private float minFov;
    [SerializeField] private float maxFov;

    [SerializeField] private float fovLerpSpeed;

    private float maxSpeed = 17f;

    private string currentCamState;

    private void Awake()
    {
    }

    private void Update()
    {
        AdjustFOV();
        HandleLockOn();
    }

    private void AdjustFOV()
    {
        Vector3 velocity = new Vector3(playerManager.rb.linearVelocity.x , 0, playerManager.rb.linearVelocity.z);
        float currentSpeed = velocity.magnitude;
        float speedRatio = Mathf.Clamp01(currentSpeed / maxSpeed);

        float targetFov = Mathf.Lerp(minFov, maxFov, speedRatio);

        
        normalCam.Lens.FieldOfView = Mathf.Lerp(normalCam.Lens.FieldOfView, targetFov, fovLerpSpeed * Time.deltaTime);
        lockOnCam.Lens.FieldOfView = Mathf.Lerp(lockOnCam.Lens.FieldOfView, targetFov, fovLerpSpeed * Time.deltaTime);
    }

    private void HandleLockOn()
    {
        if (playerManager.StateMachine.isLockedOn)
        {
            GameObject targetEnemy = playerManager.StateMachine.lockOnTarget;
            lockOnCam.Target.LookAtTarget = targetEnemy.transform;
            
            playerCamController.Play("LockOnCam");
        }
        else
        {
            playerCamController.Play("NormalCam");
        }
    }
}
