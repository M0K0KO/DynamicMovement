using System;
using UnityEngine;

// Spawns Player and it's Attachments
public class PlayerSpawnPoint : MonoBehaviour
{
    [SerializeField]
    private GameObject playerPrefab;

    [SerializeField] 
    private GameObject playerCamera;

    private void Awake()
    {
        if (playerPrefab == null) Debug.LogError("playerPrefab unassigned");
        if (playerCamera == null) Debug.LogError("playerCamera unassigned");
    }

    private void Start()
    {
        Instantiate(playerPrefab, transform.position, Quaternion.identity);
        Instantiate(playerCamera, transform.position, Quaternion.identity);
    }
}
