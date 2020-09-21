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
    AudioSource audio;

    bool isChasing;
    bool isIdle;
    bool isDead;
    bool isFading;
    [SerializeField]Image healthBar;
    [SerializeField]float maxLife;
    float currentLife;

    public float nextAnimation;
    public float animationDelay;

    [SerializeField]Transform currentTarget;

    float timeBeforeMove = 2f;
    float currentTime;

    float walkingSpeed = 2.5f;
    float runningSpeed = 5f;

    #region Audio
    [SerializeField] List<AudioClip> breathingClips;
    [SerializeField] List<AudioClip> damageClips;
    [SerializeField] AudioClip deathClip;

    float currentBreath = 0f;
    float nextBreath = 5f;
    float baseBreathDelay = 5f;

    [SerializeField] bool GetTargetAtBeginning;
    #endregion

    enum State
    {
        Idle,
        Walking,
        Chasing,
        Dead,
    }

    void Start()
    {
        Initialize();
    }

    void Initialize()
    {
        agent = this.GetComponent<NavMeshAgent>();
        animator = this.GetComponent<Animator>();
        audio = this.GetComponent<AudioSource>();
        currentLife = maxLife;
        currentTarget = MonsterCheckpointHandler.Instance.ReturnRandomCheckpoint();
        agent.SetDestination(currentTarget.position);
    }

    void Update()
    {
        if (isFading)
        {
            this.transform.position += transform.up * -1f * Time.deltaTime;
        }
        if (isDead) return;
        Breathe();
        animator.SetFloat("Speed", GetVelocity());

        if (isChasing)
        {
            agent.SetDestination(GameManager.Instance.Player.transform.position);
        }

        if (agent.remainingDistance < 2f && !isIdle) //TODO: change 2f to meaningful value || GAAT HIER ALTIJD IN AAN HET BEGIN OMDAT HIJ NOG GEEN DESTINATION HEEFT GEHAD (denk ik, maar iig komthij hier altijd atm)
        {
            if (isChasing)
            {
                //attack
            }
            else if (!isChasing)
            {
                StartCoroutine(StayIdle());
            }
        }
    }

    //TOOD: Turn off UI after a second or so, ugly to keep around

    IEnumerator StayIdle()
    {
        isIdle = true;
        // look around or something
        yield return new WaitForSeconds(3f);
        SetNewTarget();
        isIdle = false;
        //set new target
    }

    public void SetNewTarget()
    {
        agent.SetDestination(MonsterCheckpointHandler.Instance.ReturnRandomCheckpoint(currentTarget).position);
    }

    void Breathe()
    {
        if (Time.time > currentBreath + nextBreath && !isChasing)
        {
            currentBreath = Time.time;
            nextBreath = baseBreathDelay * Random.Range(0.8f, 1.2f);
            if (!audio.isPlaying)
            {
                audio.clip = breathingClips[Random.Range(0, breathingClips.Count)];
                audio.Play();
            }
        }
    }
    public void TakeDamage(float damage)
    {
        if (isDead) return;

        if (!isChasing)
        {
            StartChasing();
        }

        if (Time.time > nextAnimation)
        {
            //prolong agent delay before speedy;
            animator.SetTrigger("Hit");
            agent.velocity = Vector3.zero;
            nextAnimation = Time.time + animationDelay;
            audio.clip = damageClips[Random.Range(0, damageClips.Count)];
            audio.Play();
        }

        if (currentLife - damage <= 0f)
        {
            Die();

        }
        else
        {
            currentLife -= damage;
        }
        //UpdateUI();
    }

    void StartChasing()
    {
        isChasing = true;
        agent.speed = runningSpeed;
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
        agent.isStopped = true;
        audio.clip = deathClip;
        audio.Play();
        StartCoroutine(waitUntilFade());
    }

    IEnumerator waitUntilFade()
    {
        yield return new WaitForSeconds(1f); // wait 1 second before turning off agent, allowing other monsters to step over his corpse. Could use the animation length for waitforsecond as well
        agent.enabled = false;
        yield return new WaitForSeconds(5f);
        healthBar.gameObject.transform.parent.gameObject.SetActive(false);
        isFading = true;
        yield return new WaitForSeconds(5f);
        this.gameObject.SetActive(false);
    }

    float GetVelocity()
    {
        if (agent.speed == walkingSpeed)
        {
            return agent.velocity.magnitude / walkingSpeed;
        }
        else if (agent.speed == runningSpeed)
        {
            return agent.velocity.magnitude / runningSpeed * 2f;
        }
        else
        {
            return 0f;
        }
    }
}
