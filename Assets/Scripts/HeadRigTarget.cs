using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeadRigTarget : MonoBehaviour
{
    //public bool isTracking;

    [SerializeField] Transform origin;

    float lerpStart;

    public Transform previousTarget;
    public Transform trackedTarget;

    private void Start()
    {
        previousTarget = origin;
        trackedTarget = origin;        
    }

    //TODO: It keeps tracking even tho it's outside of view range

    private void Update()
    {
        //if (isTracking)
        //{
        //    if (trackedTarget != null)
        //    {
        //        this.transform.position = trackedTarget.position;
        //    }
        //}
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
            //isTracking = true;
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
        //isTracking = false;

        trackedTarget = origin;
        lerpStart = Time.time;
        //trackedTarget = null;
        //this.transform.position = origin.position;
    }

    public void Lerp()
    {
        this.transform.position = Vector3.Lerp(previousTarget.position, trackedTarget.position, Time.time - lerpStart);
    }
}
