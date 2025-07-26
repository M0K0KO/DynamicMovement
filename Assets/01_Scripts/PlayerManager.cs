using System;
using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    public Rigidbody rb { get; private set; }
    public CapsuleCollider playerCollider { get; private set; }

    public PlayerStateManager StateManager { get; private set; }
    public PlayerInputManager InputManager { get; private set; }
    public PlayerStateMachine StateMachine { get; private set; }

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        playerCollider = GetComponent<CapsuleCollider>();
        
        StateManager = GetComponent<PlayerStateManager>();
        InputManager = GetComponent<PlayerInputManager>();
        StateMachine = GetComponent<PlayerStateMachine>();
        
    }
}
