using System.Collections;

using UnityEngine;

[CreateAssetMenu(fileName = "LaserStrategy", menuName = "WorldOfIdes/Skills/Magic/Create Laser Strategy")]
public class LaserStrategy : Skill
{
    public GameObject _laserPrefab;
    Behaviour obj;
    public LaserStrategy()
    {
        SkillType = SKILLTYPE.MAGIC;
        duration = 5f;
        cooldown = 12f;
    }
    public override void Activate(Transform origin)
    {
        PlayerManager.Instance._playerController._animator.SetBool("laserMagic", true);
        
        obj.ActivateLaser(origin, duration, _laserPrefab);
    }



    public override void Cooldown(Transform origin)
    {
        throw new System.NotImplementedException();
    }
}
