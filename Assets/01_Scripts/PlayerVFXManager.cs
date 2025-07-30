using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PlayerVFXManager : MonoBehaviour
{
    public List<VFXSequenceData> vfxSequences;

    // (선택사항) 빠른 접근을 위한 딕셔너리
    private Dictionary<string, VFXSequenceData> vfxSequenceDictionary;

    private void Awake()
    {
        // 리스트에 있는 데이터를 딕셔너리로 변환하여 쉽게 찾을 수 있도록 준비합니다.
        // vfxTag를 Key로 사용합니다.
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
        if (!int.TryParse(parts[2], out int index)) { /* ... */ return; }
        if (!int.TryParse(parts[3], out int parentFlag)) { Debug.LogError("추적여부 플래그 오류!"); return; }

        // ... (시퀀스 데이터 찾는 로직은 기존과 동일) ...
        if (!vfxSequenceDictionary.TryGetValue(sequenceName, out VFXSequenceData sequenceData)) { /* ... */ return; }
        if (index < 0 || index >= sequenceData.sequence.Count) { /* ... */ return; }

        VFXTransformData transformData = sequenceData.sequence[index];
        Vector3 localPos = transformData.localPosition;
        Quaternion localRot = Quaternion.Euler(transformData.localEulerAngles);
        float startTime = transformData.startTime;

        // ✨ [수정] '추적여부(parentFlag)' 값에 따라 다르게 처리합니다.
        if (parentFlag == 1)
        {
            // 1: 플레이어를 추적 (자식으로 만듦)
            // 로컬 좌표와 부모(this.transform)를 그대로 전달합니다.
            VFXPoolManager.Instance.GetFromPool(poolTag, localPos, localRot, this.transform, startTime);
        }
        else
        {
            // 0: 월드 공간에 고정
            // 로컬 좌표를 월드 좌표로 변환하고, 부모(parent)는 null로 전달합니다.
            Vector3 worldPos = transform.TransformPoint(localPos);
            Quaternion worldRot = transform.rotation * localRot;
            VFXPoolManager.Instance.GetFromPool(poolTag, worldPos, worldRot, null, startTime);
        }
    }
}
