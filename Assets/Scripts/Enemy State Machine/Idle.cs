using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Idle : MonoBehaviour, IState
{
    NavMeshAgent _agent;
    float currentIdleTime;
    public Idle(NavMeshAgent agent)
    {
        _agent = agent;
    }

    public void Tick()
    {
        currentIdleTime++;
    }

    public void OnEnter()
    {
        _agent.enabled = false;
        currentIdleTime = 0f;
    }

    public void OnExit()
    {
        currentIdleTime = 0f;
    }
}
