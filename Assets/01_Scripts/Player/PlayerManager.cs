using System;
using System.Collections.Generic;
using UnityEngine;

// Holds Player's Scripts, Works as an interface
[RequireComponent(typeof(PlayerInputManager))]
public class PlayerManager : MonoBehaviour
{
    public PlayerInputManager playerInputManager { get; private set; }
    public Rigidbody rb { get; private set; }
    public CharacterController controller { get; private set; }
    public Collider playerCollider { get; private set; }
    public Animator animator { get; private set; }
    public DashTrail dashTrail { get; private set; }
    public PlayerVFXManager vfxManager { get; private set; }
    
    public GameObject weapon;
    public Transform camFollowTarget;

    public float detectionRadius { get; private set; } = 20f;
    public float rotationSpeed { get; private set; } = 15f;
    public float sprintSpeed { get; private set; } = 15f;
    public float dashForce { get; private set; } = 40f;
    public float dashDuration { get; private set; } = 0.5f;


    #region Initialization
    private void Awake()
    {
        playerInputManager = GetComponent<PlayerInputManager>();
        rb = GetComponent<Rigidbody>();
        controller = GetComponent<CharacterController>();
        playerCollider = GetComponent<Collider>();
        animator = GetComponent<Animator>();
        dashTrail = GetComponent<DashTrail>();
        vfxManager = GetComponent<PlayerVFXManager>();
    }
    #endregion
}
