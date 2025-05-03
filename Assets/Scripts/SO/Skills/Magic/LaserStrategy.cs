using System.Collections;

using UnityEngine;

[CreateAssetMenu(fileName = "LaserStrategy", menuName = "WorldOfIdes/Skills/Magic/Create Laser Strategy")]
public class LaserStrategy : Skill
{
    public GameObject _laserPrefab;
    public int animationDelay = 2;

    public LaserStrategy()
    {
        SkillType = SKILLTYPE.MAGIC;
        duration = 5f;
        cooldown = 12f;
    }
    public override IEnumerator Activate(Transform origin)
    {
        PlayerManager.Instance._playerController._animator.SetBool("laserMagic", true);      

        yield return new WaitForSeconds(animationDelay);        

        var shield = Instantiate(_laserPrefab, origin.position + origin.up * 1f + origin.forward * 1f, origin.rotation, origin);
        Destroy(shield, duration);
        PlayerManager.Instance._playerController._animator.SetBool("laserMagic", false);
    }

    public override IEnumerator Cooldown(Transform origin)
    {
        throw new System.NotImplementedException();
        yield return new WaitForSeconds(0);
    }
}
