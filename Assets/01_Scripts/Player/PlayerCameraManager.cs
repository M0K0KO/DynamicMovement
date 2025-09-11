using System;
using System.Collections;
using Unity.Cinemachine;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerCameraManager : MonoBehaviour
{
    [SerializeField] private CinemachineStateDrivenCamera playerCam;
    public CinemachineCamera normalCam;
    public CinemachineCamera dashCam;
    public CinemachineCamera lockOnCam;
    public CinemachineCamera lockOnDashCam;
    public Animator playerCamController { get; private set; }
    
    
    public PlayerManager playerManager;

    [SerializeField] private float minFov;
    [SerializeField] private float maxFov;

    [SerializeField] private float fovLerpSpeed;

    [SerializeField] private float maxSpeed = 13f;

    private string currentCamState;

    private void Awake()
    {
        playerCamController = playerCam.GetComponent<Animator>();
    }

    private void Start()
    {
        HandleDefaultCameraState();
    }

    private void Update()
    {
        AdjustFOV();
    }

    private void AdjustFOV()
    {
        Vector3 velocity = new Vector3(playerManager.rb.linearVelocity.x, 0, playerManager.rb.linearVelocity.z);
        float currentSpeed = velocity.magnitude;
        float speedRatio = Mathf.Clamp01(currentSpeed / maxSpeed);

        float targetFov = Mathf.Lerp(minFov, maxFov, speedRatio);


        normalCam.Lens.FieldOfView = Mathf.Lerp(normalCam.Lens.FieldOfView, targetFov, fovLerpSpeed * Time.deltaTime);
        dashCam.Lens.FieldOfView = Mathf.Lerp(normalCam.Lens.FieldOfView, targetFov, fovLerpSpeed * Time.deltaTime);
        lockOnCam.Lens.FieldOfView = Mathf.Lerp(lockOnCam.Lens.FieldOfView, targetFov, fovLerpSpeed * Time.deltaTime);
        lockOnDashCam.Lens.FieldOfView = Mathf.Lerp(lockOnDashCam.Lens.FieldOfView, targetFov, fovLerpSpeed * Time.deltaTime);
    }

    public void HandleDefaultCameraState()
    {
        if (playerManager.StateMachine.isLockedOn && currentCamState != "LockOnCam")
        {
            ChangeLockOnCameraState("LockOnCam", lockOnCam);
        }
        else if (!playerManager.StateMachine.isLockedOn && currentCamState != "NormalCam")
        {
            ChangeCameraState("NormalCam");
        }
    }

    public void ChangeLockOnCameraState(string state, CinemachineCamera cam)
    {
        GameObject targetEnemy = playerManager.StateMachine.lockOnTarget;
        cam.Target.LookAtTarget = targetEnemy.transform;

        playerCamController.Play(state);
    }

    public void ChangeCameraState(string state)
    {
        playerCamController.Play(state);
    }
}