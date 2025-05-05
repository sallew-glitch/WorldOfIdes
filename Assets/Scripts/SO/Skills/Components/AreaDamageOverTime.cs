using UnityEngine;
using System.Collections;

public class AreaOfEffectOverTime : MonoBehaviour
{
    private float _radius;
    private float _damagePerTick;
    private float _duration;
    private float _tickInterval;   
    private LayerMask _targetLayer;
    private bool _isInitialized;

    public void Initialize(Transform origin, SkillContext ctx)
    {
        transform.position = origin.position;
        _radius = ctx.areaRadius;
        _damagePerTick = ctx.damage;
        _duration = ctx.duration;
        _targetLayer = ctx.targetLayerMask;
        _tickInterval = ctx.tickInterval;

        _isInitialized = true;
    }

    public void Execute()
    {
        if (!_isInitialized) return;
        StartCoroutine(DamageOverTimeCoroutine());
    }

    private IEnumerator DamageOverTimeCoroutine()
    {
        float elapsed = 0f;
        while (elapsed < _duration)
        {            
            Collider[] hits = Physics.OverlapSphere(transform.position, _radius, _targetLayer);
            foreach (var c in hits)
            {
                var npc = c.GetComponentInParent<NPCController>();
                if (npc != null)
                    npc.RecieveDamage(_damagePerTick);
            }
            
            yield return new WaitForSeconds(_tickInterval);
            elapsed += _tickInterval;
        }

        Destroy(gameObject);
    }
}
