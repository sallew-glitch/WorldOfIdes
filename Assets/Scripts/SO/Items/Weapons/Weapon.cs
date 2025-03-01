using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Weapon : Item
{
    public GameObject _prefab;
    public override void UseItem()
    {}
    public abstract GameObject Instantiate();
    
    public abstract void Attack(GameObject obj);
}
