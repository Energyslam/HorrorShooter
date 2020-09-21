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

        foreach (Transform t in transform)
        {
            if (t.gameObject.activeInHierarchy)
            {
                monsterCheckpoints.Add(t);
            }
        }
    }

    /// <summary>
    /// Retrieves a random checkpoint from the list of active checkpoints
    /// </summary>
    /// <returns>A random active checkpoint</returns>
    public Transform ReturnRandomCheckpoint()
    {
        Transform tmp = monsterCheckpoints[Random.Range(0, monsterCheckpoints.Count)];
        return tmp;
    }
    /// <summary>
    /// Retrieves a random checkpoint from the list of active checkpoints that is not the same as the given checkpoint
    /// </summary>
    /// <param name="current">Transform of the current targeted checkpoint</param>
    /// <returns></returns>
    public Transform ReturnRandomCheckpoint(Transform current)
    {
        Transform tmp = monsterCheckpoints[Random.Range(0, monsterCheckpoints.Count)];
        if (tmp == current)
        {
            tmp = ReturnRandomCheckpoint(current);
        }
        return tmp;
    }
}
