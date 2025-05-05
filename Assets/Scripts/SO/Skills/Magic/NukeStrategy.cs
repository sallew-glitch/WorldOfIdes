using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NukeStrategy", menuName = "WorldOfIdes/Skills/Magic/Create Nuke Strategy")]
public class NukeStrategy : Skill
{
    public GameObject _nukePrefab;
    public float AOERadius = 20f;
    public float damage = 10f;
    public LayerMask NPCLayer;

    public override IEnumerator Activate(Transform origin)
    {
        PlayerManager.Instance._playerController._animator.SetTrigger("magicArea");

        yield return new WaitForSeconds(2);

        var nuke = Instantiate(_nukePrefab, origin.position, Quaternion.identity, origin);

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
            areaRadius = AOERadius,
            targetLayerMask = NPCLayer
        };

        var effect = nuke.AddComponent<AreaOfEffectDamage>();
        effect.Initialize(origin, ctx);
        effect.Execute();

        Destroy(nuke, duration);

        yield return new WaitForSeconds(0);
    }

    public override IEnumerator Cooldown(Transform origin)
    {
        throw new System.NotImplementedException();
        yield return new WaitForSeconds(0);
    }
}
