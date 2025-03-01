using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ShieldStrategy", menuName = "WorldOfIdes/Items/Create a shieldStrategy")]
public class ShieldStrategy : Weapon
{    
    public override void UseItem()
    {

    }

    public override void Attack(GameObject obj)
    {

    }

    public override GameObject Instantiate()
    {
        return Instantiate(_prefab);
    }
}
