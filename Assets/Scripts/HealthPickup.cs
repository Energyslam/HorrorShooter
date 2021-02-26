using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthPickup : MonoBehaviour
{
    private Material mat;

    private float startTimeDisappear;

    private bool isDisappearing;

    [SerializeField]private  int healthAmount;

    private void Awake()
    {
        mat = this.GetComponent<Renderer>().material;
    }

    private void Update()
    {
        if (isDisappearing)
        {
            this.mat.SetFloat("_Transparancy", 1f - (Time.time - startTimeDisappear));
            if ((Time.time - startTimeDisappear) > 1f)
            {
                Destroy(this.gameObject);
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            other.gameObject.GetComponent<Player>().IncreaseHealth(healthAmount);
            startTimeDisappear = Time.time;
            isDisappearing = true;
            this.GetComponent<BoxCollider>().enabled = false;
        }
    }
}
