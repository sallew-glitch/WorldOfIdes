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
    public override void Activate(Transform origin)
    {
        var shield = Instantiate(_shieldPrefab, origin.position, Quaternion.identity, origin);
        Destroy(shield, duration);
    }

    public override void Cooldown(Transform origin)
    {
        throw new System.NotImplementedException();
    }
}
   