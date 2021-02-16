using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Detector : MonoBehaviour
{
    [SerializeField]List<Transform> currentTrackableObjects = new List<Transform>();
    public bool containsObjects;
    public bool hasObjectsInView;
    public bool containsPlayer;

    public float viewingAngle = 60f;
    private float nextCheck = 0f;
    private float delay = 0.3f;

    private Transform playerTransform;


    private void Start()
    {
        playerTransform = GameManager.Instance.Player.transform;
    }

    private void Update()
    {
        CheckForInactives();
        if (containsObjects)
        {
            CheckForRetrievableObjects();
        }
        CheckForPlayer();
    }

    public Transform RetrieveClosestObject()
    {
        if (currentTrackableObjects.Contains(GameManager.Instance.Player.transform))
        {
            if (HelperFunctions.IsInViewingRange(viewingAngle, GameManager.Instance.Player.transform.position, this.transform))
            {
                return GameManager.Instance.Player.transform;
            }
        }
        Transform t = null;
        float currentClosestDistance = 99999f;
        for (int i = 0; i < currentTrackableObjects.Count; i++)
        {
            if (currentTrackableObjects[i].gameObject.activeInHierarchy)
            {
                if (HelperFunctions.IsInViewingRange(viewingAngle, currentTrackableObjects[i].position, this.transform))
                {
                    if ((this.transform.position - currentTrackableObjects[i].position).magnitude < currentClosestDistance)
                    {
                        t = currentTrackableObjects[i];
                    }
                }
            }
        }
        return t;
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") || other.CompareTag("Tracer"))
        {
            if (!currentTrackableObjects.Contains(other.transform))
            {
                currentTrackableObjects.Add(other.transform);
                containsObjects = true;
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player") || other.CompareTag("Tracer"))
        {
            if (currentTrackableObjects.Contains(other.transform))
            {
                currentTrackableObjects.Remove(other.transform);
                if (currentTrackableObjects.Count == 0)
                {
                    containsObjects = false;
                }
            }
        }
    }


    private void CheckForPlayer()
    {
        if (currentTrackableObjects.Contains(playerTransform))
        {
            containsPlayer = true;
        }
        else
        {
            containsPlayer = false;
        }
    }
    private void CheckForInactives()
    {
        for (int i = 0; i < currentTrackableObjects.Count; i++)
        {
            if (!currentTrackableObjects[i].gameObject.activeInHierarchy)
            {
                currentTrackableObjects.Remove(currentTrackableObjects[i]);
            }
        }
        if (currentTrackableObjects.Count <= 0)
        {
            containsObjects = false;
            hasObjectsInView = false;
        }
    }

    private void CheckForRetrievableObjects()
    {
        bool tmpBool = false;
        for (int i = 0; i < currentTrackableObjects.Count; i++)
        {           
            if (HelperFunctions.IsInViewingRange(viewingAngle, currentTrackableObjects[i].position, this.transform))
            {
                tmpBool = true;
            }
        }
        hasObjectsInView = tmpBool;

    }

}
