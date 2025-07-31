using System;
using UnityEngine;

public class EnemyVFXManager : MonoBehaviour
{
    private EnemyManager enemyManager;

    private void Awake()
    {
        enemyManager = GetComponent<EnemyManager>();
    }
    
    public void PlayVFX(VFXTransformData data, string poolTag, bool parent)
    {
        Vector3 localPos = data.localPosition;
        Quaternion localRot = Quaternion.Euler(data.localEulerAngles);
        float startTime = data.startTime;

        if (parent)
        {
            VFXPoolManager.Instance.GetFromPool(poolTag, localPos, localRot, transform, startTime);
        }
        else
        {
            Vector3 worldPos = transform.TransformPoint(localPos);
            Quaternion worldRot = transform.rotation * localRot;
            VFXPoolManager.Instance.GetFromPool(poolTag, worldPos, worldRot, null, startTime);
        }
    }
}
