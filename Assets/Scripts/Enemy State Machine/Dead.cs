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
    GameObject _enemyGO;

    float despawnDelay = 8f;
    float despawnTimer;
    public Dead(NavMeshAgent agent, Animator animator, AudioSource audio, AudioClip clip, GameObject enemyGO)
    {
        _agent = agent;
        _animator = animator;
        _audio = audio;
        _clip = clip;
        _enemyGO = enemyGO;
    }
    public void Tick()
    {
        if (Time.time > despawnTimer)
        {
            MonsterSpawner.Instance.RemoveMonster(_enemyGO);
            Destroy(_enemyGO);
        }
    }

    public void OnEnter()
    {
        _agent.enabled = false;
        _animator.SetTrigger("Die");
        _audio.clip = _clip;
        _audio.Play();

        despawnTimer = Time.time + despawnDelay;
        GameManager.Instance.UpdateGhoulKills();
    }

    public void OnExit()
    {

    }
}
