using UnityEngine;
using System.Collections.Generic;
// 하나의 위치/회전 값을 담을 구조체
[System.Serializable]
public struct VFXTransformData
{
    public Vector3 localPosition;
    public Vector3 localEulerAngles;
    public float startTime;
}

[CreateAssetMenu(fileName = "VFXSequenceData", menuName = "VFX/VFX Sequence Data")]
public class VFXSequenceData : ScriptableObject
{
    public string sequenceName; 
    public List<VFXTransformData> sequence; 
}