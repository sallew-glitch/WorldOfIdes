using UnityEngine;
using UnityEngine.UI;
using System.Collections;
public abstract class Enemy : MonoBehaviour 
{
    public float health;
    public float damage;

    public int EXPGiven;

    public Slider healthSlider;
    protected Animator animator;

    public GameObject enemyAttackPos;

    public GameObject gotHitPos;

    public GameObject gotHitPrefab;
    private GameObject gotHitObj;

    public void RecieveDamage(float value)
    {
        float calculatedValue = value + (1 + PlayerManager.Instance.Strength * 0.05f);

        if (health > calculatedValue)
        {
            animator.SetTrigger("takeDamage");

            gotHitObj = Instantiate(gotHitPrefab, gotHitPos.transform.position, Quaternion.identity, transform);
            //int index = Random.Range(1, 10);
            //AudioManager.instance.Play("FleshPierce" + index);
        }
        else
        {
            StartCoroutine(GiveExpWaitor());
            //PlayerManager.Instance.GiveEXP(EXPGiven);

            gotHitObj = Instantiate(gotHitPrefab, gotHitPos.transform.position, Quaternion.identity, transform);
            //int index = Random.Range(1, 6);
            //AudioManager.instance.Play("Gore" + index);
        }
        health -= calculatedValue;
        healthSlider.value = health;
    }

    private IEnumerator GiveExpWaitor()
    {
        yield return new WaitForSeconds(2f);
        PlayerManager.Instance.GiveEXP(EXPGiven);
    }
}
