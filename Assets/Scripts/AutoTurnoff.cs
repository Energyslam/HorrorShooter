using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AutoTurnoff : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        if (this.gameObject.activeInHierarchy)
        {
            this.gameObject.SetActive(false);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
