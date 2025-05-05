using System.Collections;
using UnityEngine;

[CreateAssetMenu(fileName = "BloodBloomStrategy", menuName = "WorldOfIdes/Skills/Magic/Create Blood Bloom Strategy")]
public class BloodBloomStrategy : Skill
{
    public GameObject _bloodPrefab;    

    public float range;
    public float mpCost;
    public float damage;
    public float AOERadius;
    public LayerMask targetLayer;

    public override IEnumerator Activate(Transform origin)
    {
        if (!PlayerManager.Instance.TrySpendMana(mpCost))
            yield break;

        PlayerManager.Instance._playerController._animator.SetTrigger("magicArea2");

        yield return new WaitForSeconds(2);

        var newPosition = origin.position + origin.forward * range;

        var blood = Instantiate(_bloodPrefab, newPosition, Quaternion.identity);

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

        var effect = blood.AddComponent<AreaOfEffectDamage>();
        effect.Initialize(blood.transform, ctx);
        effect.Execute();

        Destroy(blood, duration);

        yield return new WaitForSeconds(0f);
    }

    public override IEnumerator Cooldown(Transform origin)
    {
        throw new System.NotImplementedException();
    }
}
