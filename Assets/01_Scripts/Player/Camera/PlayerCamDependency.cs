using System;
using Unity.Cinemachine;
using UnityEngine;

// Connects Player and PlayerCam Right After Spawn
public class PlayerCamDependency : MonoBehaviour
{
     CinemachineCamera cam;

     private void Awake()
     {
          cam = GetComponent<CinemachineCamera>();
     }

     private void Start()
     {
          PlayerManager player = FindAnyObjectByType<PlayerManager>();
          if (player != null && player.camFollowTarget != null)
          {
               cam.Follow = player.camFollowTarget;
          }
          else
          {
               Debug.LogError("Player Not Found");
          }
     }
}
