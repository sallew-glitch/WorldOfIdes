using UnityEngine;

public class NPCUtilities
{
    public static Transform GetClosestNPCInRange(Vector3 origin, float maxRange, LayerMask npcLayer)
    {
        Collider[] hits = Physics.OverlapSphere(origin, maxRange, npcLayer);
        Transform closest = null;
        float bestDist = float.MaxValue;

        foreach (var hit in hits)
        {
            var npc = hit.GetComponentInParent<Enemy>();
            if (npc == null) continue;
            float d = (npc.transform.position - origin).sqrMagnitude;
            if (d < bestDist)
            {
                bestDist = d;
                closest = npc.transform;
            }
        }
        return closest;
    }
}
