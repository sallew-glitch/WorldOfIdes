using UnityEngine;
using UnityEngine.AI;

public class WalkingState : IState
{

    private readonly NPC _civillian;
    private readonly Animator _animator;
    private readonly NavMeshAgent _navMeshAgent;

    private static readonly int Speed = Animator.StringToHash("Speed");
    private Vector3 _lastPosition = Vector3.zero;

    public float TimeStuck = 0;
    
    public WalkingState(NPC civillian, Animator animator, NavMeshAgent navMeshAgent)
    {
        _civillian = civillian;
        _animator = animator;
        _navMeshAgent = navMeshAgent;
    }

    public void Tick()
    {
        if (Vector3.Distance(_civillian.transform.position, _lastPosition) <= 0f)
            TimeStuck += Time.deltaTime;

        _lastPosition = _civillian.transform.position;
    }

    public void OnEnter()
    {        
        TimeStuck = 0f;
        _navMeshAgent.enabled = true;
        _navMeshAgent.speed = 2f;
        _navMeshAgent.SetDestination(_civillian.Destination);        
        _animator?.SetFloat(Speed, 0.5f);
    }

    public void OnExit()
    {     
        _navMeshAgent.enabled = false;
        _animator?.SetFloat(Speed, 0f);
    }
}
