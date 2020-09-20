using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// A class for storing generic GameObject items so an object pool can be created.
/// </summary>
[System.Serializable]
public class ObjectPoolItem
{
    public int amountToPool;
    public GameObject objectToPool;
    public bool shouldExpand;
}


/// <summary>
/// Holds and handles object pools. All objects are stored in the same list, and objects can be retrieved by using their tag.
/// </summary>
public class ObjectPoolHandler : MonoBehaviour
{
    private static ObjectPoolHandler _instance;
    public static ObjectPoolHandler Instance { get { return _instance; } }

    public List<ObjectPoolItem> itemsToPool;

    public List<GameObject> pooledObjects;

    private Vector3 standardDecalSize;

    void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
        }

        else if (_instance != null)
        {
            Destroy(this);
        }
    }

    private void Start()
    {
        foreach(ObjectPoolItem item in itemsToPool)
        {
            for (int i = 0; i < item.amountToPool; i++)
            {
                GameObject go = (GameObject)Instantiate(item.objectToPool, this.transform);
                go.SetActive(false);
                pooledObjects.Add(go);
            }
        }
        standardDecalSize = GetPooledObject("Decal").transform.localScale;
    }

    public GameObject GetPooledObject(string tag)
    {
        for (int i = 0; i < pooledObjects.Count; i++)
        {
            if (!pooledObjects[i].activeInHierarchy && pooledObjects[i].tag == tag)
            {
                return pooledObjects[i];
            }
        }
        foreach (ObjectPoolItem item in itemsToPool)
        {
            if (item.objectToPool.tag == tag)
            {
                if (item.shouldExpand)
                {
                    GameObject obj = (GameObject)Instantiate(item.objectToPool, this.gameObject.transform);
                    obj.SetActive(false);
                    pooledObjects.Add(obj);
                    return obj;
                }
            }
        }
        return null;
    }

    public void CreateDecal(Vector3 normal, Vector3 position)
    {
        Quaternion decalRotation = Quaternion.FromToRotation(Vector3.up, normal);

        GameObject GO = GetPooledObject("Decal");
        GO.SetActive(true);
        GO.transform.position = position + normal.normalized * 0.0001f;
        GO.transform.rotation = decalRotation;
        GO.transform.Rotate(transform.up, Random.Range(-180f, 180f));
        GO.transform.localScale = standardDecalSize * Random.Range(0.8f, 1.2f);
    }

    public void CreateBlood(Vector3 direction, Vector3 position)
    {
        Quaternion bloodRotation = Quaternion.FromToRotation(Vector3.forward, direction);

        GameObject GO = GetPooledObject("Blood");
        GO.transform.position = position;
        GO.transform.rotation = bloodRotation;
        GO.SetActive(true);
    }

    public void CreateTracer(Vector3 direction, Vector3 position, float force)
    {
        GameObject GO = GetPooledObject("Tracer");
        GO.GetComponent<TrailRenderer>().Clear();
        GO.SetActive(true);
        GO.transform.position = position;

        GO.GetComponent<Rigidbody>().velocity = Vector3.zero;
        GO.GetComponent<Rigidbody>().AddForce(direction * force, ForceMode.Impulse);
    }
}
