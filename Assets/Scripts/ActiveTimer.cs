using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Attached to object pool items to deactive itself after a set amount of time after being activated.
/// </summary>
public class ActiveTimer : MonoBehaviour
{
    public float activeTime;

    private float creationTime;

    private void OnEnable()
    {
        creationTime = Time.time;
    }

    void Update()
    {
        if (Time.time > creationTime + activeTime)
        {
            this.gameObject.SetActive(false);
        }
    }
}
