using UnityEngine;
using System.Collections;

public class ReturnToPool : MonoBehaviour
{
    private ParticleSystem ps;
    private float maxLifetime;

    void Awake()
    {
        ParticleSystem[] particleSystems = GetComponentsInChildren<ParticleSystem>();
        foreach (ParticleSystem system in particleSystems)
        {
            if (system.main.duration + system.main.startLifetime.constantMax > maxLifetime)
            {
                maxLifetime = system.main.duration + system.main.startLifetime.constantMax;
            }
        }
    }

    void OnEnable()
    {
        StartCoroutine(DeactivateRoutine());
    }

    IEnumerator DeactivateRoutine()
    {
        yield return new WaitForSeconds(maxLifetime);

        gameObject.SetActive(false);
    }
}