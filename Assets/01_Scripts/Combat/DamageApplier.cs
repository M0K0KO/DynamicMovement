using System;
using System.Collections.Generic;
using Unity.Cinemachine;
using UnityEngine;

public class DamageApplier : MonoBehaviour
{
    [SerializeField] private PlayerManager playerManager;
    private List<IDamageable> _hitTargets;

    private CinemachineImpulseSource impulseSource;
    
    private void Awake()
    {
        _hitTargets = new List<IDamageable>();
        impulseSource = GetComponent<CinemachineImpulseSource>();
    }

    private void OnEnable()
    {
        _hitTargets.Clear();
    }
    
    private void OnDisable()
    {
        _hitTargets.Clear();
    }

    private void OnTriggerEnter(Collider other)
    {
        IDamageable target = other.GetComponent<IDamageable>();

        if (target != null && !_hitTargets.Contains(target))
        {
            HitstopManager.Instance.StartHitstop(0.035f);

            Vector3 attackDirection = other.transform.position - other.ClosestPoint(transform.position);
            Vector3 impulseVelocity = attackDirection.normalized;
            impulseSource.GenerateImpulseWithVelocity(impulseVelocity * 0.2f);
            
            target.TakeDamage(playerManager.gameObject, other.ClosestPoint(transform.position), 0);
            _hitTargets.Add(target);
        }
    }
}