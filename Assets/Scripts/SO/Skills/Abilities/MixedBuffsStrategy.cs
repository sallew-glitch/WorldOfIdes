using System.Collections;
using UnityEngine;

public class MixedBuffsStrategy : Skill
{
    public GameObject _buffPrefab;
    public float damageMultiplier;
    public float movementMultiplier;
    public float MPMultiplier;

    public override IEnumerator Activate(Transform origin)
    {
        PlayerManager.Instance._playerController._animator.SetTrigger("nukeMagic");

        var buff = Instantiate(_buffPrefab, origin.position, Quaternion.LookRotation(origin.forward), origin);

        var ctx = new SkillContext
        {
            damageMultiplier = damageMultiplier,
            movementMultiplier = movementMultiplier,
            MPMultiplier = MPMultiplier,
        };

       

        Destroy(buff, duration);

        yield return new WaitForSeconds(0);
    }

    public override IEnumerator Cooldown(Transform origin)
    {
        throw new System.NotImplementedException();
    }
}
