using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "SwordStrategy", menuName = "WorldOfIdes/Items/Weapons/Create a swordStrategy")]
public class SwordStrategy : Weapon
{
    public float damage = 30;
    public float maxRange = 3f;

    //public Vector3 rayOrigin;

    public override GameObject Instantiate()
    {
        Debug.Log("INSTANTIATING SWORD");
        return Instantiate(_prefab);
    }

    public override void UseItem()
    {
        base.UseItem();
    }

    public override void Attack(GameObject obj)
    {
        RaycastHit hit;

        Vector3 rayOrigin = PlayerManager.Instance.transform.position + PlayerManager.Instance.transform.up * 1f + PlayerManager.Instance.transform.forward * 0.5f;

        if (Physics.SphereCast(rayOrigin, 0.5f, PlayerManager.Instance.transform.forward, out hit, maxRange))
        {
            var npcController = hit.transform.root.GetComponent<NPCController>();
            if (npcController && !PlayerManager.Instance._playerController.hasHit)
            {
                npcController.RecieveDamage(damage);
                PlayerManager.Instance._playerController.hasHit = true;

                Debug.Log("Hit NPC: " + npcController.name);
            }
        }

        Debug.DrawRay(rayOrigin, PlayerManager.Instance.transform.forward * maxRange, Color.red, 20f); // Debug the raycast
    }
}
