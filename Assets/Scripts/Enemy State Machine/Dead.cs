using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Dead : MonoBehaviour, IState
{
    NavMeshAgent _agent;
    Animator _animator;
    AudioSource _audio;
    AudioClip _clip;
    public Dead(NavMeshAgent agent, Animator animator, AudioSource audio, AudioClip clip)
    {
        _agent = agent;
        _animator = animator;
        _audio = audio;
        _clip = clip;
    }
    public void Tick()
    {
    }

    public void OnEnter()
    {
        _agent.enabled = false;
        _animator.SetTrigger("Die");
        _audio.clip = _clip;
        _audio.Play();
    }

    public void OnExit()
    {

    }
}
