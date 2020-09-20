using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public class Enemy : MonoBehaviour
{
    // Start is called before the first frame update
    NavMeshAgent agent;
    Animator animator;
    bool running;
    bool chasing;
    bool isDead;
    [SerializeField]Image healthBar;

    [SerializeField]float maxLife;
    float currentLife;

    public float nextAnimation;
    public float animationDelay;

    [SerializeField]Transform currentTarget;

    float timeBeforeMove = 2f;
    float currentTime;

    float walkingSpeed = 2.5f;
    float runningSpeed = 7.0f;

    public enum State
    {
        Idle,
        Walking,
        Running
    }

    public State state = State.Idle;

    void Start()
    {
        Initialize();
        SetNewTarget();
    }

    void Initialize()
    {
        agent = this.GetComponent<NavMeshAgent>();
        animator = this.GetComponent<Animator>();
        currentLife = maxLife;
    }

    void Update()
    {
        if (isDead) return;

        CheckForTarget();
        float velocity = agent.velocity.magnitude / agent.speed;

        animator.SetFloat("Speed", velocity);

        if (chasing)
        {
            agent.destination = GameManager.Instance.Player.transform.position;
        }
    }
    public void TakeDamage(float damage)
    {
        if (isDead) return;

        if (Time.time > nextAnimation)
        {
            animator.SetTrigger("Hit");
            nextAnimation = Time.time + animationDelay;
        }

        if (currentLife - damage < 0f)
        {
            Die();

        }
        else
        {
            currentLife -= damage;
        }
        UpdateUI();
        //Keep particle system spawning in Gun script, or move here?
    }

    void UpdateUI()
    {
        if (!healthBar.transform.parent.gameObject.activeInHierarchy)
        {
            healthBar.transform.parent.gameObject.SetActive(true);
        }
        healthBar.fillAmount = currentLife / maxLife;
    }

    void Die()
    {
        currentLife = 0;
        animator.SetTrigger("Die");
        isDead = true;
    }

    void SetNewTarget()
    {
        Transform temp = MonsterCheckpointHandler.Instance.monsterCheckpoints[Random.Range(0, MonsterCheckpointHandler.Instance.monsterCheckpoints.Count)];
        if (currentTarget != temp)
        {
            currentTarget = temp;
            agent.SetDestination(currentTarget.position);
            StartWalking();
        }

    }

    void CheckForTarget()
    {
        if (agent.remainingDistance < 2f)
        {
            Idle();
        }
    }

    void Idle()
    {
        StopMoving();
        currentTime = Time.time;
    }

    void StopMoving()
    {
        agent.speed = 0f;
        state = State.Idle;
    }

    void StartRunning()
    {
        agent.speed = runningSpeed;
        state = State.Running;
    }

    void StartWalking()
    {
        agent.speed = walkingSpeed;
        state = State.Walking;
    }
}
