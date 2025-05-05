using UnityEngine;
using System.Collections;

[CreateAssetMenu(fileName = "HealingStrategy", menuName = "WorldOfIdes/Skills/Magic/Create Healing Strategy")]
public class HealingStrategy : Skill 
{
    public GameObject _healingPrefab;

    public float healValue;
    public float mpCost;

    public override IEnumerator Activate(Transform origin)
    {
        if (!PlayerManager.Instance.TrySpendMana(mpCost))
            yield break;

        PlayerManager.Instance._playerController._animator.SetTrigger("healCircle");

        yield return new WaitForSeconds(0.5f);

        var heal = Instantiate(_healingPrefab, origin.position, Quaternion.identity, origin);

        if (soundEffect != null)
        {
            AudioManager.instance.PlayOneShot(
                soundEffect,
                soundVolume,
                soundPitch
            );
        }

        PlayerManager.Instance.UpdateHealth(PlayerManager.Instance.playerHP + healValue);

        Destroy(heal, duration);

        yield return new WaitForSeconds(0f);
    }

    public override IEnumerator Cooldown(Transform origin)
    {
        throw new System.NotImplementedException();
    }
}
