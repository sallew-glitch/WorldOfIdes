using UnityEngine;

public class EnableBossOnTrigger : MonoBehaviour
{
    [Tooltip("The boss GameObject to activate")]
    public GameObject boss;

    [Tooltip("Layer of the boss arena floor")]
    public string bossArenaLayerName = "Boss1Floor";

    private int bossArenaLayer;

    private void Start()
    {
        if (boss != null)
            boss.SetActive(false); // Make sure it's initially disabled

        bossArenaLayer = LayerMask.NameToLayer(bossArenaLayerName);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Player"))
        {
            Debug.Log("Debuggg");
            ActivateBoss();
        }
    }

    private void ActivateBoss()
    {
        if (boss != null && !boss.activeSelf)
        {
            boss.SetActive(true);
            Debug.Log("Boss activated!");
        }

        // Disable this script after activation (optional)
        enabled = false;
    }
}
