using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ShieldDomeStrategy", menuName = "WorldOfIdes/Skills/Physical/Create ShieldDome Strategy")]
public class ShieldDomeStrategy : Skill
{
    public GameObject _shieldPrefab;

    public  ShieldDomeStrategy()
    {
        SkillType = SKILLTYPE.PHYSICAL;
        duration = 10f;
        cooldown = 14f;
    }
    public override IEnumerator Activate(Transform origin)
    {
        var shield = Instantiate(_shieldPrefab, origin.position, Quaternion.identity, origin);
        Destroy(shield, duration);
        yield return new WaitForSeconds(0);
    }

    public override IEnumerator Cooldown(Transform origin)
    {
        throw new System.NotImplementedException();
        yield return new WaitForSeconds(0);
    }
}
   