using UnityEngine;

public class AreaOfEffectDamage : MonoBehaviour, IEffectBehavior
{
    private float radius;
    private float damage;
    private LayerMask targetLayer;

    public void Initialize(Transform origin, SkillContext ctx)
    {
        transform.position = origin.position;
        radius = ctx.areaRadius;
        damage = ctx.damage;
        targetLayer = ctx.targetLayerMask;
    }
    public void Execute()
    {
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, radius, targetLayer);

        foreach (Collider hitCollider in hitColliders)
        {
            Enemy npc = hitCollider.GetComponentInParent<Enemy>();
            if (npc != null)
            {
                npc.RecieveDamage(damage);
                Debug.Log($"Damaged NPC: {npc.name} for {damage} damage.");
            }
        }
    }

}
