using System;
using UnityEngine;

public class EnemyManager : MonoBehaviour
{
    public EnemyCombatManager combatManager { get; private set; }
    public EnemyVFXManager vfxManager { get; private set; }
    public EnemyLocomotionManager locomotionManager { get; private set; }


    public CharacterController cc { get; private set; }
    public Animator animator { get; private set; }

    private void Awake()
    {
        combatManager = GetComponent<EnemyCombatManager>();
        vfxManager = GetComponent<EnemyVFXManager>();
        locomotionManager = GetComponent<EnemyLocomotionManager>();
        
        cc = GetComponent<CharacterController>();
        animator = GetComponent<Animator>();
    }
}
