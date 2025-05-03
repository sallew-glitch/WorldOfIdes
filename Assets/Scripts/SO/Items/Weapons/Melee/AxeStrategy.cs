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

    public override void LightAttack(GameObject obj)
    {
        //Attack audio
        int index = Random.Range(1, 8);
        AudioManager.instance.Play("Swipe" + index);

        InputManager.Instance.StartCoroutine(LightAttackWaiter(0.6f));
    }

    IEnumerator LightAttackWaiter(float duration)
    {
        Transform attackPos = GameObject.FindGameObjectWithTag("attackPos").transform;

        yield return new WaitForSeconds(duration);

        LayerMask enemyLayer = LayerMask.GetMask("Enemies");
        Collider[] hitObjects = Physics.OverlapSphere(attackPos.position, maxRange, enemyLayer);

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

                //hitstop
                HitStop.instance.Stop(0.1f);

                enemyControl.RecieveDamage(damage);
                Debug.Log("Perform Test3");


                //enemyRb.AddForce(knockbackDirection.normalized * knockbackForce, ForceMode.Impulse);
            }
        }
    }

    public override void HeavyAttack(GameObject obj)
    {
        //Attack audio
        int index = Random.Range(1, 8);
        AudioManager.instance.Play("Swipe" + index);

        InputManager.Instance.StartCoroutine(HeavyAttackWaiter(0.6f));
    }

    IEnumerator HeavyAttackWaiter(float duration)
    {
        Transform attackPos = GameObject.FindGameObjectWithTag("attackPos").transform;

        yield return new WaitForSeconds(duration);

        LayerMask enemyLayer = LayerMask.GetMask("Enemies");
        Collider[] hitObjects = Physics.OverlapSphere(attackPos.position, maxRange, enemyLayer);

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

                //hitstop
                HitStop.instance.Stop(0.1f);

                enemyControl.RecieveDamage(damage);
                Debug.Log("Perform Test3");


                //enemyRb.AddForce(knockbackDirection.normalized * knockbackForce, ForceMode.Impulse);
            }
        }
    }

}
