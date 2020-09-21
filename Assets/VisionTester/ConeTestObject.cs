using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConeTestObject : MonoBehaviour
{
    private ConeTester coneTester = null;
    private void OnDrawGizmos()
    {
        if (coneTester == null)
        {
            coneTester = GameObject.FindObjectOfType<ConeTester>();
            if (coneTester == null)
            {
                return;
            }
        }

        Gizmos.color = coneTester.TestCone(this.transform.position) ? Color.green : Color.red;
        Gizmos.DrawSphere(this.transform.position, 0.1f);
    }
}
