using UnityEngine;
using UnityEngine.UI;
using UnityEngine.AI;
using System.Collections;

public class DemonControllerAnother : Enemy
{
    
    private NavMeshAgent navMeshAgent;
    private GameObject playerObject;

    public float maxRange = 2f;

    public float speedRun = 1f;
    private float attackCooldown;

    private bool isAttacking = false;
    private Vector3 lastAttackPosition;

    private float vertical, horizontal;

    public GameObject spawnFireballPos;
    public GameObject castPrefab;
    private GameObject castObj;

    private float nextFireballTime = 0f;
    public float fireballCooldown = 3f;
    public float fireballChance = 0.25f; // 25% chance every interval

    private void Awake()
    {
        damage = 40f;
    }

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
                navMeshAgent.isStopped = true;
                navMeshAgent.velocity = Vector3.zero;

                attackCooldown = Time.time + 2f;
                Attack(damage); // or CastFireball(); depending on your preferred melee style

                StartCoroutine(ResumeChaseAfterAttack(2f));
            }
        }
        else
        {
            if (!isAttacking)
            {
                navMeshAgent.isStopped = false;
                navMeshAgent.speed = speedRun;
                navMeshAgent.SetDestination(playerObject.transform.position);

                // Fireball logic
                if (Time.time >= nextFireballTime)
                {
                    nextFireballTime = Time.time + fireballCooldown;

                    if (Random.value <= fireballChance)
                    {
                        isAttacking = true;
                        navMeshAgent.isStopped = true;
                        navMeshAgent.velocity = Vector3.zero;
                        CastFireball();
                    }

                    StartCoroutine(ResumeChaseAfterAttack(1f));
                }
            }
        }

        vertical = horizontal = isAttacking ? 0f : distance;
    }


    private IEnumerator ResumeChaseAfterAttack(float duration)
    {
        yield return new WaitForSeconds(duration);
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
        Debug.Log("Firing fireball");
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
        castObj = Instantiate(castPrefab, spawnFireballPos.transform.position, Quaternion.identity, spawnFireballPos.transform);

        //castObj = Instantiate(castPrefab, spawnFireballPos.transform.position, Quaternion.identity);
        Debug.Log("Fireball spawned");
        //castObj.transform.parent = spawnFireballPos.transform;
    }

    public void ThrowCast()
    {
        castObj.transform.parent = null;

        // Look directly at the player
        Vector3 direction = (playerObject.transform.position + new Vector3(0, 1f, 0)) - castObj.transform.position;
        castObj.transform.rotation = Quaternion.LookRotation(direction.normalized);

        //castObj.GetComponent<SFB_MoveOverTime>().enabled = true;
        castObj.GetComponent<MoveOverTime>().enabled = true;
        Debug.Log("Fireball thrown");
    }


    public void OnDrawGizmosSelected()
    {
        Vector3 center = GameObject.FindGameObjectWithTag("enemyAttackPos").transform.position;
        Gizmos.DrawSphere(center, 1f);
    }
    
}
