using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "HealthBottleStrategy", menuName = "WorldOfIdes/Items/Create a healthBottleStrategy")]
public class HealthBottleStrategy : Item
{
    public GameObject _prefab;
    public override void UseItem()
    {
        PlayerManager.Instance.UpdateHealth(PlayerManager.Instance.playerHP + 20);
    }
}
