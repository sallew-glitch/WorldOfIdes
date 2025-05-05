using UnityEngine;
using System.Collections;

public class FreezeEnemy : MonoBehaviour, IEffectBehavior, IHitEffect
{
    private float duration;
    private NPCController _npcController;
    public void Initialize(Transform origin, SkillContext ctx)
    {
        transform.position = origin.position;
        duration = ctx.freezeDuration;
        _npcController = ctx.npcController;
    }

    public void Execute()
    {
        StartCoroutine(Freeze());
    }

    public void Apply(NPCController npc)
    {
        StartCoroutine(Freeze());
    }

    public IEnumerator Freeze()
    {
        Debug.Log("[Freeze] Freezing Started");
        _npcController?.Freeze(true);
        yield return new WaitForSeconds(duration);
        Debug.Log("[Freeze] Freezing Ended");
        _npcController?.Freeze(false);
    }
}
