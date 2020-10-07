using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageDealer : MonoBehaviour
{
    float cooldown = 0f;

    Enemy enemy;

    void Start()
    {
        this.enemy = this.transform.root.GetComponent<Enemy>();
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player") && Time.time > cooldown && enemy._stateMachine.CurrentState != enemy._deadState)
        {
            cooldown = Time.time + 2f;
            collision.gameObject.GetComponent<Player>().TakeDamage(20);
        }
    }
}
