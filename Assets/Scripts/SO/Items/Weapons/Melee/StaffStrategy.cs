using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "StaffStrategy", menuName = "WorldOfIdes/Items/Weapons/Create a staffStrategy")]
public class StaffStrategy : Weapon
{
    public float damage = 30;
    public float maxRange = 2f;

    //public Vector3 rayOrigin;

    public override GameObject Instantiate()
    {
        Debug.Log("INSTANTIATING STAFF");
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

            Enemy enemyControl = obj.GetComponent<Enemy>();
            Debug.Log("Perform Test2 : " + obj);

            if (enemyControl != null && !PlayerManager.Instance._playerController.hasHit)
            {
                HitStop.instance.Stop(0.1f);

                enemyControl.RecieveDamage(damage);
                Debug.Log("Perform Test3");

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

            Enemy enemyControl = obj.GetComponent<Enemy>();
            Debug.Log("Perform Test2 : " + obj);

            if (enemyControl != null && !PlayerManager.Instance._playerController.hasHit)
            {
                HitStop.instance.Stop(0.1f);

                enemyControl.RecieveDamage(damage);
                Debug.Log("Perform Test3");

            }
        }
    }
}
