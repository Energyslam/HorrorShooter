using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActiveTimer : MonoBehaviour
{
    public float activeTime;

    private float creationTime;

    private void OnEnable()
    {
        creationTime = Time.time;
    }

    // Update is called once per frame
    void Update()
    {
        if (Time.time > creationTime + activeTime)
        {
            this.gameObject.SetActive(false);
        }
    }
}
