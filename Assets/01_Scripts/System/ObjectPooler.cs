using System.Collections.Generic;
using UnityEngine;

public class ObjectPooler : MonoBehaviour
{
    public static ObjectPooler Instance;

    [System.Serializable]
    public class Pool
    {
        public string tag;
        public GameObject prefab;
        public int size;
    }

    public List<Pool> pools;
    public Dictionary<string, List<GameObject>> poolDictionary;

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        poolDictionary = new Dictionary<string, List<GameObject>>();

        foreach (Pool pool in pools)
        {
            List<GameObject> objectPool = new List<GameObject>();

            for (int i = 0; i < pool.size; i++)
            {
                GameObject obj = Instantiate(pool.prefab);
                obj.SetActive(false); 
                objectPool.Add(obj);
            }
            poolDictionary.Add(pool.tag, objectPool);
        }
    }

    public GameObject SpawnFromPool(string tag, Vector3 position, Quaternion rotation)
    {
        if (!poolDictionary.ContainsKey(tag))
        {
            return null;
        }

        GameObject objectToSpawn = null;
        foreach (GameObject obj in poolDictionary[tag])
        {
            if (!obj.activeInHierarchy)
            {
                objectToSpawn = obj;
                break; // 찾았으면 반복 중단
            }
        }

        if (objectToSpawn == null)
        {
            Pool p = pools.Find(pool => pool.tag == tag);
            if (p != null)
            {
                objectToSpawn = Instantiate(p.prefab);
                poolDictionary[tag].Add(objectToSpawn); 
            }
            else
            {
                return null;
            }
        }


        objectToSpawn.SetActive(true);
        objectToSpawn.transform.position = position;
        objectToSpawn.transform.rotation = rotation;

        ParticleSystem[] particleSystems = objectToSpawn.GetComponentsInChildren<ParticleSystem>();
        foreach (ParticleSystem ps in particleSystems)
        {
            ps.Play();
        }

        return objectToSpawn;
    }
}