using UnityEngine;
using System.Collections;

[CreateAssetMenu(fileName = "ElectricBombardmentStrategy", menuName = "WorldOfIdes/Skills/Magic/Create Electric Bombardment Strategy")]
public class ElectricExplosionStrategy : Skill
{
    public GameObject _electricityPrefab;
    public GameObject _explosionPrefab;

    public float range;
    public float mpCost;
    public float damage;
    public float AOERadius;
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


        var electricity = Instantiate(_electricityPrefab, npc.position, Quaternion.identity);
        var explosion = Instantiate(_explosionPrefab, npc.position, Quaternion.identity);

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
            damage = damage,
            range = range,
            areaRadius = AOERadius,
            targetLayerMask = targetLayer
        };

        var effect = electricity.AddComponent<AreaOfEffectDamage>();
        effect.Initialize(electricity.transform, ctx);
        effect.Execute();

        Destroy(electricity, duration);
        Destroy(explosion, duration);

        yield return new WaitForSeconds(0f);
    }

    public override IEnumerator Cooldown(Transform origin)
    {
        throw new System.NotImplementedException();
    }
}
