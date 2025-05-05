using UnityEngine;
using System.Collections;

[CreateAssetMenu(fileName = "ShardExplosionStrategy", menuName = "WorldOfIdes/Skills/Magic/Create Shard Explosion Strategy")]
public class ShardExplosionStrategy : Skill
{
    public GameObject _shardExplosionPrefab;
    public float AOERadius;
    public float range;
    public float damage;    
    public LayerMask targetLayer;

    public override IEnumerator Activate(Transform origin)
    {

        var npc = NPCUtilities.GetClosestNPCInRange(origin.position, range, targetLayer);

        if (npc == null)
            yield break;

        PlayerManager.Instance._playerController._animator.SetTrigger("areaExplosion");

        yield return new WaitForSeconds(1f);

        var shard = Instantiate(_shardExplosionPrefab, npc.position, Quaternion.identity);

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
            targetLayerMask = targetLayer,
            freezeDuration = duration - 1f,
            npcController = npc.GetComponent<NPCController>(),
        };

        var aoeEffect = shard.AddComponent<AreaOfEffectDamage>();
       // var freezeEffect = shard.AddComponent<FreezeEnemy>();

        aoeEffect.Initialize(shard.transform, ctx);
        //freezeEffect.Initialize(shard.transform, ctx);

        aoeEffect.Execute();
        //freezeEffect.Execute();


        Destroy(shard, duration);

        yield return new WaitForSeconds(0);
    }

    public override IEnumerator Cooldown(Transform origin)
    {
        throw new System.NotImplementedException();
        yield return new WaitForSeconds(0);
    }
}
