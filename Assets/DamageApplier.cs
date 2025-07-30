using System;
using System.Collections.Generic;
using UnityEngine;

public class DamageApplier : MonoBehaviour
{
    private List<IDamageable> _hitTargets;

    private void Awake()
    {
        _hitTargets = new List<IDamageable>();
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
            target.TakeDamage(0);
            _hitTargets.Add(target);
        }
    }
}