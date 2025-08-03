using System;
using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

public class HitstopManager : MonoBehaviour
{
    public static HitstopManager Instance { get; private set; }

    private Coroutine hitstopCoroutine;
    private float originalTimeScale = 1f;
    private bool isHitstopping = false;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Update()
    {
        Debug.Log(hitstopCoroutine == null);
    }

    public void StartHitstop(float duration)
    {
        // 이미 힛스탑이 실행 중이면 중지
        if (hitstopCoroutine != null)
        {
            StopCoroutine(hitstopCoroutine);
        }

        hitstopCoroutine = StartCoroutine(HitstopCoroutine(duration));
    }

    private IEnumerator HitstopCoroutine(float duration)
    {
        if (!isHitstopping)
        {
            originalTimeScale = Time.timeScale;
            isHitstopping = true;
        }
        
        Time.timeScale = 0f;

        yield return new WaitForSecondsRealtime(duration);

        Time.timeScale = originalTimeScale;
        isHitstopping = false;
        hitstopCoroutine = null;
    }
}
