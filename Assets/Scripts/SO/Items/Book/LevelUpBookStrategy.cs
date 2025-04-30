using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "LevelUpBookStrategy", menuName = "WorldOfIdes/Items/Books/Create a LevelUpBookStrategy")]
public class LevelUpBookStrategy : Item
{
    public GameObject _prefab;

    public override void UseItem()
    {
        PlayerManager.Instance.playerLevel += 1;
        PlayerManager.Instance.playerXP = 0;
    }
}
