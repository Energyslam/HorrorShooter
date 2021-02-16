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
    [SerializeField] private float animationDelay;
    [SerializeField] private float maxLife;
    [SerializeField] private float viewingDistance = 1f;

    private float currentBreath = 0f;
    private float nextBreath = 5f;
    private float baseBreathDelay = 5f;

    private float nextAnimation;
    private float currentLife;

    private float nextIdle = 0f;
    private float idleDelay = 3f;

    #region Audio
    [SerializeField] private List<AudioClip> breathingClips;
    [SerializeField] private List<AudioClip> damageClips;
    [SerializeField] private AudioClip _deathClip;
    [SerializeField] private AudioClip _growlClip;


    #endregion

    [SerializeField] private HeadRigTarget tracker;

    [SerializeField] private Detector detector;

    public IState _deadState { get; private set; }
    private IState _idleState;
    private IState _walkingState;
    private IState _chasingState;

    public StateMachine _stateMachine;

    private bool waitingForChase;
    private void Start()
    {
        Initialize();
        _stateMachine.SetState(_walkingState);
    }

    private void Initialize()
    {
        _agent = this.GetComponent<NavMeshAgent>();
        _animator = this.GetComponent<Animator>();
        _audio = this.GetComponent<AudioSource>();
        _player = GameManager.Instance.Player.transform;

        _stateMachine = new StateMachine();
        _idleState = new Idle(_agent);
        _walkingState = new Walking(_agent, _walkingSpeed, _animator, this.transform);
        _chasingState = new Chasing(_agent, _player, _chasingSpeed, _walkingSpeed, _animator, this.transform);
        _deadState = new Dead(_agent, _animator, _audio, _deathClip, this.gameObject);
        _player = GameManager.Instance.Player.transform;
        currentLife = maxLife;
    }

    private void Update()
    {
        _stateMachine.Tick();
        if (_stateMachine.CurrentState == _deadState)
            return;

        Breathe();
        CheckForTrackables();
        CheckForPlayerInSight();
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
    }

    private IEnumerator GrowlAndWaitBeforeChasing()
    {
        waitingForChase = true;
        _audio.clip = _growlClip;
        _audio.Play();
        yield return new WaitForSeconds(2f);
        _stateMachine.SetState(_chasingState);
        waitingForChase = false;
    }

    private void CheckForTrackables()
    {
        if (detector.hasObjectsInView && _stateMachine.CurrentState != _deadState)
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

    private void CheckForPlayerInSight()
    {
        if (_stateMachine.CurrentState != _chasingState && !waitingForChase)
        {
            if (detector.containsPlayer)
            {
                StartCoroutine(GrowlAndWaitBeforeChasing());
            }
        }
    }

    private void UpdateTracker(Transform target)
    {
        tracker.TrackTarget(target);
    }

    private void ResetTracker()
    {
        tracker.Reset();
    }

    private void Breathe()
    {
        if (Time.time > currentBreath + nextBreath && (_stateMachine.CurrentState == _walkingState || _stateMachine.CurrentState == _idleState))
        {
            currentBreath = Time.time;
            nextBreath = baseBreathDelay * UnityEngine.Random.Range(0.8f, 1.2f); //TODO magic numbers, change to something meaningful?
            if (!_audio.isPlaying)
            {
                _audio.clip = breathingClips[UnityEngine.Random.Range(0, breathingClips.Count)];
                _audio.Play();
            }
        }
    }
    private void GetHit()
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

    private void UpdateUI()
    {
        if (!healthBar.transform.parent.gameObject.activeInHierarchy)
        {
            healthBar.transform.parent.gameObject.SetActive(true);
        }
        healthBar.fillAmount = currentLife / maxLife;
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawSphere(this.transform.position, viewingDistance);
    }
}
