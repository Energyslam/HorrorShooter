using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConeSpawner : MonoBehaviour
{
    public int spawnNumber = 500;
    public float radius = 50f;

    void Start()
    {
        for (int i = 0; i < spawnNumber; i++)
        {
            GameObject spawnedObject = new GameObject();
            spawnedObject.transform.position = this.transform.position + UnityEngine.Random.insideUnitSphere * radius;
            spawnedObject.AddComponent(typeof(ConeTestObject));
        }
    }
}
