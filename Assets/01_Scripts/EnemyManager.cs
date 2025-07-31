using System;
using UnityEngine;

public class EnemyManager : MonoBehaviour
{
    public EnemyCombatManager combatManager { get; private set; }
    public EnemyVFXManager vfxManager { get; private set; }

    private void Awake()
    {
        combatManager = GetComponent<EnemyCombatManager>();
        vfxManager = GetComponent<EnemyVFXManager>();
    }
}
