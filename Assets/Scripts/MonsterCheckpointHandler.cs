using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterCheckpointHandler : MonoBehaviour
{
    private static MonsterCheckpointHandler _instance;
    public static MonsterCheckpointHandler Instance { get { return _instance; } }

    public List<Transform> monsterCheckpoints;
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
        foreach(Transform t in transform)
        {
            if (t.gameObject.activeInHierarchy)
            {
                monsterCheckpoints.Add(t);
            }
        }
    }
}
