using UnityEngine;
using UnityEngine.UI;
using UnityEngine.AI;
using System.Collections;

public class DemonController : MonoBehaviour
{

    public Slider healthSlider;
    private Animator animator;
    private NavMeshAgent navMeshAgent;
    private GameObject playerObject;

    public float health;
    public float damage = 10f;
    public float maxRange = 2f;

    public float speedRun = 1f;
    private float attackCooldown;

    private bool isAttacking = false;
    private Vector3 lastAttackPosition;

    private float vertical, horizontal;

    public GameObject spawnFireballPos;
    public GameObject castPrefab;
    private GameObject castObj;

    void Start()
    {
        animator = GetComponent<Animator>();
        navMeshAgent = GetComponent<NavMeshAgent>();
        playerObject = GameObject.FindGameObjectWithTag("Player");
        if (playerObject == null) Debug.LogWarning("No object with player tag");

        healthSlider.maxValue = 100;
        healthSlider.value = health;
    }

    void Update()
    {
        Animations();

        if (health <= 0)
        {
            Destroy(gameObject, 4f);
            return;
        }

        Chasing();
    }

    public void Animations()
    {
        animator.SetFloat("vertical", vertical);
        animator.SetFloat("horizontal", horizontal);
        animator.SetFloat("health", health);
    }

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

    private void Chasing()
    {
        if (PlayerManager.Instance.PlayerStatus != PLAYERSTATUS.ALIVE) return;

        float distance = Vector3.Distance(transform.position, playerObject.transform.position);

        if (!isAttacking)
            vertical = horizontal = distance;
        else
            vertical = horizontal = 0;

        // Face the player smoothly
        Vector3 dir = (playerObject.transform.position - transform.position).normalized;
        dir.y = 0f;
        if (dir != Vector3.zero)
        {
            Quaternion lookRot = Quaternion.LookRotation(dir);
            transform.rotation = Quaternion.Slerp(transform.rotation, lookRot, Time.deltaTime * 5f);
        }

        if (distance <= maxRange)
        {
            if (!isAttacking && Time.time > attackCooldown)
            {
                isAttacking = true;

                navMeshAgent.velocity = Vector3.zero; // Stop motion immediately
                navMeshAgent.isStopped = true;

                attackCooldown = Time.time + 2f;

                //Attack(damage);
                CastFireball();
                StartCoroutine(ResumeChaseAfterAttack());
            }

        }
        else if (!isAttacking)
        {
            navMeshAgent.isStopped = false;
            navMeshAgent.speed = speedRun;
            navMeshAgent.SetDestination(playerObject.transform.position);
        }
    }


    private IEnumerator ResumeChaseAfterAttack()
    {
        yield return new WaitForSeconds(2f);
        navMeshAgent.isStopped = false;
        isAttacking = false;
    }


    private void Attack(float damage)
    {
        string att = "attack" + Random.Range(1, 4);
        animator.SetTrigger(att);
        if (isAttacking)
        {
            navMeshAgent.isStopped = true;
            navMeshAgent.velocity = Vector3.zero;

            StartCoroutine(Waiter(0.6f));
        }
    }

    private void CastFireball()
    {
        animator.SetTrigger("magic");
    }

    IEnumerator Waiter(float duration)
    {
        Transform attackPos = GameObject.FindGameObjectWithTag("enemyAttackPos").transform;

        yield return new WaitForSeconds(duration);

        LayerMask playerLayer = LayerMask.GetMask("Player");
        Collider[] hitObjects = Physics.OverlapSphere(attackPos.position, maxRange, playerLayer);

        foreach (Collider obj in hitObjects)
        {
            PlayerController playerControl = obj.GetComponent<PlayerController>();

            if (playerControl != null && !PlayerManager.Instance._playerController.hasHit)
            {
                HitStop.instance.Stop(0.1f);
                playerControl.hasHit = false;
                playerControl.TakeDamage(damage);
            }
        }
    }

    public void CastStart()
    {
        castObj = Instantiate(castPrefab, spawnFireballPos.transform.position, Quaternion.identity);
        Debug.Log("Fireball spawned");
        castObj.transform.parent = spawnFireballPos.transform;
    }

    public void ThrowCast()
    {
        castObj.transform.parent = null;
        castObj.transform.rotation = transform.rotation;
        castObj.GetComponent<SFB_MoveOverTime>().enabled = true;
        Debug.Log("Fireball thrown");
    }

    public void OnDrawGizmosSelected()
    {
        Vector3 center = GameObject.FindGameObjectWithTag("enemyAttackPos").transform.position;
        Gizmos.DrawSphere(center, 1f);
    }

}