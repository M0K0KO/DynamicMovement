using System.Collections;
using UnityEngine;

public class HitstopManager : MonoBehaviour
{
    public static HitstopManager Instance { get; private set; }

    private Coroutine hitstopCoroutine;
    private float originalTimeScale = 1f;

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
        // 기존 Time.timeScale 저장
        originalTimeScale = Time.timeScale;
        
        // 시간 멈춤
        Time.timeScale = 0f;

        // 지정된 시간(실제 시간 기준)만큼 대기
        yield return new WaitForSecondsRealtime(duration);

        // 시간 원래대로 복원
        Time.timeScale = originalTimeScale;

        hitstopCoroutine = null;
    }
}