using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Useful for attaching to GameObject that only serve a purpose in Editor mode.
/// </summary>
public class AutoTurnoff : MonoBehaviour
{

    void Start()
    {
        if (this.gameObject.activeInHierarchy)
        {
            this.gameObject.SetActive(false);
        }
    }
}
