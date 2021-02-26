using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConeTester : MonoBehaviour
{
    public float cutoff = 45f;

    public bool TestCone(Vector3 inputPoint)
    {
        float cosAngle = Vector3.Dot((inputPoint - this.transform.position).normalized, this.transform.forward);
        float angle = Mathf.Acos(cosAngle) * Mathf.Rad2Deg;
        return angle < cutoff;
    }
}
