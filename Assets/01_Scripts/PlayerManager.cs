using System;
using Unity.Cinemachine;
using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    public Rigidbody rb { get; private set; }
    public CapsuleCollider playerCollider { get; private set; }
    public Camera playerCam { get; private set; }
    public Animator animator { get; private set; }

    [SerializeField] public PlayerStateManager StateManager;
    [SerializeField] public PlayerInputManager InputManager;
    [SerializeField] public PlayerStateMachine StateMachine;

    public float moveLerpSpeed = 5f;
    
    public float walkRotationSpeed = 8f;
    public float walkSpeed = 4.5f;
    
    public float runRotationSpeed = 6f;
    public float runSpeed = 10f;

    public float jumpSpeed = 12f;
    public float jumpDuration = 0.3f;
    
    public float airAcceleration = 10f;
    public float airRotationSpeed = 2f;
    public float maxAirSpeed = 12f;

    public float dashDuration = 0.3f;
    public float dashSpeed = 20f;
    

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        playerCollider = GetComponent<CapsuleCollider>();
        playerCam = Camera.main;
        animator = GetComponent<Animator>();
    }
}