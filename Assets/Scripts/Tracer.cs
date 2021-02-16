using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Tracers need to "attach" to walls, but movement may be too fast and collision is detected once tracer is already inside an object.
/// So the velocity of the tracer is reversed and added to the contact point so a nearest point on the collider can be calculated.
/// </summary>
///
[RequireComponent(typeof(Rigidbody))]
public class Tracer : MonoBehaviour
{
    private Rigidbody rb;
    [SerializeField] private GameObject gizmo;

    private void Start()
    {
        rb = this.GetComponent<Rigidbody>();
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
    }
}
