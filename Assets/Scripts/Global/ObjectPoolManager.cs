using System.Collections.Generic;
using UnityEngine;

namespace Global
{
    public class ObjectPoolManager : MonoBehaviour
    {
        public static ObjectPoolManager Instance;
        
        public List<Pool> pools;
        
        private Dictionary<string, Queue<GameObject>> _poolDictionary = new Dictionary<string, Queue<GameObject>>();

        private void Awake()
        {
            if (Instance == null) Instance = this;
            else Destroy(this);
            InitializePools();
        }
        private void InitializePools()
        {
            foreach (var pool in pools)
            {
                var parentObject = new GameObject(pool.tag + "_Pool");
                var objectQueue = new Queue<GameObject>();
                for (var i = 0; i < pool.initialSize; i++)
                {
                    var obj = Instantiate(pool.prefab, parentObject.transform, true);
                    obj.SetActive(false);
                    objectQueue.Enqueue(obj);
                }
                _poolDictionary.Add(pool.tag, objectQueue);

            }
        }

        public GameObject SpawnFromPool(string poolTag, Vector3 spawnPosition, Quaternion spawnRotation)
        {
            if (!_poolDictionary.ContainsKey(poolTag)) return null;
            var objectToSpawn = _poolDictionary[poolTag].Count > 0
                ? _poolDictionary[poolTag].Dequeue()
                : Instantiate
                    (pools.Find(pool => pool.tag == poolTag)?.prefab);
            objectToSpawn.transform.SetParent(null);
            objectToSpawn.SetActive(true);
            objectToSpawn.transform.position = spawnPosition;
            objectToSpawn.GetComponent<Rigidbody>().linearVelocity = Vector3.zero;
            objectToSpawn.GetComponent<Rigidbody>().angularVelocity = Vector3.zero;

            objectToSpawn.transform.rotation = spawnRotation;
            
            return objectToSpawn;
        }

        public void ReturnToPool(string poolTag, GameObject obj)
        {
            var sanitizedTag = obj.name.Replace("(Clone)", "");
            if (!_poolDictionary.ContainsKey(sanitizedTag))
            {
                _poolDictionary[poolTag] = new Queue<GameObject>();
            }

            var poolParent = GameObject.Find(sanitizedTag + "_Pool");
            if (poolParent)
            {
                obj.transform.SetParent(poolParent.transform);
                obj.transform.localPosition = Vector3.zero; 
                obj.transform.localRotation = Quaternion.identity;
            }
            
            obj.SetActive(false);
            _poolDictionary[sanitizedTag].Enqueue(obj);
        }

    }

    [System.Serializable]
    public class Pool
    {
        public string tag;
        public GameObject prefab;
        public int initialSize;
    }
}
