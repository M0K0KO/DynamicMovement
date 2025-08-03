using System;
using UnityEngine;

public class EnemyLocomotionManager : MonoBehaviour
{
    private EnemyManager enemyManager; 
        
    public readonly int IDLE_ANIMATION = Animator.StringToHash("EnemyIdle");

    public readonly int HIT_ANIMATION = Animator.StringToHash("EnemyHit");
    public readonly int GROUNDED_PARAM = Animator.StringToHash("Grounded"); // 0 for air, 1 for ground
    public readonly int HITHORIZONTAL_PARAM = Animator.StringToHash("HitHorizontal"); 
    public readonly int HITVERTICAL_PARAM = Animator.StringToHash("HitVertical");


    private void Awake()
    {
        enemyManager = GetComponent<EnemyManager>();
    }

    private void Update()
    {
        UpdateAnimationParams();
    }

    private void OnAnimatorMove()
    {
        Vector3 deltaPosition = enemyManager.animator.deltaPosition;
        enemyManager.cc.Move(deltaPosition);
        transform.rotation = enemyManager.animator.rootRotation;
    }

    public void PlayHitAnimation(HitEventArgs args)
    {
        Vector3 hitPoint = new Vector3(args.HitPoint.x, 0f, args.HitPoint.z);
        Vector3 currentPosition = new Vector3(transform.position.x, 0f, transform.position.z);

        Vector3 hitDirection = (hitPoint - currentPosition).normalized;
        
        SetHitAnimationParams(hitDirection);
        enemyManager.animator.CrossFadeInFixedTime(HIT_ANIMATION, 0f);
    }

    private void UpdateAnimationParams()
    {
        enemyManager.animator.SetFloat(GROUNDED_PARAM, 1f);
        // GROUNDED_PARAM = enemyManager.stateManager.isGrounded;
    }

    private void SetHitAnimationParams(Vector3 hitDirection)
    {
        enemyManager.animator.SetFloat(HITHORIZONTAL_PARAM, hitDirection.x);
        enemyManager.animator.SetFloat(HITVERTICAL_PARAM, hitDirection.z);
    }
}
