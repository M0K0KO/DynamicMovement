using UnityEngine;

public struct HitEventArgs
{
    public GameObject Attacker;
    public GameObject Victim;
    public float Damage;
    public Vector3 HitPoint;
}
