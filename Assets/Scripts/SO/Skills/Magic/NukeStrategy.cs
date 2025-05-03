using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NukeStrategy", menuName = "WorldOfIdes/Skills/Magic/Create Nuke Strategy")]
public class NukeStrategy : Skill
{
    public GameObject _nukePrefab;
    public float AOERadius = 20f;
    public float damage = 10f;
    public LayerMask NPCLayer;

    public NukeStrategy()
    {
        SkillType = SKILLTYPE.MAGIC;
        duration = 10f;
        cooldown = 20;
    }
    public override void Activate(Transform origin)
    {
        PlayerManager.Instance._playerController._animator.SetTrigger("nukeMagic");
        var nuke = Instantiate(_nukePrefab, origin.position, Quaternion.identity, origin);

        Vector3 aoeCenter = origin.position;

        Collider[] hitColliders = Physics.OverlapSphere(aoeCenter, AOERadius, NPCLayer);
        
        foreach (Collider hitCollider in hitColliders)
        {
            NPCController npc = hitCollider.GetComponentInParent<NPCController>();
            if (npc != null)
            {
                npc.RecieveDamage(damage); 
                Debug.Log($"Damaged NPC: {npc.name} for {damage} damage.");
            }
        }

        Destroy(nuke, duration);
    }



    public override void Cooldown(Transform origin)
    {
        throw new System.NotImplementedException();
    }
}
