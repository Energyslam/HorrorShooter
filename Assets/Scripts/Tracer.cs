using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Tracers need to "attach" to walls, but movement may be too fast and collision is detected once tracer is already inside an object.
/// So the velocity of the tracer is reversed and added to the contact point so a nearest point on the collider can be calculated.
/// Atleast it used to when collision detection was set to discrete. By using continuous, it's possible to simply set the rigidbody velocity to 0 to keep the tracer wherever it hit something.
/// </summary>
///
[RequireComponent(typeof(Rigidbody))]
public class Tracer : MonoBehaviour
{
    private Rigidbody rb;
    [SerializeField] private GameObject gizmo;
    private Transform objectPoolTransform;

    private void Start()
    {
        rb = this.GetComponent<Rigidbody>();
        objectPoolTransform = this.transform.parent;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("Environment"))
        {
            //Vector3 collisionPoint = collision.contacts[0].point;
            //Vector3 pointToPlayer = GameManager.Instance.Player.transform.position - collisionPoint;
            //Vector3 adjustedDistance = collisionPoint + pointToPlayer * 0.1f;
            //Vector3 closestPointOfEntry = collision.collider.ClosestPoint(adjustedDistance);
            //this.transform.position = closestPointOfEntry;
            rb.velocity = Vector3.zero;
        }
        else if (collision.gameObject.transform.root.CompareTag("Enemy"))
        {
            rb.velocity = Vector3.zero;

            //attaching to the enemy object currently doesn't work as the enemy object is destroyed, breaking the object pool as the tracer is also removed.
            //TODO either object pool enemies, disable them first before destroying (or not destroy at all) or create a function that dislodges the tracer before destruction
            //this.gameObject.transform.parent = collision.gameObject.transform; //attach to the enemy so the light follows enemy movement.
        }
    }
}
