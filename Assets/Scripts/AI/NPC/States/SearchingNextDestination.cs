using UnityEngine;
using UnityEngine.AI;

public class SearchingNextDestination : IState
{
    private readonly Civillian _civillian;

    private float wanderingRadius = 15f;

    public SearchingNextDestination(Civillian civillian)
    {
        _civillian = civillian;
    }

    public void Tick()
    {
        _civillian.Destination = FindingNextTarget();
    }

    private Vector3 FindingNextTarget()
    {
        Vector3 randomDir = Random.insideUnitSphere * wanderingRadius;
        Vector3 rawPos = _civillian.spawnPosition + randomDir;

        if (NavMesh.SamplePosition(rawPos, out NavMeshHit hit, wanderingRadius, NavMesh.AllAreas))
            return hit.position;
        
        return _civillian.spawnPosition;
    }

    public void OnEnter()
    {        
    }

    public void OnExit()
    {        
    }
}
