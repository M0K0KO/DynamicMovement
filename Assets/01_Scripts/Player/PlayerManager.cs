using System;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.VFX;

public class PlayerManager : MonoBehaviour
{
    public Rigidbody rb { get; private set; }
    public CapsuleCollider playerCollider { get; private set; }
    public Camera playerCam { get; private set; }
    public Animator animator { get; private set; }

    [SerializeField] public PlayerStateManager StateManager;
    [SerializeField] public PlayerInputManager InputManager;
    [SerializeField] public PlayerStateMachine StateMachine;
    [SerializeField] public PlayerLocomotionManager LocomotionManager;
    [SerializeField] public PlayerVFXManager VFXManager;
    [SerializeField] public PlayerCombatManager CombatManager;

    public float moveLerpSpeed = 40f;
    
    public float walkRotationSpeed = 11f;
    public float walkSpeed = 4f;

    public float lockOnRunSpeed = 7.5f;
    public float runRotationSpeed = 7f;
    public float runSpeed = 10f;

    public float jumpSpeed = 10f;
    public float jumpDuration = 0.2f;
    
    public float airAcceleration = 10f;
    public float airRotationSpeed = 1.5f;
    public float maxAirSpeed = 10f;

    public float dashDuration = 0.4f;
    public float dashSpeed = 17f;
    public float lockOnDashDuration = 0.5f;
    public float lockOnDashSpeed = 13f;

    public float dashAttackSpeed = 40f;

    public float enemyDetectionRange = 15f;
    

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        playerCollider = GetComponent<CapsuleCollider>();
        playerCam = Camera.main;
        animator = GetComponent<Animator>();
        VFXManager = GetComponent<PlayerVFXManager>();
    }
}