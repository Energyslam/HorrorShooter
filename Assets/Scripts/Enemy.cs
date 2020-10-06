using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public class Enemy : MonoBehaviour
{
    #region Properties
    NavMeshAgent _agent;
    Animator _animator;
    AudioSource _audio;
    Transform _player;
    [SerializeField]Image healthBar;
    #endregion

    public Transform currentTarget { get; private set; }

    [SerializeField] private float stoppingDistance = 5f;
    [SerializeField] private float _walkingSpeed = 2.5f;
    [SerializeField] private float _chasingSpeed = 5f;

    private float nextAnimation;
    [SerializeField] private float animationDelay;
    [SerializeField] private float maxLife;
    private float currentLife;

    float nextIdle = 0f;
    float idleDelay = 3f;

    #region Audio
    [SerializeField] List<AudioClip> breathingClips;
    [SerializeField] List<AudioClip> damageClips;
    [SerializeField] AudioClip _deathClip;
    [SerializeField] AudioClip _growlClip;

    float currentBreath = 0f;
    float nextBreath = 5f;
    float baseBreathDelay = 5f;
    #endregion

    [SerializeField] HeadRigTarget tracker;

    [SerializeField] float viewingDistance = 1f;

    [SerializeField] Detector detector;

    private IState _deadState;
    private IState _idleState;
    private IState _walkingState;
    private IState _chasingState;

    public StateMachine _stateMachine;

    bool waitingForChase;
    void Start()
    {
        Initialize();
    }

    void Initialize()
    {
        _agent = this.GetComponent<NavMeshAgent>();
        _animator = this.GetComponent<Animator>();
        _audio = this.GetComponent<AudioSource>();
        _player = GameManager.Instance.Player.transform;

        _stateMachine = new StateMachine();
        _idleState = new Idle(_agent);
        _walkingState = new Walking(_agent, _walkingSpeed, _animator, this.transform);
        _chasingState = new Chasing(_agent, _player, _chasingSpeed, _walkingSpeed, _animator, this.transform);
        _deadState = new Dead(_agent, _animator, _audio, _deathClip);

        _player = GameManager.Instance.Player.transform;
        currentLife = maxLife;
    }

    void Update()
    {
        if (_stateMachine.CurrentState == _deadState)
            return;

        _stateMachine.Tick();

        Breathe();
        CheckForTrackables();
        CheckForPlayerInSight(); //refactor

        //if ((_player.transform.position - this.transform.position).magnitude > 15f && _stateMachine.CurrentState == _chasingState)
        //{
        //    _stateMachine.SetState(_walkingState);
        //}
    }

    IEnumerator GrowlAndWaitBeforeChasing()
    {
        waitingForChase = true;
        _audio.clip = _growlClip;
        _audio.Play();
        yield return new WaitForSeconds(2f);
        _stateMachine.SetState(_chasingState);
        waitingForChase = false;
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

    void CheckForPlayerInSight()
    {
        if (_stateMachine.CurrentState != _chasingState && !waitingForChase)
        {
            if (detector.containsPlayer)
            {
                StartCoroutine(GrowlAndWaitBeforeChasing());
            }
        }
    }

    void UpdateTracker(Transform target)
    {
        tracker.TrackTarget(target);
    }

    void ResetTracker()
    {
        tracker.Reset();
    }

    void Breathe()
    {
        if (Time.time > currentBreath + nextBreath && (_stateMachine.CurrentState == _walkingState || _stateMachine.CurrentState == _idleState))
        {
            currentBreath = Time.time;
            nextBreath = baseBreathDelay * UnityEngine.Random.Range(0.8f, 1.2f); //magic numbers
            if (!_audio.isPlaying)
            {
                _audio.clip = breathingClips[UnityEngine.Random.Range(0, breathingClips.Count)];
                _audio.Play();
            }
        }
    }
    void GetHit()
    {
        if (_stateMachine.CurrentState != _chasingState)
        {
            waitingForChase = false;
            StopCoroutine(GrowlAndWaitBeforeChasing());
            _stateMachine.SetState(_chasingState);
        }

        if (Time.time > nextAnimation)
        {
            //TODO: prolong agent delay before starting movement again; ||||| Make into state ?
            _animator.SetTrigger("Hit");
            _agent.velocity = Vector3.zero;
            nextAnimation = Time.time + animationDelay;
            _audio.clip = damageClips[UnityEngine.Random.Range(0, damageClips.Count)];
            _audio.Play();
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

    public void TakeDamage(float damage)
    {
        if (_stateMachine.CurrentState == _deadState) return;
        if (currentLife - damage <= 0f)
        {
            currentLife = 0;
            _stateMachine.SetState(_deadState);
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
