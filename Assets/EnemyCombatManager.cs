using UnityEngine;

public class EnemyCombatManager : MonoBehaviour, IDamageable
{
    public void TakeDamage(float damage)
    {
        Debug.Log(this.gameObject.name + " taking damage");
    }
}
