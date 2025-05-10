using System;
using UnityEngine;

// Holds Player's Scripts, Works as an interface
[RequireComponent(typeof(PlayerInputManager))]
public class PlayerManager : MonoBehaviour
{
    public PlayerInputManager playerInputManager { get; private set; }
    public Transform camFollowTarget;

    #region Initialization
    private void Awake()
    {
        playerInputManager = GetComponent<PlayerInputManager>();
    }
    #endregion
}
