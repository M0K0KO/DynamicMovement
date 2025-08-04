using System;
using Unity.VisualScripting;
using UnityEngine;

// animation Play 담당
public class PlayerLocomotionManager : MonoBehaviour
{
    private PlayerManager playerManager;
    private Animator animator;
    
    [Header("Animation Smoothing")]
    [Range(0, 20f)]
    public float animationSmoothTime = 10f;
    
    public readonly int SPEED_PARAM = Animator.StringToHash("Speed"); // 0 : idle, 0.5 : walk, 1.0 : run
    public readonly int HORIZONTAL_PARAM = Animator.StringToHash("Horizontal"); // 0~1
    public readonly int VERTICAL_PARAM = Animator.StringToHash("Vertical"); // 0~1
    public readonly int DASHHORIZONTAL_PARAM = Animator.StringToHash("DashHorizontal"); // 0~1
    public readonly int DASHVERTICAL_PARAM = Animator.StringToHash("DashVertical"); // 0~1
    public readonly int ISLOCKEDON_PARAM = Animator.StringToHash("isLockedOn"); // 0 : not locked, 1 : locked

    public readonly int GROUNDEDMOVEMENT_ANIMATION = Animator.StringToHash("GroundedMovement");
    public readonly int JUMP_0_START = Animator.StringToHash("Player_Jump_0_Start");
    public readonly int JUMP_0_LOOP= Animator.StringToHash("Player_Jump_0_Loop");
    public readonly int JUMP_0_END = Animator.StringToHash("Player_Jump_0_End");
    public readonly int DASH_ANIMATION = Animator.StringToHash("Dash");

    public readonly int PLAYER_COMBO_01_1 = Animator.StringToHash("Player_Combo_01-1");
    public readonly int PLAYER_COMBO_01_2 = Animator.StringToHash("Player_Combo_01-2");
    public readonly int PLAYER_COMBO_01_3 = Animator.StringToHash("Player_Combo_01-3");
    public readonly int PLAYER_COMBO_01_4 = Animator.StringToHash("Player_Combo_01-4");
    public readonly int NEXT_COMBO_TRIGGER = Animator.StringToHash("NextCombo");
    
    public readonly int PLAYER_CHARGE_ATTACK_START = Animator.StringToHash("Player_Charge_Attack_Start");
    public readonly int PLAYER_CHARGE_ATTACK_LOOP = Animator.StringToHash("Player_Charge_Attack_Loop");
    public readonly int PLAYER_CHARGE_ATTACK_END = Animator.StringToHash("Player_Charge_Attack_End");

    public readonly int PLAYER_DASH_ATTACK = Animator.StringToHash("Player_DashAttack");

    
    private void Awake()
    {
        playerManager = GetComponent<PlayerManager>();
        animator = playerManager.animator;
    }

    private void LateUpdate()
    {
        float targetHorizontal = playerManager.InputManager.moveInput.x;
        float targetVertical = playerManager.InputManager.moveInput.y;

        float targetSpeed;
        if (playerManager.InputManager.moveInput == Vector2.zero)
        {
            targetSpeed = 0f;
        }
        else
        {
            targetSpeed = playerManager.StateMachine.isRunning ? 1f : 0.5f;
        }
        
        animator.SetFloat(ISLOCKEDON_PARAM, playerManager.StateMachine.isLockedOn ? 1f : 0f);
        LerpUpdateParam(HORIZONTAL_PARAM, targetHorizontal);
        LerpUpdateParam(VERTICAL_PARAM, targetVertical);
        LerpUpdateParam(SPEED_PARAM, targetSpeed);
    }

    private void OnAnimatorMove()
    {
        if (playerManager.StateMachine.currentState == playerManager.StateMachine.attackState 
            || playerManager.StateMachine.currentState == playerManager.StateMachine.chargeAttackState
            || playerManager.StateMachine.currentState == playerManager.StateMachine.dashAttackState)
        {
            Vector3 deltaPosition = animator.deltaPosition;

            if (playerManager.StateMachine.isLockedOn &&
                Vector3.Distance(playerManager.StateMachine.lockOnTarget.transform.position,
                    transform.position) < 1.5f)
            {
                Vector3 localDelta = transform.InverseTransformDirection(deltaPosition);
                localDelta.z = 0;
                deltaPosition = transform.TransformDirection(localDelta);
            }
            
            Vector3 targetPosition = playerManager.rb.position + deltaPosition;
            
            playerManager.rb.MovePosition(targetPosition);
            playerManager.rb.MoveRotation(animator.rootRotation);
        }
    }

    public void HandleMovement(LocomotionParameters parameters)
    {
        if (parameters.PerformRotation)
        {
            if (playerManager.StateMachine.isLockedOn)
            {
                HandleLockOnRotation(parameters.RotationSpeed, parameters.InstantRotation);
            }
            else
            {
                HandleNormalRotation(parameters.RotationSpeed, parameters.InstantRotation);
            }
        }

        if (parameters.PerformMovement)
        {
            ApplyMovementVelocity(parameters.MoveSpeed);
        }
    }

