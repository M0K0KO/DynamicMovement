using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PlayerVFXManager : MonoBehaviour
{
    private PlayerManager playerManager;
    
    public List<VFXSequenceData> vfxSequences;
    private Dictionary<string, VFXSequenceData> vfxSequenceDictionary;

    private Dictionary<GameObject, string> activeVFXs;
    
    
    private void Awake()
    {
        playerManager = GetComponent<PlayerManager>();
        activeVFXs = new Dictionary<GameObject, string>();
        vfxSequenceDictionary = vfxSequences.ToDictionary(data => data.sequenceName, data => data);
    }

    public void PlayVFX(string eventData)
    {
        string[] parts = eventData.Split(':');
        if (parts.Length != 4)
        {
            Debug.LogError("이벤트 포맷 오류! \"풀태그:시퀀스이름:인덱스:추적여부(1/0)\" 형식으로 입력하세요.");
            return;
        }

        string poolTag = parts[0];
        string sequenceName = parts[1];
        if (!int.TryParse(parts[2], out int index)) {  return; }
        if (!int.TryParse(parts[3], out int parentFlag)) { Debug.LogError("추적여부 플래그 오류!"); return; }

        if (!vfxSequenceDictionary.TryGetValue(sequenceName, out VFXSequenceData sequenceData)) { Debug.Log("Sequence Data Not Found"); return; }
        if (index < 0 || index >= sequenceData.sequence.Count) { Debug.Log("VFX Index Out Of Range"); return; }

        VFXTransformData transformData = sequenceData.sequence[index];
        Vector3 localPos = transformData.localPosition;
        Quaternion localRot = Quaternion.Euler(transformData.localEulerAngles);
        float startTime = transformData.startTime;

        GameObject vfxInstance = null;
        
        if (parentFlag == 1)
        {
            vfxInstance = VFXPoolManager.Instance.GetFromPool(poolTag, localPos, localRot, transform, startTime);
        }
        else
        {
            Vector3 worldPos = transform.TransformPoint(localPos);
            Quaternion worldRot = transform.rotation * localRot;
            vfxInstance = VFXPoolManager.Instance.GetFromPool(poolTag, worldPos, worldRot, null, startTime);
        }
        
        if (vfxInstance != null && !activeVFXs.ContainsKey(vfxInstance))
        {
            activeVFXs.Add(vfxInstance, poolTag); 
        }
    }

    public void DisableAllVFX()
    {
        
        List<GameObject> keys = new List<GameObject>(activeVFXs.Keys);
        
        foreach (GameObject vfx in keys)
        {
            if (vfx == null)
            {
                Debug.Log("VFX is Null");
                continue;
            }
            
            string tag = activeVFXs[vfx];
            if (tag == "GroundImpact1") continue;
            
            VFXPoolManager.Instance.ReturnToPool(tag, vfx);
        }

        activeVFXs.Clear();
    }
}
