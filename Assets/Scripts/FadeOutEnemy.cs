using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FadeOutEnemy : MonoBehaviour
{
    void Update()
    {
        this.transform.position += transform.up * -1f * Time.deltaTime;
    }
}
