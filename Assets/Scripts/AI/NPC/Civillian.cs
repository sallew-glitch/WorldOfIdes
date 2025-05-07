using UnityEngine;
using UnityEngine.AI;
using System;

public class Civillian : MonoBehaviour
{
    private Animator _animator;
    private NavMeshAgent _navMeshAgent;
    private DetectPlayer detectPlayer;

    private StateMachine _stateMachine;
        
    public Vector3 Destination = Vector3.zero;
    public Vector3 spawnPosition;    

    private void Awake()
    {
        _animator = GetComponent<Animator>();
        _navMeshAgent = GetComponent<NavMeshAgent>();
        detectPlayer = GetComponent<DetectPlayer>();

        _stateMachine = new StateMachine();

        var idle = new IdleState(this, _animator, _navMeshAgent);
        var searchingDestination = new SearchingNextDestination(this);
        var walking = new WalkingState(this, _animator, _navMeshAgent);
        var react = new ReactToPlayerState(this, _navMeshAgent, _animator, detectPlayer);

        At(idle, searchingDestination, IdleTimeConsumed());
        At(searchingDestination, walking, DestinationFound());
        At(walking, searchingDestination, StuckForOverASecond());
        At(walking, idle, DestinationReached());
        At(react, walking, ReactToPlayerTImeConsumed());

        _stateMachine.SetState(idle);

        Func<bool> IdleTimeConsumed() => () => idle.IdleTime >= IdleState.maxIdleTime;
        Func<bool> DestinationFound() => () => Destination != Vector3.zero;
        Func<bool> StuckForOverASecond() => () => walking.TimeStuck > 1;
        Func<bool> DestinationReached() => () => Destination != Vector3.zero && Vector3.Distance(Destination, transform.position) < 1.0f;
        Func<bool> PlayerDetected() => () => detectPlayer.playerDetected;
        Func<bool> ReactToPlayerTImeConsumed() => () => react.LookAtTime >= ReactToPlayerState.lookAtTimeMax;

        _stateMachine.AddAnyTransition(react, PlayerDetected());

        void At(IState from, IState to, Func<bool> predicate) => _stateMachine.AddTransition(from, to, predicate);

        spawnPosition = transform.position;
    }

    private void Update()
    {
        _stateMachine.Tick();
    }
}
