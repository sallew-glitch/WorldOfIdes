using UnityEngine;
using UnityEngine.UI;
public abstract class Enemy : MonoBehaviour 
{
    public float health;
    public float damage;

    public Slider healthSlider;
    protected Animator animator;

    public GameObject enemyAttackPos;

    public void RecieveDamage(float value)
    {
        if (health > value)
        {
            animator.SetTrigger("takeDamage");
            int index = Random.Range(1, 10);
            AudioManager.instance.Play("FleshPierce" + index);
        }
        else
        {
            int index = Random.Range(1, 6);
            AudioManager.instance.Play("Gore" + index);
        }
        health -= value;
        healthSlider.value = health;
    }
}
