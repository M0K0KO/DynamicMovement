using UnityEngine;

public interface IDamageable
{
    public void TakeDamage(GameObject attacker, Vector3 hitPoint,  float damage);
}
