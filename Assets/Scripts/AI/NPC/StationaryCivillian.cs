using UnityEngine;
using UnityEngine.AI;
using System;


public class StationaryCivillian : NPC
{
    private Animator _animator;
    private NavMeshAgent _navMeshAgent;    

    private StateMachine _stateMachine;

    private void Awake()
    {
        _animator = GetComponent<Animator>();
        _navMeshAgent = GetComponent<NavMeshAgent>();        

        _stateMachine = new StateMachine();

        var idle = new IdleState(this, _animator, _navMeshAgent);
    
        _stateMachine.SetState(idle);



        spawnPosition = transform.position;
    }

    private void Update()
    {
        _stateMachine.Tick();
    }
}
