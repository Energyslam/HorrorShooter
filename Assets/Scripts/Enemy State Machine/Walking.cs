using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Walking : MonoBehaviour, IState
{
    private NavMeshAgent _agent;
    private Transform _enemy;
    private Transform _currentDestination;
    private Animator _animator;
    private float _speed;
    private float _animSpeed;

    public Walking(NavMeshAgent agent, float speed, Animator animator, Transform enemy)
    {
        _agent = agent;
        _speed = speed;
        _animator = animator;
        _enemy = enemy;
    }
    public void Tick()
    {
        _animSpeed = 1f + (_agent.velocity.magnitude / _speed);
        _animator.SetFloat("Speed", _animSpeed);
        if (_currentDestination == null || (_enemy.position - _currentDestination.position).magnitude < 2f)
        {
            GetNewDestination();
        }
    }

    public void OnEnter()
    {
        _agent.enabled = true;
        _agent.speed = _speed;
        GetNewDestination();
        _agent.SetDestination(_currentDestination.position);
    }

    void GetNewDestination()
    {
        Transform tmp = _currentDestination;
        _currentDestination = MonsterCheckpointHandler.Instance.ReturnRandomCheckpoint(_currentDestination);
        _agent.SetDestination(_currentDestination.position);
        if (tmp != null)
        {
            MonsterCheckpointHandler.Instance.ReleaseCheckpoint(tmp);
        }
    }
    public void OnExit()
    {
        MonsterCheckpointHandler.Instance.ReleaseCheckpoint(_currentDestination);
        _animator.SetFloat("Speed", 0f);
    }
}
