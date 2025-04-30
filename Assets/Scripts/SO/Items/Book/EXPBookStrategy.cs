using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName= "EXPBookStrategy", menuName= "WorldOfIdes/Items/Books/Create an EXPBookStrategy")]
public class EXPBookStrategy : Item
{
    public GameObject _prefab;    
    
    public override void UseItem()
    {
        PlayerManager.Instance.UpdateXP(PlayerManager.Instance.playerXP + 50);
    }
}
