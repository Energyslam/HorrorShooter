using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Simple gizmo script for easy gizmo testing.
/// </summary>
public class DrawGizmo : MonoBehaviour
{
    public Color col = Color.red;
    public float size = 1f;
    private void OnDrawGizmos()
    {
        Gizmos.color = col;
        Gizmos.DrawSphere(transform.position, size);
    }
}
