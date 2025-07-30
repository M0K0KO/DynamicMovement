using UnityEngine;
using System.Collections;

public class VFXReturnToPool : MonoBehaviour
{
    [Tooltip("VFXPoolManager에 설정된 것과 동일한 태그를 입력하세요.")]
    public string poolTag;

    [Tooltip("이펙트가 활성화된 후 자동으로 비활성화될 때까지의 시간(초)")]
    public float lifeTime = 2f;

    private void OnEnable()
    {
        StartCoroutine(ReturnAfterTime());
    }

    private IEnumerator ReturnAfterTime()
    {
        yield return new WaitForSeconds(lifeTime);

        VFXPoolManager.Instance.ReturnToPool(poolTag, this.gameObject);
    }

    private void OnDisable()
    {
        StopAllCoroutines();
    }
}