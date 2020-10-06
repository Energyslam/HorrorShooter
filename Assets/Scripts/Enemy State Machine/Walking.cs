using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Walking : MonoBehaviour, IState
{
    NavMeshAgent _agent;
    float _speed;
    float _animSpeed;
    Transform _enemy;
    Vector3 _currentDestination;
    Animator _animator;
    
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
    }

    public void OnEnter()
    {
        _agent.enabled = true;
        _agent.speed = _speed;
        if (_currentDestination == null)
        {
            _currentDestination = MonsterCheckpointHandler.Instance.ReturnRandomCheckpoint().position;
        }
        if ((_enemy.position - _currentDestination).magnitude < 2f){
            _currentDestination = MonsterCheckpointHandler.Instance.ReturnRandomCheckpoint().position;
        }
        _agent.SetDestination(_currentDestination);
    }

    public void OnExit()
    {
        _animator.SetFloat("Speed", 0f);
    }
}
