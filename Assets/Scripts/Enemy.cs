using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Enemy : MonoBehaviour
{
    // Start is called before the first frame update
    NavMeshAgent agent;
    Animator animator;
    bool running;
    bool chasing;

    float maxLife;
    float currentLife;

    public float nextAnimation;
    public float animationDelay;
    void Start()
    {
        agent = this.GetComponent<NavMeshAgent>();
        animator = this.GetComponent<Animator>();
        currentLife = maxLife;
    }

    // Update is called once per frame
    void Update()
    {
        float velocity = agent.velocity.magnitude / agent.speed;

        if (chasing)
        {
            agent.destination = GameManager.Instance.Player.transform.position;
        }

        if (velocity > 0f)
        {
            if (running)
            {
                animator.SetFloat("Speed", velocity * 2f);
            }
            else
            {
                animator.SetFloat("Speed", velocity);
            }
        }
        else
        {
            animator.SetFloat("Speed", 0f);
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            chasing = true;
        }

        if (Input.GetKeyDown(KeyCode.U))
        {
            running = true;
            agent.speed = agent.speed * 2f;
        }

        if (Input.GetKeyDown(KeyCode.I))
        {
            agent.speed = 0f;
            chasing = false;
        }

        if (Input.GetKeyDown(KeyCode.P))
        {
            agent.speed = 5f;
        }
    }

    public void TakeDamage(float damage)
    {
        if (Time.time > nextAnimation)
        {
            animator.SetTrigger("Hit");
            nextAnimation = Time.time + animationDelay;
        }

        if (currentLife - damage < 0f)
        {
            //die
        }
        else
        {
            currentLife -= damage;
            //update healthcounter?
        }

        //spawn particle system // object pool
    }
}
