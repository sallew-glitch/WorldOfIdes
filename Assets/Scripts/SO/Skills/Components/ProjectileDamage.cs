using UnityEngine;

public class ProjectileDamage : MonoBehaviour, IEffectBehavior
{

    private float damage;
    private float duration = 0f;
    private LayerMask targetLayer;
    public void Initialize(Transform origin, SkillContext ctx)
    {
        damage = ctx.damage;
        targetLayer = ctx.targetLayerMask;

        if (ctx.duration > 0f)
            duration = ctx.duration;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (((1 << other.gameObject.layer) & targetLayer) == 0)
        {                     
            return;
        }

        Enemy npc = other.GetComponentInParent<Enemy>();

        if (npc != null)
        {
            npc.RecieveDamage(damage);

            var hitEffects = GetComponents<IHitEffect>();
            foreach (var effect in hitEffects)
                effect.Apply(npc);
        }

        Destroy(gameObject, duration);
    }

    public void Execute()
    {}

}
