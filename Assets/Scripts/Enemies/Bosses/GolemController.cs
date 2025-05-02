using UnityEngine;
using UnityEngine.UI;
using UnityEngine.AI;
using System.Collections;

public class GolemController : MonoBehaviour
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

    public float stompRange = 1.0f;
    public Transform leftFoot;
    public Transform rightFoot;
    public float stompDamage = 20f;

    public float slamRange = 4f;
    public float slamDamage = 25f;
    public float slamCooldownTime = 10f;

    private float slamCooldown = 0f;


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

        if (slamCooldown > 0f)
            slamCooldown -= Time.deltaTime;
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
        float distance = Vector3.Distance(transform.position, playerObject.transform.position);

        // Face the player smoothly
        Vector3 dir = (playerObject.transform.position - transform.position).normalized;
        dir.y = 0f;
        if (dir != Vector3.zero && !isAttacking)
        {
            Quaternion lookRot = Quaternion.LookRotation(dir);
            transform.rotation = Quaternion.Slerp(transform.rotation, lookRot, Time.deltaTime * 5f);
        }

        // Handle attack logic only if not currently attacking
        if (!isAttacking && Time.time > attackCooldown)
        {
            if (distance <= stompRange)
            {
                // Foot stomp if very close
                isAttacking = true;
                navMeshAgent.isStopped = true;
                navMeshAgent.velocity = Vector3.zero;

                attackCooldown = Time.time + 3f;
                DoStomp();

                Debug.Log("Stomp");

                StartCoroutine(ResumeChaseAfterAttack(2f));
            }
            else if (distance <= 3.5f)
            {
                // Melee attack
                isAttacking = true;
                navMeshAgent.isStopped = true;
                navMeshAgent.velocity = Vector3.zero;

                attackCooldown = Time.time + 2f;
                Attack(damage);

                Debug.Log("Melee");

                StartCoroutine(ResumeChaseAfterAttack(2f));
            }
            else if (distance <= maxRange && slamCooldown <= 0f)
            {
                

                // Special AOE slam attack if in medium range
                isAttacking = true;
                navMeshAgent.isStopped = true;
                navMeshAgent.velocity = Vector3.zero;

                slamCooldown = slamCooldownTime;
                attackCooldown = Time.time + 3f;
                DoSlamAttack();

                Debug.Log("Slam");

                StartCoroutine(ResumeChaseAfterAttack(2f));
            }
        }

        // If not attacking, chase the player
        if (distance > maxRange && !isAttacking)
        {
            navMeshAgent.isStopped = false;
            navMeshAgent.speed = speedRun;
            navMeshAgent.SetDestination(playerObject.transform.position);
        }

        // Animation blend control
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

    private void DoStomp()
    {
        // Determine which foot is closer
        float leftDist = Vector3.Distance(playerObject.transform.position, leftFoot.position);
        float rightDist = Vector3.Distance(playerObject.transform.position, rightFoot.position);

        string stompAnim = leftDist < rightDist ? "stompLeft" : "stompRight";
        animator.SetTrigger(stompAnim);

        StartCoroutine(StompWaiter(0.5f, stompDamage));
    }

    IEnumerator StompWaiter(float delay, float damage)
    {
        yield return new WaitForSeconds(delay);

        LayerMask playerLayer = LayerMask.GetMask("Player");
        Collider[] hitObjects = Physics.OverlapSphere(transform.position, stompRange, playerLayer);

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

    private void DoSlamAttack()
    {

        animator.SetTrigger("slam" + Random.Range(1, 3));
        StartCoroutine(SlamWaiter(0.8f, slamDamage));
    }

    IEnumerator SlamWaiter(float delay, float damage)
    {
        yield return new WaitForSeconds(delay);

        LayerMask playerLayer = LayerMask.GetMask("Player");
        Collider[] hitObjects = Physics.OverlapSphere(transform.position, slamRange, playerLayer);

        foreach (Collider obj in hitObjects)
        {
            PlayerController playerControl = obj.GetComponent<PlayerController>();

            if (playerControl != null && !PlayerManager.Instance._playerController.hasHit)
            {
                HitStop.instance.Stop(0.15f);
                playerControl.hasHit = false;
                playerControl.TakeDamage(damage);
            }
        }
    }

    public void OnDrawGizmosSelected()
    {
        Vector3 center = GameObject.FindGameObjectWithTag("enemyAttackPos").transform.position;
        Gizmos.DrawSphere(center, 1f);
    }
    
}
