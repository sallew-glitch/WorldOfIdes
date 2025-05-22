using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ScytheStrategy", menuName = "WorldOfIdes/Items/Weapons/Create a scytheStrategy")]
public class ScytheStrategy : Weapon
{
    public float damage = 30;
    public float maxRange = 3f;

    //public Vector3 rayOrigin;

    public override GameObject Instantiate()
    {
        Debug.Log("INSTANTIATING SCYTHE");
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

    /*    public override void LightAttack(GameObject obj)
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
        }*/

    /*    public override void HeavyAttack(GameObject obj)
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
        }*/
}
