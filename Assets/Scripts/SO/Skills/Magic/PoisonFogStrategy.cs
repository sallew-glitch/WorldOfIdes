using UnityEngine;
using System.Collections;

[CreateAssetMenu(fileName = "PoisonFogStrategy", menuName = "WorldOfIdes/Skills/Magic/Create Poison Fog Strategy")]
public class PoisonFogStrategy : Skill
{
    public GameObject _poisonFogPrefab;
    public float damagePerTick;
    public float tickInterval;
    public float AOERadius;
    public float mpCost;
    public float range;
    public LayerMask targetLayer;

    public override IEnumerator Activate(Transform origin)
    {
        if (!PlayerManager.Instance.TrySpendMana(mpCost))
            yield break;

        var npc = NPCUtilities.GetClosestNPCInRange(origin.position, range, targetLayer);

        if (npc == null)
            yield break;

        PlayerManager.Instance._playerController._animator.SetTrigger("magicArea3");

        yield return new WaitForSeconds(1.5f);

        var poison = Instantiate(_poisonFogPrefab, npc.position, Quaternion.identity);

        if (soundEffect != null)
        {
            AudioManager.instance.PlayOneShot(
                soundEffect,
                soundVolume,
                soundPitch
            );
        }

        var ctx = new SkillContext
        {            
            damage = damagePerTick,
            areaRadius = AOERadius,
            targetLayerMask = targetLayer,
            tickInterval = tickInterval,
            duration = duration,
        };

        var effect = poison.AddComponent<AreaOfEffectOverTime>();
        effect.Initialize(poison.transform, ctx);
        effect.Execute();

        Destroy(poison, duration);

        yield return new WaitForSeconds(0f);
    }

    public override IEnumerator Cooldown(Transform origin)
    {
        throw new System.NotImplementedException();
    }
}
