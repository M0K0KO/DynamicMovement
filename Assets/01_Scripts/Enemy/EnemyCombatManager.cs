using UnityEngine;

public class EnemyCombatManager : MonoBehaviour, IDamageable
{
    private EnemyManager enemyManager;

    private void Awake()
    {
        enemyManager = GetComponent<EnemyManager>();
    }
    
    public void TakeDamage(GameObject attacker, Vector3 hitPoint,  float damage)
    {
        Debug.Log(this.gameObject.name + " taking damage");

        var hitEventArgs = new HitEventArgs
        {
            Attacker = attacker,
            HitPoint = hitPoint,
            Damage = damage
        };
        WorldEventManager.TriggerOnHitEvent(hitEventArgs);

        var vfxData = new VFXTransformData
        {
            localPosition = transform.InverseTransformPoint(hitPoint),
            localEulerAngles = new Vector3(0,0,0),
            startTime = 0f,
        };
        
        
        enemyManager.vfxManager.PlayVFX(vfxData, "PunchHit", false);
        enemyManager.locomotionManager.PlayHitAnimation(hitEventArgs);
    }
}
