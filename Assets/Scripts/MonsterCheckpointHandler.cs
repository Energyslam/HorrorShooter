using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.PlayerLoop;

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

    public void ReleaseCheckpoint(Transform checkpoint)
    {
        monsterCheckpoints.Find(x => x == checkpoint).gameObject.SetActive(true);
    }

    /// <summary>
    /// Retrieves a random checkpoint from the list of active checkpoints
    /// </summary>
    /// <returns>A random active checkpoint</returns>
    public Transform ReturnRandomCheckpoint()
    {
        Transform tmp = monsterCheckpoints[Random.Range(0, monsterCheckpoints.Count)];
        monsterCheckpoints.Find(x => x == tmp).gameObject.SetActive(false);
        return tmp;
    }
    
    /// <summary>
    /// Retrieves a random checkpoint from the list of active checkpoints that is not the same as the given checkpoint
    /// </summary>
    /// <param name="current">Transform of the current targeted checkpoint</param>
    /// <returns></returns>
    public Transform ReturnRandomCheckpoint(Transform current)
    {
        List<Transform> activeTransforms = monsterCheckpoints.FindAll(x => x.gameObject.activeInHierarchy);
        Transform tmp = activeTransforms[Random.Range(0, activeTransforms.Count)];
        if (tmp == current)
        {
            tmp = ReturnRandomCheckpoint(current);
            return tmp;
        }
        monsterCheckpoints.Find(x => x == tmp).gameObject.SetActive(false);
        return tmp;
    }
}
