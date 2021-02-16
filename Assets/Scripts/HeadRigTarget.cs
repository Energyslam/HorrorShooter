using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeadRigTarget : MonoBehaviour
{
    //public bool isTracking;

    [SerializeField] private Transform origin;
    public Transform previousTarget;
    public Transform trackedTarget;

    private float lerpStart;

    private void Start()
    {
        previousTarget = origin;
        trackedTarget = origin;        
    }

    //TODO: Currently tracking while dead. 

    private void Update()
    {
        Lerp();
    }

    public void TrackTarget(Transform target)
    {
        if (target == null)
        {
            previousTarget = trackedTarget;
            trackedTarget = origin;
            lerpStart = Time.time;
        }
        else
        {
            previousTarget = trackedTarget;
            trackedTarget = target;
            lerpStart = Time.time;
        }

    }

    public void Reset()
    {
        if (trackedTarget == origin)
        {
            return;
        }
        if (trackedTarget == null)
        {
            trackedTarget = origin;
        }
        if (this.transform.position != trackedTarget.position)
        {
            previousTarget = this.transform;
        }
        else
        {
            previousTarget = trackedTarget;
        }

        trackedTarget = origin;
        lerpStart = Time.time;
    }

    public void Lerp()
    {
        this.transform.position = Vector3.Lerp(previousTarget.position, trackedTarget.position, Time.time - lerpStart);
    }
}
