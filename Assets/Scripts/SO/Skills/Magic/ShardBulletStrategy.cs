using UnityEngine;
using System.Collections;

[CreateAssetMenu(fileName = "ShardBulletStrategy", menuName = "WorldOfIdes/Skills/Magic/Create Shard Bullet Strategy")]
public class ShardBulletStrategy : Skill
{
    public GameObject _shardBulletPrefab;
    
    public float damage;    
    public LayerMask targetLayer;

    private Vector3 direction;
    public override IEnumerator Activate(Transform origin)
    {
        PlayerManager.Instance._playerController._animator.SetTrigger("magicProjectile1");

        yield return new WaitForSeconds(2f);

        PlayerManager.Instance._playerController._animator.SetTrigger("fireballMagic");

        direction = origin.forward;
        var newPosition = origin.position + origin.forward * 3f;

        var shardBullet = Instantiate(_shardBulletPrefab, newPosition, Quaternion.LookRotation(direction));

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
            targetLayerMask = targetLayer,
            damage = damage,
            duration = duration,            
            freezeDuration = duration - 1f,            
        };
        
        var damageEffect = shardBullet.AddComponent<ProjectileDamage>();
        //var freezeEffect = shardBullet.AddComponent<FreezeEnemy>();

        damageEffect.Initialize(shardBullet.transform, ctx);

        //freezeEffect.Initialize(shardBullet.transform, ctx);
        //freezeEffect.Execute();

        yield return new WaitForSeconds(duration);

        Destroy(shardBullet);
    }
    public override IEnumerator Cooldown(Transform origin)
    {
        throw new System.NotImplementedException();
        yield return new WaitForSeconds(0);
    }
}
