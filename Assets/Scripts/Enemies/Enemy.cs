using UnityEngine;
using UnityEngine.UI;
public abstract class Enemy : MonoBehaviour 
{
    public float health;
    public float damage;

    public int EXPGiven;

    public Slider healthSlider;
    protected Animator animator;

    public void RecieveDamage(float value)
    {
        float calculatedValue = value + (1 + PlayerManager.Instance.Strength * 0.05f);

        if (health > calculatedValue)
        {
            animator.SetTrigger("takeDamage");
            int index = Random.Range(1, 10);
            AudioManager.instance.Play("FleshPierce" + index);
        }
        else
        {
            int index = Random.Range(1, 6);
            PlayerManager.Instance.GiveEXP(EXPGiven);
            AudioManager.instance.Play("Gore" + index);
        }
        health -= calculatedValue;
        healthSlider.value = health;
    }
}
