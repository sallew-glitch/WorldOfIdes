using System.Collections;

using UnityEngine;

[CreateAssetMenu(fileName = "LaserStrategy", menuName = "WorldOfIdes/Skills/Magic/Create Laser Strategy")]
public class LaserStrategy : Skill
{
    public GameObject _laserPrefab;
    public int animationDelay = 2;
    public float damagePerTick;
    public float tickInterval;
    public LayerMask targetLayer;

    public override IEnumerator Activate(Transform origin)
    {
        PlayerManager.Instance._playerController._animator.SetTrigger("laser");

        yield return new WaitForSeconds(1.6f);

        PlayerManager.Instance._playerController._animator.speed = 0.12f;

        var newPosition = origin.position + Vector3.up * 1.15f + origin.forward * 1.3f;
        var laser = Instantiate(_laserPrefab, newPosition, Quaternion.LookRotation(origin.forward), origin);

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
            duration = duration,
            tickInterval = tickInterval,
            damage = damagePerTick,
            targetLayerMask = targetLayer,
};

        var effect = laser.AddComponent<ProjectileDamageOverTime>();
        effect.Initialize(laser.transform, ctx);
        effect.Execute();

        Destroy(laser, duration);

        yield return new WaitForSeconds(duration);

        PlayerManager.Instance._playerController._animator.speed = 1f;
    }

    public override IEnumerator Cooldown(Transform origin)
    {
        throw new System.NotImplementedException();
        yield return new WaitForSeconds(0);
    }
}
