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

    // Jump Dash Settings
    public float jumpDashTriggerDistance = 8f;
    public float jumpDashChance = 0.15f;
    public float jumpDashDamage = 80f;
    public float jumpDuration = 0.5f;
    public float slamDuration = 0.4f;

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

            // Dash decision logic
            if (!isAttacking && !isDashing && Time.time >= lastDashTime + dashCooldown)
            {
                if (distance <= jumpDashTriggerDistance && Random.value < jumpDashChance)
                {
                    StartCoroutine(PerformJumpDash());
                }
                else if (distance <= dashTriggerDistance && Random.value < dashChance)
                {
                    int variant = Random.Range(0, 2); // 0 = normal, 1 = combo
                    if (variant == 0)
                        StartCoroutine(PerformDash(dashDuration, dashSpeed));
                    else
                        StartCoroutine(PerformDashCombo(dashDuration, dashSpeed));
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

    IEnumerator PerformDash(float dashDuration, float dashSpeed)
    {
        navMeshAgent.enabled = false;
        isDashing = true;
        lastDashTime = Time.time;

        animator.SetTrigger("spAttack1");

        yield return new WaitForSeconds(1.5f);

        float elapsed = 0f;
        while (elapsed < dashDuration)
        {
            transform.position += transform.forward * dashSpeed * Time.deltaTime;
            elapsed += Time.deltaTime;
            yield return null;
        }

        DoDashDamage(dashDamage, 2f);
    }

    IEnumerator PerformDashCombo(float dashDuration, float dashSpeed)
    {
        navMeshAgent.enabled = false;
        isDashing = true;
        lastDashTime = Time.time;

        animator.SetTrigger("spAttack2");

        yield return new WaitForSeconds(1.5f);

        float elapsed = 0f;
        while (elapsed < dashDuration)
        {
            transform.position += transform.forward * dashSpeed * Time.deltaTime;
            elapsed += Time.deltaTime;
            yield return null;
        }

        yield return DoComboHits(dashComboDamage, 2f);
    }


    private IEnumerator PerformJumpDash()
    {
        navMeshAgent.enabled = false;
        isDashing = true;
        lastDashTime = Time.time;

        animator.SetTrigger("spAttack4"); // Your jumping dash animation

        yield return new WaitForSeconds(0.5f);

        Vector3 startPos = transform.position;
        Vector3 targetPos = playerObject.transform.position;
        targetPos.y = startPos.y; // keep y level consistent during forward motion

        Vector3 direction = (targetPos - startPos).normalized;
        float jumpHeight = 4f;

        float elapsed = 0f;

        // Phase 1: Jump arc toward target
        while (elapsed < jumpDuration)
        {
            float t = elapsed / jumpDuration;

            // Horizontal movement toward the player
            Vector3 move = Vector3.Lerp(startPos, targetPos, t);

            // Add vertical arc using sine wave
            float heightOffset = Mathf.Sin(t * Mathf.PI) * jumpHeight;
            move.y += heightOffset;

            transform.position = move;

            elapsed += Time.deltaTime;
            yield return null;
        }

        // Ensure final position is right at targetPos (on the ground)
        transform.position = targetPos;

        DoJumpDashDamage(jumpDashDamage, 3f);
    }

    void DoDashDamage(float damageAmount, float radius)
    {
        Transform attackPos = GameObject.FindGameObjectWithTag("enemyAttackPos").transform;
        LayerMask playerLayer = LayerMask.GetMask("Player");
        Collider[] hitObjects = Physics.OverlapSphere(attackPos.position, radius, playerLayer);

        foreach (Collider obj in hitObjects)
        {
            PlayerController playerControl = obj.GetComponent<PlayerController>();
            if (playerControl != null && !PlayerManager.Instance._playerController.hasHit)
            {
                HitStop.instance.Stop(0.1f);
                playerControl.hasHit = false;
                playerControl.TakeDamage(damageAmount);
            }
        }

        StartCoroutine(ResetDash(2f));
    }

    IEnumerator DoComboHits(float damageAmount, float radius)
    {
        Transform attackPos = GameObject.FindGameObjectWithTag("enemyAttackPos").transform;
        LayerMask playerLayer = LayerMask.GetMask("Player");

        for (int i = 0; i < 2; i++)
        {
            Collider[] hitObjects = Physics.OverlapSphere(attackPos.position, radius, playerLayer);
            foreach (Collider obj in hitObjects)
            {
                PlayerController playerControl = obj.GetComponent<PlayerController>();
                if (playerControl != null && !PlayerManager.Instance._playerController.hasHit)
                {
                    HitStop.instance.Stop(0.1f);
                    playerControl.hasHit = false;
                    playerControl.TakeDamage(damageAmount);
                }
            }
            yield return new WaitForSeconds(0.5f);
        }

        StartCoroutine(ResetDash(2f));
    }

    void DoJumpDashDamage(float damageAmount, float radius)
    {
        // Phase 2: Damage after landing
        Transform attackPos = GameObject.FindGameObjectWithTag("enemyAttackPos").transform;
        LayerMask playerLayer = LayerMask.GetMask("Player");
        Collider[] hitObjects = Physics.OverlapSphere(attackPos.position, radius, playerLayer);

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

        StartCoroutine(ResetDash(3f));
    }

    IEnumerator ResetDash(float duration)
    {
        yield return new WaitForSeconds(duration);
        isDashing = false;
        navMeshAgent.enabled = true;
    }
}
