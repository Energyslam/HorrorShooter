using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Chasing : MonoBehaviour, IState
{
    private NavMeshAgent _agent;
    private Transform _player;
    Transform _enemy;
    private Animator _animator;
    private float _walkingSpeed;
    private float _runningSpeed;
    private float _animSpeed;
    private float attackDelay = 1f;
    private float nextAttack;
    private bool isAttacking;

    public Chasing(NavMeshAgent agent, Transform player, float runningSpeed, float walkingSpeed, Animator animator, Transform enemy)
    {
        _agent = agent;
        _player = player;
        _runningSpeed = runningSpeed;
        _walkingSpeed = walkingSpeed;
        _animator = animator;
        _enemy = enemy;
    }
    public void Tick()
    {
        _agent.SetDestination(_player.position);
        if (!isAttacking)
        {
            if (_agent.velocity.magnitude > _walkingSpeed)
            {
                _animSpeed = ((_agent.velocity.magnitude - _walkingSpeed) / _runningSpeed);
                _animator.SetFloat("RunningSpeed", _animSpeed);
            }
            if (_agent.velocity.magnitude < _walkingSpeed)
            {
                _animSpeed = ((_agent.velocity.magnitude) / _walkingSpeed);
                _animator.SetFloat("Speed", _animSpeed);
            }
        }
        CheckDistance();
        ManualTimeTracker();
    }

    public void CheckDistance()
    {
        if ((_player.position - _enemy.position).magnitude < 3f && !isAttacking)
        {
            Attack();
        }
    }

    public void ManualTimeTracker()
    {
        if (Time.time > nextAttack)
        {
            isAttacking = false;
            _agent.speed = _runningSpeed;
        }
    }
    public void Attack()
    {
        isAttacking = true;
        nextAttack = Time.time + attackDelay;
        _agent.speed = 0f;
        _agent.velocity = Vector3.zero;
        _animator.SetFloat("RunningSpeed", 0f);
        _animator.SetFloat("Speed", 0f);
        _animator.SetTrigger("Attack");

    }
    public void OnEnter()
    {
        _agent.enabled = true;
        _agent.speed = _runningSpeed;
        _animator.SetFloat("Speed", 1f);
    }

    public void OnExit()
    {
        isAttacking = false;
        _animator.SetFloat("RunningSpeed", 0f);
    }
}
