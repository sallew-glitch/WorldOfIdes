using UnityEngine;
using UnityEngine.UI;
using UnityEngine.AI;
using System.Collections;

public class DarkLordController : MonoBehaviour
{
    public Slider healthSlider;
    private Animator animator;
    private NavMeshAgent navMeshAgent;
    private GameObject playerObject;

    [Header("Combat Settings")]
    public float health;
    public float damage = 10f;
    public float maxRange = 2f;
    private float attackCooldown;
    private bool isAttacking = false;

    [Header("Movement Settings")]
    public float speedWalk = 1f;
    private float vertical, horizontal;

    [Header("Dash Attack Settings")]
    public float dashMinDistance = 5f;
    public float dashMaxDistance = 10f;
    public float dashChance = 0.3f;
    public float dashCooldownTime = 6f;
    private float nextDashTime = 0f;
    private bool isDashing = false;
    private float dashDuration = 1.0f;     // match this with animation length
    private float dashSpeed = 7f;         // tweak this for desired distance
    private float elapsed = 0f;

    void Start()
    {
        animator = GetComponent<Animator>();
        navMeshAgent = GetComponent<NavMeshAgent>();
        playerObject = GameObject.FindGameObjectWithTag("Player");
        if (playerObject == null) Debug.LogWarning("No object with player tag");

        healthSlider.maxValue = 100;
        healthSlider.value = health;

        animator.applyRootMotion = true;
    }

    void Update()
    {
        Animations();

        if (health <= 0)
        {
            Destroy(gameObject, 4f);
            return;
        }

        TryDashAttack();
        if (!isDashing)
        {
            Chasing();
        }

        if (isDashing)
        {
            Debug.Log("Dashing... ApplyRootMotion = " + animator.applyRootMotion);
        }
    }

    void LateUpdate()
    {
        animator.applyRootMotion = false;
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

        Vector3 dir = (playerObject.transform.position - transform.position).normalized;
        dir.y = 0f;
        if (dir != Vector3.zero && !isAttacking)
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
                Attack(damage);

                StartCoroutine(ResumeChaseAfterAttack(2f));
            }
        }
        else
        {
            if (!isAttacking)
            {
                navMeshAgent.isStopped = false;
                navMeshAgent.speed = speedWalk;
                //navMeshAgent.SetDestination(playerObject.transform.position);
            }
        }

        vertical = navMeshAgent.velocity.magnitude;
    }

    private void TryDashAttack()
    {
        if (Time.time < nextDashTime || isAttacking || isDashing) return;

        float distance = Vector3.Distance(transform.position, playerObject.transform.position);

        if (distance >= dashMinDistance && distance <= dashMaxDistance)
        {
            if (Random.value <= dashChance)
            {
                Debug.Log("Dash");
                StartCoroutine(DashAttackRoutine());
            }
        }
    }

    private IEnumerator DashAttackRoutine()
    {
        isDashing = true;
        isAttacking = true;
        nextDashTime = Time.time + dashCooldownTime;

        navMeshAgent.isStopped = true;

        navMeshAgent.enabled = false;

        // Initial face toward player (can help for animation)
        Vector3 initialDir = (playerObject.transform.position - transform.position).normalized;
        initialDir.y = 0f;
        if (initialDir != Vector3.zero)
            transform.rotation = Quaternion.LookRotation(initialDir);

        animator.SetTrigger("spAttack1");

        yield return new WaitForSeconds(0.3f); // wind-up before moving

        // Face player again before movement
        Vector3 dashDirection = (playerObject.transform.position - transform.position).normalized;
        dashDirection.y = 0f;
        if (dashDirection != Vector3.zero)
            transform.rotation = Quaternion.LookRotation(dashDirection);

        while (elapsed < dashDuration)
        {
            transform.position += transform.forward * dashSpeed * Time.deltaTime;
            elapsed += Time.deltaTime;
            yield return null;
        }

        StartCoroutine(DashHitCheck());

        yield return new WaitForSeconds(0.3f);

        navMeshAgent.enabled = true;

        isDashing = false;
        isAttacking = false;
        navMeshAgent.isStopped = false;
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

    private IEnumerator Waiter(float duration)
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

    private IEnumerator DashHitCheck()
    {
        yield return new WaitForSeconds(0.4f); // impact moment in dash animation

        Transform attackPos = GameObject.FindGameObjectWithTag("enemyAttackPos").transform;

        LayerMask playerLayer = LayerMask.GetMask("Player");
        Collider[] hitObjects = Physics.OverlapSphere(attackPos.position, maxRange, playerLayer);

        foreach (Collider obj in hitObjects)
        {
            PlayerController playerControl = obj.GetComponent<PlayerController>();

            if (playerControl != null && !PlayerManager.Instance._playerController.hasHit)
            {
                HitStop.instance.Stop(0.1f);
                playerControl.hasHit = false;
                playerControl.TakeDamage(damage * 1.1f);
            }
        }
    }

    private IEnumerator ResumeChaseAfterAttack(float duration)
    {
        yield return new WaitForSeconds(duration);
        navMeshAgent.isStopped = false;
        isAttacking = false;
    }

    /*
    public void OnDrawGizmosSelected()
    {
        Vector3 center = GameObject.FindGameObjectWithTag("enemyAttackPos").transform.position;
        Gizmos.DrawSphere(center, 1f);
    }
    */
}
