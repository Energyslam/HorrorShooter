using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HelperFunctions : MonoBehaviour
{
    public static bool FastVectorApproximately(Vector3 a, Vector3 b, float threshold)
    {
        if (FastApproximately(a.x, b.x, threshold) && FastApproximately(a.y, b.y, threshold) && FastApproximately(a.z, b.z, threshold))
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    public static bool FastQuaternionApproximately(Quaternion a, Quaternion b, float threshold)
    {
        if (FastApproximately(a.x, b.x, threshold) && FastApproximately(a.y, b.y, threshold) && FastApproximately(a.z, b.z, threshold) && FastApproximately(a.w, a.w, threshold))
        {
            return true;
        }
        else
        {
            return false;
        }
    }
    public static bool FastApproximately(float a, float b, float threshold)
    {
        if (threshold > 0f)
        {
            return Mathf.Abs(a - b) <= threshold;
        }
        else
        {
            return Mathf.Approximately(a, b);
        }
    }
}
