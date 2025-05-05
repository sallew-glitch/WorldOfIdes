using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ProjectileDamageOverTime : MonoBehaviour, IEffectBehavior
{
    private float _duration;
    private float _tickInterval;
    private float _damagePerTick;
    private LayerMask _targetLayerMask;
    
    private readonly HashSet<NPCController> _insideTargets = new();

    private bool _isInitialized;

    public void Initialize(Transform origin, SkillContext ctx)
    {        
        transform.position = origin.position;
        transform.rotation = origin.rotation;

        _duration = ctx.duration;
        _tickInterval = ctx.tickInterval;
        _damagePerTick = ctx.damage;     
        _targetLayerMask = ctx.targetLayerMask;
        
        var col = GetComponent<Collider>();
        col.isTrigger = true;

        _isInitialized = true;
    }

    public void Execute()
    {
        if (!_isInitialized) return;
        StartCoroutine(DamageLoop());
    }

    private void OnTriggerEnter(Collider other)
    {
        if (((1 << other.gameObject.layer) & _targetLayerMask) == 0) return;
        var npc = other.GetComponentInParent<NPCController>();
        if (npc != null)
            _insideTargets.Add(npc);
    }

    private void OnTriggerExit(Collider other)
    {
        if (((1 << other.gameObject.layer) & _targetLayerMask) == 0) return;
        var npc = other.GetComponentInParent<NPCController>();
        if (npc != null)
            _insideTargets.Remove(npc);
    }

    private IEnumerator DamageLoop()
    {
        float elapsed = 0f;
        while (elapsed < _duration)
        {         
            foreach (var npc in _insideTargets)
            {
                if (npc != null)
                    npc.RecieveDamage(_damagePerTick);
            }

            yield return new WaitForSeconds(_tickInterval);
            elapsed += _tickInterval;
        }

        Destroy(gameObject);
    }
}
