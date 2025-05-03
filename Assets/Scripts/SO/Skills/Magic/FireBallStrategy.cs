using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "FireballStrategy", menuName = "WorldOfIdes/Skills/Magic/Create Fireball Strategy")]
public class FireBallStrategy : Skill
{
    public GameObject _fireballPrefab; 
    Behaviour _fireballBehaviour;
    public FireBallStrategy()
    {
        SkillType = SKILLTYPE.MAGIC;
        duration = 1f;
        cooldown = 8f;
    }
    public override void Activate(Transform origin)
    {
        PlayerManager.Instance._playerController._animator.SetTrigger("fireballMagic");
        var fireball = Instantiate(_fireballPrefab, origin.position, Quaternion.identity, origin);
        _fireballBehaviour.ActivateFireball(origin, duration, fireball);
        
    }



    public override void Cooldown(Transform origin)
    {
        throw new System.NotImplementedException();
    }
}
