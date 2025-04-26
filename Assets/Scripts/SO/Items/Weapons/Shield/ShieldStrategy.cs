using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ShieldStrategy", menuName = "WorldOfIdes/Items/Create a shieldStrategy")]
public class ShieldStrategy : Weapon
{
    public override GameObject Instantiate()
    {
        return Instantiate(_prefab);
    }

    public override void UseItem()
    {
        base.UseItem();
    }

    public override void LightAttack(GameObject obj)
    {

    }

    public override void HeavyAttack(GameObject obj)
    {
        
    }
}
