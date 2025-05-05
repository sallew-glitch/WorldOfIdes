using UnityEngine;
using System.Collections;

[CreateAssetMenu(fileName = "DashStrategy", menuName = "WorldOfIdes/Skills/Physical/Create Dash Strategy")]
public class DashStrategy : Skill
{
    public GameObject _dashPrefab;

    public float dashDistance;
    public float safetyBuffer = 0.1f;
    public LayerMask obstacleLayers;

    public override IEnumerator Activate(Transform origin)
    {
        //PlayerManager.Instance._playerController._animator.SetTrigger("magicArea2");

        var dash = Instantiate(_dashPrefab, origin.position, Quaternion.identity, origin);

        if (soundEffect != null)
        {
            AudioManager.instance.PlayOneShot(
                soundEffect,
                soundVolume,
                soundPitch
            );
        }

        Vector3 start = origin.position + Vector3.up * 1.5f;
        Vector3 dir = origin.forward;

        float travel = dashDistance;
        if (Physics.Raycast(start, dir, out var hit, dashDistance, obstacleLayers))
        {
            float avail = Mathf.Max(0, hit.distance - safetyBuffer);            
            travel = avail;
        }        

        var cc = origin.GetComponent<CharacterController>();

        if (cc != null)
        {
            Vector3 displacement = dir * travel;            
            cc.Move(displacement);
        }
        else                   
            origin.position += dir * travel;

        Destroy(dash, duration);

        yield return null;
    }

    public override IEnumerator Cooldown(Transform origin)
    {
        throw new System.NotImplementedException();
    }
}
