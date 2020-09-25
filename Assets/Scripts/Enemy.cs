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

    [SerializeField]Image healthBar;

    [SerializeField]Transform currentTarget;

    [SerializeField] private float stoppingDistance = 2f;
    [SerializeField] private float walkingSpeed = 2.5f;
    private float nextAnimation;
    [SerializeField] private float animationDelay;
    [SerializeField] private float maxLife;
    private float currentLife;

    float nextIdle = 0f;
    float idleDelay = 3f;

    #region Audio
    [SerializeField] List<AudioClip> breathingClips;
    [SerializeField] List<AudioClip> damageClips;
    [SerializeField] AudioClip deathClip;

    float currentBreath = 0f;
    float nextBreath = 5f;
    float baseBreathDelay = 5f;
    #endregion

    [SerializeField] HeadRigTarget tracker;

    [SerializeField] float viewingDistance = 1f;

    [SerializeField] Detector detector;

    public bool moveAtStart;
    enum State
    {
        Idle,
        Growling,
        Walking,
        Chasing,
        Dead,
        SecretState
    }

    State state = State.Idle;
    State previousState = State.Idle;
    void Start()
    {
        Initialize();
        if (!moveAtStart)
        {
            state = State.SecretState;
        }
    }

    void Initialize()
    {
        agent = this.GetComponent<NavMeshAgent>();
        animator = this.GetComponent<Animator>();
        audio = this.GetComponent<AudioSource>();
        currentLife = maxLife;
    }

    void Update()
    {
        if (CheckIfDead()) return;

        Breathe();
        SetAnimatorSpeed();
        CheckForTrackables();


        if (state == State.Chasing)
        {
            agent.SetDestination(GameManager.Instance.Player.transform.position);
        }

        CheckForArrival();

    }

    void CheckForArrival()
    {
        //On arriving at destination
        if (agent.remainingDistance < stoppingDistance)
        {
            if (state == State.Idle)
            {
                if (Time.time > nextIdle)
                {
                    nextIdle = Time.time + nextIdle;
                    SetNewAgentTarget();
                    ChangeState(State.Walking);
                }
            }
            else if (state == State.Walking)
            {
                ChangeState(State.Idle);
            }
            else if (state == State.Chasing)
            {
                Debug.Log("Remaing = " + agent.remainingDistance + " || stopping distance = " + stoppingDistance);
                animator.SetTrigger("Attack");
            }
        }
    }

    void ChangeState(State newState)
    {
        previousState = this.state;
        this.state = newState;

        if (newState == State.Idle)
        {
            nextIdle = Time.time + idleDelay;
        }
        else if (newState == State.Chasing)
        {
            UpdateTracker(GameManager.Instance.Player.transform);
        }

        else if (newState == State.Dead)
        {
            StartCoroutine(Die());
        }

        if (previousState == State.Chasing)
        {
            ResetTracker();
        }
        UpdateAgentSpeed();
    }

    //TODO: Optimize later
    void CheckForTrackables()
    {
        if (detector.hasObjectsInView)
        {
            if (tracker.trackedTarget != detector.RetrieveClosestObject())
            {
                UpdateTracker(detector.RetrieveClosestObject());
            }
        }
        else
        {
            ResetTracker();
        }
    }

    void SetAnimatorSpeed()
    {
        float speed = 0f;

        if (state == State.Walking)
        {
            speed = agent.velocity.magnitude / walkingSpeed;
        }
        else if (state == State.Chasing)
        {
            speed = agent.velocity.magnitude / walkingSpeed * 2f; ;
        }
        animator.SetFloat("Speed", speed);
    }

    void UpdateTracker(Transform target)
    {
        tracker.TrackTarget(target);
    }

    void ResetTracker()
    {
        tracker.Reset();
    }

    bool CheckIfDead()
    {
        if (state == State.Dead)
        {
            return true;
        }
        else
        {
            return false;
        }

    }
    void SetNewAgentTarget()
    {
        currentTarget = MonsterCheckpointHandler.Instance.ReturnRandomCheckpoint(currentTarget);
        agent.SetDestination(currentTarget.position);
    }

    void Breathe()
    {
        if (Time.time > currentBreath + nextBreath && (state == State.Walking || state == State.Idle))
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
    void GetHit()
    {
        if (state == State.Dead) return;

        if (state != State.Chasing)
        {
            ChangeState(State.Chasing);
        }

        if (Time.time > nextAnimation)
        {
            //TODO: prolong agent delay before starting movement again;
            animator.SetTrigger("Hit");
            agent.velocity = Vector3.zero;
            nextAnimation = Time.time + animationDelay;
            audio.clip = damageClips[Random.Range(0, damageClips.Count)];
            audio.Play();
        }
    }

    void UpdateAgentSpeed()
    {
        if (agent.isActiveAndEnabled)
        {
            if (state == State.Chasing)
            {
                agent.speed = walkingSpeed * 2f;
            }
            else if (state == State.Walking)
            {
                agent.speed = walkingSpeed;
            }
            else
            {
                agent.speed = 0f;
            }
        }
    }

    void UpdateUI()
    {
        if (!healthBar.transform.parent.gameObject.activeInHierarchy)
        {
            healthBar.transform.parent.gameObject.SetActive(true);
        }
        healthBar.fillAmount = currentLife / maxLife;
    }

    IEnumerator Die()
    {
        animator.SetTrigger("Die"); //TODO?: create animator fucntion/script
        //agent.isStopped = true; //would be nice to do, but can be a bit wonky? seems agent might still be moving a bit afterward. maybe just lack of experience with navmesh
        agent.enabled = false;
        audio.clip = deathClip; // TODO?: Create audio function/script
        audio.Play();

        yield return new WaitForSeconds(3f); // wait 1 second before turning off agent, allowing other monsters to step over his corpse. Could use the animation length for waitforsecond as well


        this.gameObject.AddComponent(typeof(FadeOutEnemy));  //add the script instead of holding functions/states only to fade out the enemy. 
        healthBar.gameObject.transform.parent.gameObject.SetActive(false); //TODO: might be better to get a permanent reference to this, altho it's only used here

        yield return new WaitForSeconds(5f);

        this.gameObject.SetActive(false);


    }

    public void TakeDamage(float damage)
    {
        if (state == State.Dead) return;
        if (currentLife - damage <= 0f)
        {
            currentLife = 0;
            ChangeState(State.Dead);
        }
        else
        {
            currentLife -= damage;
            GetHit();
        }
        //UpdateUI(); //TODO: I don't like how the UI looks in the dark
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawSphere(this.transform.position, viewingDistance);
    }
}
