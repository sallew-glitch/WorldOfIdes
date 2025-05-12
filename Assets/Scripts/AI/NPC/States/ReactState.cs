using UnityEngine;
using UnityEngine.AI;

public class ReactToPlayerState : IState
{
    private readonly NPC _civillian;
    private readonly Animator _animator;
    private readonly NavMeshAgent _navMeshAgent;
    private readonly DetectPlayer _detectPlayer;
    private static readonly int Speed = Animator.StringToHash("Speed");

    public static readonly float lookAtTimeMax = 5f;
    public float LookAtTime;

    public ReactToPlayerState(NPC civillian, NavMeshAgent navMeshAgent, Animator animator, DetectPlayer detectPlayer)
    {
        _civillian = civillian;
        _navMeshAgent = navMeshAgent;
        _animator = animator;
        _detectPlayer = detectPlayer;
    }

    public void Tick()
    {
        LookAtPlayer();

        if (LookAtTime < lookAtTimeMax)
            LookAtTime += Time.deltaTime;
    }

    private void LookAtPlayer()
    {
        Vector3 flatDir = PlayerManager.Instance.transform.position - _civillian.transform.position;
        flatDir.y = 0f;              

        if (flatDir.sqrMagnitude < 0.001f)
            return;                  

        Quaternion rot = Quaternion.LookRotation(flatDir);
        _civillian.transform.rotation = rot;

    }

    public void OnEnter()
    {
        LookAtTime = 0f;
        _navMeshAgent.enabled = false;
        _animator?.SetFloat(Speed, 0f);
    }

    public void OnExit()
    {
        _navMeshAgent.enabled = true;
        _detectPlayer.playerDetected = false;
    }    
}
