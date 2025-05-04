using UnityEngine;
using UnityEngine.UI;
using UnityEngine.AI;
using System.Collections;

public class DarkLordControllerAnother : MonoBehaviour
{
    public Slider healthSlider;
    private Animator animator;
    private NavMeshAgent navMeshAgent;
    private GameObject playerObject;

    public float health;
    public float damage = 40f;
    public float maxRange = 2.5f;

    public float speedWalk = 1f;
    private float attackCooldown;

    private bool isAttacking = false;
    private bool isDashing = false;
    private Vector3 lastAttackPosition;

    private float vertical, horizontal;

    // Dash Settings
    public float dashTriggerDistance = 5f;
    public float dashChance = 0.3f;
    public float dashSpeed = 10f;
    public float dashDuration = 0.6f;
    public float dashCooldown = 10f;
    private float lastDashTime = -Mathf.Infinity;
    public float dashDamage = 60f;
    public float dashComboDamage = 25f;

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
            animator.SetTrigger("takeDamage" + Random.Range(1, 4));
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

        // Face the player
        Vector3 dir = (playerObject.transform.position - transform.position).normalized;
        dir.y = 0f;
        if (dir != Vector3.zero && !isAttacking && !isDashing)
        {
            Quaternion lookRot = Quaternion.LookRotation(dir);
            transform.rotation = Quaternion.Slerp(transform.rotation, lookRot, Time.deltaTime * 5f);
        }

        if (distance <= maxRange)
        {
            if (!isAttacking && !isDashing && Time.time > attackCooldown)
            {
                isAttacking = true;
                navMeshAgent.isStopped = true;
                navMeshAgent.velocity = Vector3.zero;

                attackCooldown = Time.time + 2f;
                Attack(damage);

                StartCoroutine(ResumeChaseAfterAttack(2f));
            }
        }
        else
        {
            if (!isAttacking && !isDashing)
            {
                navMeshAgent.isStopped = false;
                navMeshAgent.speed = speedWalk;
                navMeshAgent.SetDestination(playerObject.transform.position);
            }

            // Dash logic with cooldown
            if (!isAttacking && !isDashing && Time.time >= lastDashTime + dashCooldown)
            {
                if (distance <= dashTriggerDistance && Random.value < dashChance)
                {
                    StartCoroutine(PerformDash(dashDuration, dashSpeed));
                }
            }
        }

        vertical = navMeshAgent.velocity.magnitude;
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

    // Dash Coroutine with cooldown tracking
    IEnumerator PerformDash(float dashDuration, float dashSpeed)
    {
        navMeshAgent.enabled = false;
        isDashing = true;
        lastDashTime = Time.time;

        bool isComboDash = Random.value < 0.5f; // 50% chance for combo dash
        string dashTrigger = isComboDash ? "spAttack2" : "spAttack1";
        animator.SetTrigger(dashTrigger);

        yield return new WaitForSeconds(1.5f); // Windup before dash

        float elapsed = 0f;
        while (elapsed < dashDuration)
        {
            transform.position += transform.forward * dashSpeed * Time.deltaTime;
            elapsed += Time.deltaTime;
            yield return null;
        }

        Transform attackPos = GameObject.FindGameObjectWithTag("enemyAttackPos").transform;
        float dashHitRange = 2f;
        LayerMask playerLayer = LayerMask.GetMask("Player");

        if (isComboDash)
        {
            // First hit
            DealDashDamage(attackPos, dashHitRange, playerLayer, dashComboDamage);
            yield return new WaitForSeconds(0.5f); // Delay before second hit
            
            // Second hit
            DealDashDamage(attackPos, dashHitRange, playerLayer, dashComboDamage);
        }
        else
        {
            // Regular dash single hit
            DealDashDamage(attackPos, dashHitRange, playerLayer, dashDamage);
        }

        yield return new WaitForSeconds(2f);
        isDashing = false;
        navMeshAgent.enabled = true;
    }

    private void DealDashDamage(Transform attackPos, float range, LayerMask layer, float damage)
    {
        Collider[] hitObjects = Physics.OverlapSphere(attackPos.position, range, layer);
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

}