    public void HandleNormalRotation(float rotationSpeed, bool instant)
    {
        float targetAngle = Mathf.Atan2(playerManager.InputManager.moveInput.x, playerManager.InputManager.moveInput.y) 
            * Mathf.Rad2Deg + playerManager.playerCam.transform.eulerAngles.y;
        Quaternion targetRotation = Quaternion.Euler(0f, targetAngle, 0f);

        if (instant)
        {
            playerManager.rb.MoveRotation(targetRotation);
        }
        else
        {
            Quaternion newRotation = Quaternion.Slerp(playerManager.rb.rotation, targetRotation, 
                rotationSpeed * Time.fixedDeltaTime);
            playerManager.rb.MoveRotation(newRotation);
        }
    }
    public void HandleLockOnRotation(float rotationSpeed, bool instant)
    {
        Vector3 directionToTarget = playerManager.StateMachine.lockOnTarget.transform.position - playerManager.transform.position;
        directionToTarget.y = 0;
        directionToTarget.Normalize();
        Quaternion targetRotation = Quaternion.LookRotation(directionToTarget);

        if (instant)
        {
            playerManager.rb.MoveRotation(targetRotation);
        }
        else
        {
            Quaternion newRotation = Quaternion.Slerp(playerManager.rb.rotation, targetRotation,
                rotationSpeed * Time.fixedDeltaTime);
            playerManager.rb.MoveRotation(newRotation);
        }
    }
    public void ApplyMovementVelocity(float speed)
    {
        float verticalVelocity = playerManager.rb.linearVelocity.y;

        Vector3 finalMoveDirection = playerManager.StateManager.forward;
        Vector3 targetHorizontalVelocity = finalMoveDirection * speed;

        Vector3 currentHorizontalVelocity = playerManager.rb.linearVelocity;
        currentHorizontalVelocity.y = 0;

        Vector3 newHorizontalVelocity = Vector3.Lerp(
            currentHorizontalVelocity, 
            targetHorizontalVelocity, 
            playerManager.moveLerpSpeed * Time.fixedDeltaTime
        );

        Vector3 finalVelocity = new Vector3(newHorizontalVelocity.x, verticalVelocity, newHorizontalVelocity.z);
        playerManager.rb.linearVelocity = finalVelocity;
    }
    
    public void ApplyAirControlForce(float airAcceleration)
    {
        Vector3 camForward = playerManager.playerCam.transform.forward;
        Vector3 camRight = playerManager.playerCam.transform.right;
        camForward.y = 0;
        camRight.y = 0;
        camForward.Normalize();
        camRight.Normalize();

        Vector3 moveDirection = (camForward * playerManager.InputManager.moveInput.y + camRight * playerManager.InputManager.moveInput.x).normalized;

        playerManager.rb.AddForce(moveDirection * airAcceleration, ForceMode.Acceleration);

        Vector3 horizontalVelocity = new Vector3(playerManager.rb.linearVelocity.x, 0, playerManager.rb.linearVelocity.z);
        if (horizontalVelocity.magnitude > playerManager.maxAirSpeed)
        {
            Vector3 clampedVelocity = horizontalVelocity.normalized * playerManager.maxAirSpeed;
            playerManager.rb.linearVelocity = new Vector3(clampedVelocity.x, playerManager.rb.linearVelocity.y, clampedVelocity.z);
        }
    }


    public void PlayGroundedAnimation()
    {
        animator.CrossFadeInFixedTime(GROUNDEDMOVEMENT_ANIMATION, 0.2f);
    }
    public void PlayDashAnimation()
    {
        if (playerManager.StateMachine.isLockedOn)
        {
            animator.SetFloat(DASHHORIZONTAL_PARAM, playerManager.InputManager.moveInput.x == 0 ? 0f : playerManager.InputManager.moveInput.x > 0f ? 1f : -1f);
            animator.SetFloat(DASHVERTICAL_PARAM, playerManager.InputManager.moveInput.y == 0 ? 0f : playerManager.InputManager.moveInput.y > 0f ? 1f : -1f);
        }
        else
        {
            animator.SetFloat(DASHHORIZONTAL_PARAM, 0f);
            animator.SetFloat(DASHVERTICAL_PARAM, 1f);
        }

        animator.CrossFadeInFixedTime(DASH_ANIMATION, 0.2f);
    }
    public void PlayJumpAnimation()
    {
        animator.CrossFadeInFixedTime(JUMP_0_START, 0.2f);
    }
    public void PlayFallAnimation()
    {
        animator.CrossFadeInFixedTime(JUMP_0_LOOP, 0.2f);
    }

    public void PlayJumpEndAnimation()
    {
        animator.CrossFadeInFixedTime(JUMP_0_END, 0.2f);
    }

    public void PlayAttackAnimation()
    {
        animator.CrossFadeInFixedTime(PLAYER_COMBO_01_1, 0.2f);
    }

    public void PlayChargeAttackAnimation()
    {
        animator.CrossFadeInFixedTime(PLAYER_CHARGE_ATTACK_START, 0.2f);
    }

    public void PlayChargeAttackEndAnimation()
    {
        animator.CrossFadeInFixedTime(PLAYER_CHARGE_ATTACK_END, 0.1f);
    }

    public void PlayDashAttackAnimation()
    {
        animator.CrossFadeInFixedTime(PLAYER_DASH_ATTACK, 0.1f);
    }
    
    private void LerpUpdateParam(int paramHash, float targetValue)
    {
        float currentValue = animator.GetFloat(paramHash);

        float smoothedValue = Mathf.Lerp(currentValue, targetValue, animationSmoothTime * Time.deltaTime);

        animator.SetFloat(paramHash, smoothedValue);
    }
}

public struct LocomotionParameters
{
    public bool PerformMovement;
    public bool PerformRotation;
    public bool InstantRotation;
    public float MoveSpeed;
    public float RotationSpeed;
}
