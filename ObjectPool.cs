using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class PooledObject
{
    public Object objectToPool;
    public int pooledAmount = 50;
    public bool canGrow = true;
}

public class ObjectPool : MonoBehaviour
{
    public static ObjectPool instance;
    public PooledObject[] pooledObjectTypes;

    private Dictionary<string, List<GameObject>> pool;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
    }

    void Start()
    {
        // set up the pool and instantiate all initial objects
        pool = new Dictionary<string, List<GameObject>>();
        for (int i = 0; i < pooledObjectTypes.Length; i++)
        {
            PooledObject pooledObjectType = pooledObjectTypes[i];
            pool.Add(pooledObjectType.objectToPool.name, new List<GameObject>());
            for (int i2 = 0; i2 < pooledObjectType.pooledAmount; i2++)
            {
                AddPooledObject(pooledObjectType.objectToPool);
            }
        }
    }

    GameObject AddPooledObject(Object newObject)
    {
        GameObject pooledObject = Instantiate(newObject) as GameObject;
        if (instance.gameObject.transform != null)
        {
            pooledObject.transform.parent = instance.gameObject.transform;
        }
        pooledObject.SetActive(false);
        pool[newObject.name].Add(pooledObject);
        return pooledObject;
    }

    public void DisableAllPooledObjects()
    {
        foreach (string key in pool.Keys)
        {
            foreach (GameObject pooledObject in pool[key])
            {
                pooledObject.SetActive(false);
            }
        }
    }

    public List<GameObject> GetPooledObjects(Object gameObject)
    {
        if (!pool.ContainsKey(gameObject.name))
        {
            return null;
        }
        return pool[gameObject.name];
    }

    public GameObject GetPooledObject(Object gameObject)
    {
        // return a regular non-pooled version if there is no pool for this type
        List<GameObject> pooledObjects = GetPooledObjects(gameObject);
        if (pooledObjects == null)
        {
            Debug.Log("Returning non-pooled instance of: " + gameObject.name);
            return (Instantiate(gameObject) as GameObject);
        }

        // return first available inactive pooled object
        for (int i = 0; i < pooledObjects.Count; i++)
        {
            if (!pooledObjects[i].activeInHierarchy)
            {
                return pooledObjects[i];
            }
        }

        // grow the pool if needed and return the new object
        for (int i = 0; i < pooledObjectTypes.Length; i++)
        {
            if (pooledObjectTypes[i].objectToPool.name == gameObject.name)
            {
                if (pooledObjectTypes[i].canGrow)
                {
                    return AddPooledObject(gameObject);
                }
                else
                {
                    return pooledObjects[0];
                }
            }
        }

        // return null if none was created
        // this should be handled by the caller script
        return null;
    }
}