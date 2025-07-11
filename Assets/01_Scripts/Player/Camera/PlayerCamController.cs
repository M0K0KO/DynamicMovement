using System.Collections;
using Unity.Cinemachine;
using UnityEngine;

public class PlayerCamController : MonoBehaviour
{
    private CinemachineCamera cam;
    private StateManager playerStateManager;
    
    private void Awake()
    {
        cam = GetComponent<CinemachineCamera>();
    }

    private void Start()
    {
        playerStateManager = cam.Follow.GetComponentInParent<StateManager>();
        playerStateManager.OnDashStarted += OnDashStart_CamEffect;
        playerStateManager.OnDashEnded += OnDashEnd_CamEffect;
    }

    private void OnDestroy()
    {
        if (playerStateManager != null)
        {
            playerStateManager.OnDashStarted -= OnDashStart_CamEffect;
            playerStateManager.OnDashEnded -= OnDashEnd_CamEffect;
        }
    }

    private void OnDashStart_CamEffect()
    {
        Debug.Log("[PlayerCamController] Dash camera effect started!");
    }

    private void OnDashEnd_CamEffect()
    {
        Debug.Log("[PlayerCamController] Dash ended, restoring camera!");
    }
}
