using System;
using UnityEngine;

public class WorldEventManager : MonoBehaviour
{
    public static event Action<HitEventArgs> OnHit;

    public static void TriggerOnHitEvent(HitEventArgs args)
    {
        OnHit?.Invoke(args);
    }
}
