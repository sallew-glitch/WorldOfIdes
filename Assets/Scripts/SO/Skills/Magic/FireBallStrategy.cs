using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "FireballStrategy", menuName = "WorldOfIdes/Skills/Magic/Create Fireball Strategy")]
public class FireBallStrategy : Skill
{
    public GameObject _fireballPrefab;

    public float velocity;
    public float mpCost;
    public float travelDistance;
    public float damage;
    public int travelTime;
    public LayerMask targetLayer;

    private Vector3 direction;
    public override IEnumerator Activate(Transform origin)
    {
        if (!PlayerManager.Instance.TrySpendMana(mpCost))
            yield break;

        PlayerManager.Instance._playerController._animator.SetTrigger("magicProjectile1");

        yield return new WaitForSeconds(2f);

        direction = origin.forward;
        var newPosition = origin.position + origin.forward * 20f;

        var fireball = Instantiate(_fireballPrefab, newPosition, Quaternion.LookRotation(direction));

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
            velocity = velocity,
            travelDistance = travelDistance,
            travelTime = travelTime,
            direction = direction,
            targetLayerMask = targetLayer,
            damage = damage,
        };

        var movementEffect = fireball.AddComponent<ProjectileMotion>();
        var damageEffect = fireball.AddComponent<ProjectileDamage>();

        movementEffect.Initialize(origin, ctx);
        damageEffect.Initialize(origin, ctx);

        yield return new WaitForSeconds(travelTime);

        Destroy(fireball);
    }
    public override IEnumerator Cooldown(Transform origin)
    {
        throw new System.NotImplementedException();
        yield return new WaitForSeconds(0);
    }
}
