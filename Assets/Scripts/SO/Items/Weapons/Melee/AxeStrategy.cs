using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "AxeStrategy", menuName = "WorldOfIdes/Items/Weapons/Create an axeStrategy")]
public class AxeStrategy : Weapon
{
    public float damage = 40;
    public float maxRange = 1f;

  
    public override GameObject Instantiate()
    {
        return Instantiate(_prefab);
    }

    public override void UseItem()
    {
        base.UseItem();                
    }

    public override void Attack(GameObject obj)
    {
        /*
        RaycastHit hit;
        Debug.Log("test0");
        if (Physics.Raycast(obj.transform.position, obj.transform.up, out hit, maxRange))
        {
            Debug.Log("test1");
            if (hit.transform.root.GetComponent<NPCController>() && !PlayerManager.Instance._playerController.hasHit)
            {             
                hit.transform.root.GetComponent<NPCController>().RecieveDamage(damage);
                PlayerManager.Instance._playerController.hasHit = true;

                Debug.Log("Hit : " + hit.transform.root);
            }
        }
        */

        PerformAttack();
    }

    public void PerformAttack()
    {
        //Attack audio
        int index = Random.Range(1, 8);
        AudioManager.instance.Play("Swipe" + index);

        Transform attackPos = GameObject.FindGameObjectWithTag("attackPos").transform;
        //LayerMask enemyLayer;

        Collider[] hitObjects = Physics.OverlapSphere(attackPos.position, maxRange);

        Debug.Log("Perform Test1");
        foreach (Collider obj in hitObjects)
        {

            NPCController enemyControl = obj.GetComponent<NPCController>();
            Debug.Log("Perform Test2 : " + obj);
            //Rigidbody enemyRb = obj.GetComponent<Rigidbody>();

            if (enemyControl != null && !PlayerManager.Instance._playerController.hasHit)
            {
                // Calculate knockback direction
                /*
                Vector3 knockbackDirection = obj.transform.position - transform.position;
                knockbackDirection.y = airknockbackForce; // Keep the knockback horizontal
                */

                // Get collision point
                //Vector3 collisionPoint = obj.ClosestPoint(attackPos.position);
                //enemyControl.SpawnHitVfx(collisionPoint); //VFX for attack

                enemyControl.RecieveDamage(damage);
                Debug.Log("Perform Test3");


                //enemyRb.AddForce(knockbackDirection.normalized * knockbackForce, ForceMode.Impulse);
            }
        }
    }

}
