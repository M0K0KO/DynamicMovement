using UnityEngine;

public class VFXStartTimeSetter : MonoBehaviour
{
    [Tooltip("이 파티클을 시작하고 싶은 시간(초)")]
    public float startTime = 0f;

    private void OnEnable()
    {
        if (startTime > 0f)
        {
            var particleSystems = GetComponentsInChildren<ParticleSystem>();
            
            foreach (var ps in particleSystems)
            {
                ps.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
                ps.Simulate(startTime, true, true);
                ps.Play();
            }
        }
    }
}