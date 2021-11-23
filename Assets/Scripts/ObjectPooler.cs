using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Instantiate objects into a dictionary in the beginning of the game.

public class ObjectPooler : MonoBehaviour
{

    [System.Serializable]
    public class Pool
    {
        public string tag;
        public GameObject prefab;
        public int size;
    }

    #region Singleton

    public static ObjectPooler instance;

    private void Awake()
    {
        instance = this;
    }

    #endregion

    public List<Pool> pools;
    public Dictionary<string, Queue<GameObject>> poolDictionary = new Dictionary<string, Queue<GameObject>>();

    private void OnEnable()
    {
        foreach (Pool pool in pools)
        {
            Queue<GameObject> objectPool = new Queue<GameObject>();

            for (int i = 0; i < pool.size; i++)
            {
                GameObject obj = Instantiate(pool.prefab);
                obj.SetActive(false);
                obj.transform.parent = gameObject.transform;
                objectPool.Enqueue(obj);
            }

            poolDictionary.Add(pool.tag, objectPool);
        }

    }

    public GameObject SpawnFromPool(string tag, Vector3 position, Quaternion rotation, int playerNum)
    {
        if (!poolDictionary.ContainsKey(tag))
        {
            Debug.LogError("Pool with tag " + tag + " does not exist");
            return null;
        }

        GameObject objToSpawn = poolDictionary[tag].Dequeue();
        objToSpawn.SetActive(true);
        objToSpawn.transform.position = position;
        objToSpawn.transform.rotation = rotation;

        var partScript = objToSpawn.GetComponent<CarPart>();
        partScript.playerNum = playerNum;

        var pooledObj = objToSpawn.GetComponent<IPooledObject>();

        pooledObj?.onObjectSpawn();

        poolDictionary[tag].Enqueue(objToSpawn);

        return objToSpawn;
    }

    public void SetEntirePool(string tag, bool value)
    {
        if (!poolDictionary.ContainsKey(tag))
        {
            Debug.LogError("Pool with tag" + tag + "does not exist");
            return;
        }
        else
        {
            foreach (Pool pool in pools)
            {
                if (pool.tag != tag)
                {
                    continue;
                }

                for (int i = 0; i < pool.size; i++)
                {
                    GameObject spawnedObj = poolDictionary[tag].Dequeue();
                    spawnedObj.SetActive(value);
                    poolDictionary[tag].Enqueue(spawnedObj);
                }
            }
        }

    }
}