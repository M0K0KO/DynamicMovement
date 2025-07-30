using System.Collections.Generic;
using UnityEngine;

public class VFXPoolManager : MonoBehaviour
{
    public static VFXPoolManager Instance { get; private set; }

    [System.Serializable]
    public class Pool
    {
        public string tag;
        public GameObject prefab;
        public int size;
    }
    
    public List<Pool> pools; // 여러 종류의 풀을 관리할 리스트
    private Dictionary<string, Queue<GameObject>> poolDictionary; // 실제 풀링 데이터 저장소
    

    private void Awake()
    {
        Instance = this;
    }


    void Start()
    {
        poolDictionary = new Dictionary<string, Queue<GameObject>>();

        foreach (Pool pool in pools)
        {
            Queue<GameObject> objectPool = new Queue<GameObject>();

            for (int i = 0; i < pool.size; i++)
            {
                GameObject obj = Instantiate(pool.prefab, this.transform, true);
                obj.SetActive(false);
                objectPool.Enqueue(obj);
            }

            poolDictionary.Add(pool.tag, objectPool);
        }
    }
    
    public GameObject GetFromPool(string tag, Vector3 position, Quaternion rotation, Transform parent, float startTime)
    {
        if (!poolDictionary.ContainsKey(tag))
        {
            Debug.LogWarning($"Pool with tag '{tag}' doesn't exist.");
            return null;
        }

        Queue<GameObject> queue = poolDictionary[tag];
        GameObject objectToSpawn;

        if (queue.Count > 0)
        {
            objectToSpawn = queue.Dequeue();
        }
        else
        {
            var originalPrefab = pools.Find(p => p.tag == tag)?.prefab;
            if (originalPrefab != null)
            {
                objectToSpawn = Instantiate(originalPrefab);
            }
            else
            {
                return null;
            }
        }
        
        if (parent != null)
        {
            // 부모가 있으면, 자식으로 만들고 '로컬' 좌표를 설정합니다.
            objectToSpawn.transform.SetParent(parent, false);
            objectToSpawn.transform.localPosition = position;
            objectToSpawn.transform.localRotation = rotation;
        }
        else
        {
            // 부모가 없으면, 최상위 오브젝트로 두고 '월드' 좌표를 설정합니다.
            objectToSpawn.transform.SetParent(null);
            objectToSpawn.transform.position = position;
            objectToSpawn.transform.rotation = rotation;
        }
        
        if (startTime > 0f)
        {
            ParticleSystem ps = objectToSpawn.GetComponent<ParticleSystem>();
            if (ps != null)
            {
                ps.time = startTime;
            }
        }
        
        objectToSpawn.SetActive(true);
        return objectToSpawn;
    }

    public void ReturnToPool(string tag, GameObject objectToReturn)
    {
        if (!poolDictionary.ContainsKey(tag))
        {
            Debug.LogWarning($"Pool with tag '{tag}' doesn't exist.");
            Destroy(objectToReturn);
            return;
        }

        objectToReturn.transform.SetParent(transform);
        objectToReturn.SetActive(false);
        poolDictionary[tag].Enqueue(objectToReturn);
    }
}
