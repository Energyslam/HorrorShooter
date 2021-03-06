﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterSpawner : MonoBehaviour
{
    private static MonsterSpawner _instance;
    public static MonsterSpawner Instance { get { return _instance; } }

    [SerializeField] private GameObject ghoul;

    private List<Transform> spawnLocations = new List<Transform>();
    public List<GameObject> existingMonsters = new List<GameObject>();

    private float spawnDelay = 15f;
    private float nextSpawn;

    private int monstersToSpawn;
    private int maxMonstersToSpawn;

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

    void Start()
    {
        foreach (Transform t in this.transform)
        {
            spawnLocations.Add(t);
        }

        monstersToSpawn = spawnLocations.Count / 2;
        maxMonstersToSpawn = monstersToSpawn * 2;
        nextSpawn = Time.time + spawnDelay;
        SpawnInitialMonsters();
    }

    void Update()
    {
        if (Time.time > nextSpawn)
        {
            if (existingMonsters.Count < maxMonstersToSpawn)
            {
                SpawnMonster();
            }
            nextSpawn = Time.time + spawnDelay;
        }
    }
    public void RemoveMonster(GameObject go)
    {
        existingMonsters.Remove(go);
    }

    private void SpawnMonster()
    {
        //TODO change from picking a random point to the farthest away point
        GameObject go = Instantiate(ghoul, spawnLocations[Random.Range(0, spawnLocations.Count)].position, Quaternion.identity);
        existingMonsters.Add(go);
    }

    private void CheckForDeadMonsters()
    {

    }


    private void SpawnInitialMonsters()
    {
        List<Transform> copySpawnLocations = spawnLocations;
        List<Transform> usedSpawnLocation = new List<Transform>();

        for (int i = 0; i < monstersToSpawn; i++)
        {
            int rnd = Random.Range(0, copySpawnLocations.Count);
            usedSpawnLocation.Add(copySpawnLocations[rnd]);
            copySpawnLocations.RemoveAt(rnd);
        }

        for (int i = 0; i < usedSpawnLocation.Count; i++)
        {
            GameObject go = Instantiate(ghoul, usedSpawnLocation[i].position, Quaternion.identity);
            existingMonsters.Add(go);
        }
    }
}
