using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerVFXManager : MonoBehaviour
{
    public readonly int Dissolve = Shader.PropertyToID("_Dissolve");
    
    private PlayerManager player;
    public Material playerWeaponMaterial { get; private set; }
    public List<Transform> vfxPoints;

    public Coroutine dissolveCoroutine;
    private float playerWeaponDissolveTime = 1.5f;
    
    private void Awake()
    {
        player = GetComponent<PlayerManager>();
        playerWeaponMaterial = player.weapon.GetComponent<Renderer>().material;
        
        playerWeaponMaterial.SetFloat(Dissolve, 1f);
    }

    public void ActivateNormalComboVFX(int comboCount)
    {
        if (comboCount == 1)
        {
            Vector3 localOffset = vfxPoints[0].localPosition;
            Vector3 spawnPos = player.transform.TransformPoint(localOffset);
            Quaternion spawnRot = player.transform.rotation * vfxPoints[0].localRotation;
            ObjectPooler.Instance.SpawnFromPool("NormalCombo1", spawnPos, spawnRot);
        }
        else if (comboCount == 2)
        {
            Vector3 localOffset = vfxPoints[1].localPosition;
            Vector3 spawnPos = player.transform.TransformPoint(localOffset);
            Quaternion spawnRot = player.transform.rotation * vfxPoints[1].localRotation;
            ObjectPooler.Instance.SpawnFromPool("NormalCombo2", spawnPos, spawnRot);
        }
        else if (comboCount == 3)
        {
            Vector3 localOffset = vfxPoints[2].localPosition;
            Vector3 spawnPos = player.transform.TransformPoint(localOffset);
            Quaternion spawnRot = player.transform.rotation * vfxPoints[2].localRotation;
            ObjectPooler.Instance.SpawnFromPool("NormalCombo3", spawnPos, spawnRot);
        }
    }

    public void AdjustWeaponDissolveVFX(float targetDissolveValue, bool immediate = false)
    {
        if (dissolveCoroutine != null)
        {
            StopCoroutine(dissolveCoroutine);
        }

        if (immediate)
        {
            playerWeaponMaterial.SetFloat(Dissolve, targetDissolveValue);
        }
        else
        {
            dissolveCoroutine = StartCoroutine(DissolveCoroutine(targetDissolveValue, playerWeaponDissolveTime));
        }
    }
    
    private IEnumerator DissolveCoroutine(float targetValue, float duration)
    {
        float elapsedTime = 0f;
        float startValue = playerWeaponMaterial.GetFloat(Dissolve);

        while (elapsedTime < duration)
        {
            float t = elapsedTime / duration;
        
            float currentValue = Mathf.Lerp(startValue, targetValue, t);
            playerWeaponMaterial.SetFloat(Dissolve, currentValue);

            elapsedTime += Time.deltaTime;
        
            yield return null; 
        }

        playerWeaponMaterial.SetFloat(Dissolve, targetValue);
        dissolveCoroutine = null; 
    }
    
    //------------------------------------------------------------------------------------------------------------------------------
    private void Update()
    {
    }
}
