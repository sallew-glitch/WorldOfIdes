using UnityEngine;
using UnityEngine.AI;

public class IdleState : IState
{
    private readonly Civillian _civillian;
    private readonly Animator _animator;
    private readonly NavMeshAgent _navMeshAgent;
    private static readonly int Speed = Animator.StringToHash("Speed");

    public static readonly float maxIdleTime = 5f;
    public float IdleTime;

    public IdleState(Civillian civillian, Animator animator, NavMeshAgent navMeshAgent)
    {
        _civillian = civillian;
        _animator = animator;
        _navMeshAgent = navMeshAgent;
    }

    public void Tick()
    {
        if (IdleTime < maxIdleTime)
            IdleTime += Time.deltaTime;        
    }

    public void OnEnter()
    {        
        IdleTime = 0;
        _navMeshAgent.enabled = false;        
        _animator?.SetFloat(Speed, 0f);
    } 

    public void OnExit()
    {
        _navMeshAgent.enabled = true;       ;
    }
}