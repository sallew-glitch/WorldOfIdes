using UnityEngine;
using UnityEngine.UI;
using UnityEngine.AI;
using System.Collections;

public class ReaperController : Enemy
{
    private NavMeshAgent navMeshAgent;
    private GameObject playerObject;

    public float meleeRange = 2.5f;

    public GameObject weaponPos;
    public Weapon BossWeapon;

    private GameObject weapon;

    public float speedWalk = 1f;
    private float attackCooldown;

    private bool isAttacking = false;
    private bool isDashing = false;

    private float vertical, horizontal;

    public GameObject leftHandPos;
    public GameObject castPrefab;
    private GameObject castObj;

    public float dashAndMagicTriggerDistance = 7f;
    public float dashAndMagicCooldown = 10f;
    private float lastSpAttackTime = -Mathf.Infinity;

    // Dash Settings
    public float dashChance = 0.3f;
    public float dashSpeed = 10f;
    public float dashDuration = 0.5f;
    public float dashDamage = 35f;

    // Magic Settings
    public float magicChance = 0.3f;

    // Slam Attack Settings
    public float slamTriggerDistance = 4f;
    public float slamChance = 0.2f;
    public float slamDamage = 30f;
    public float slamRadius = 4f;
    public float slamCooldown = 15f;
    public GameObject slamPrefab;
    private GameObject groundSlamObj;

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

        SpawnWeapon();
    }

    private void SpawnWeapon()
    {
        Debug.Log("It should spawn");
        weapon = BossWeapon.Instantiate();
        weapon.transform.parent = weaponPos.transform;
        weapon.transform.localPosition = Vector3.zero;
        weapon.transform.localEulerAngles = Vector3.zero;
        weapon.transform.localScale = new Vector3(1, 1, 1);
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

        if (distance <= meleeRange)
        {
            if (!isAttacking && !isDashing && Time.time > attackCooldown)
            {
                isAttacking = true;
                navMeshAgent.isStopped = true;
                navMeshAgent.velocity = Vector3.zero;

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
            if (!isAttacking && !isDashing && Time.time >= lastSpAttackTime + dashAndMagicCooldown)
            {
                
                if (distance <= slamTriggerDistance && Random.value < slamChance)
                {
                    StartCoroutine(PerformSlamAttack());
                }
                else if (distance <= dashAndMagicTriggerDistance)
                {
                    if (Random.value < dashChance)
                    {
                        StartCoroutine(PerformDash(dashDuration, dashSpeed));
                    }
                    else if (Random.value < dashChance + magicChance)
                    {
                        MagicAttack();
                    }
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
        attackCooldown = Time.time + 2f;

        int attackIndex = Random.Range(1, 4);
        string att = "attack" + attackIndex;

        animator.SetTrigger(att);
        if (isAttacking)
        {
            navMeshAgent.isStopped = true;
            navMeshAgent.velocity = Vector3.zero;
            StartCoroutine(Waiter(0.6f)); // Single hit delay
        }
    }

    IEnumerator Waiter(float duration)
    {
        Transform attackPos = GameObject.FindGameObjectWithTag("enemyAttackPos").transform;
        yield return new WaitForSeconds(duration);

        LayerMask playerLayer = LayerMask.GetMask("Player");
        Collider[] hitObjects = Physics.OverlapSphere(attackPos.position, meleeRange, playerLayer);

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

    // Dash Function
    private IEnumerator PerformDash(float dashDuration, float dashSpeed)
    {
        navMeshAgent.enabled = false;
        isDashing = true;
        lastSpAttackTime = Time.time;

        animator.SetTrigger("spAttack2");

        yield return new WaitForSeconds(1.0f);

        animator.SetTrigger("spAttack2Valid");

        float elapsed = 0f;
        while (elapsed < dashDuration)
        {
            transform.position += transform.forward * dashSpeed * Time.deltaTime;
            elapsed += Time.deltaTime;
            yield return null;
        }

        DoDashDamage(dashDamage, 2f);
    }

    // Slam Attack Function
    private IEnumerator PerformSlamAttack()
    {
        navMeshAgent.enabled = false;
        isDashing = true;
        lastSpAttackTime = Time.time;

        animator.SetTrigger("spAttack1"); // Slam attack animation trigger

        yield return new WaitForSeconds(1.4f); // Wait for slam to land

        Vector3 slamPosition = transform.position;
        Collider[] hitObjects = Physics.OverlapSphere(slamPosition, slamRadius, LayerMask.GetMask("Player"));

        foreach (Collider obj in hitObjects)
        {
            PlayerController playerControl = obj.GetComponent<PlayerController>();
            if (playerControl != null)
            {
                HitStop.instance.Stop(0.1f);
                playerControl.TakeDamage(slamDamage);
            }
        }

        StartCoroutine(ResetDash(2f));
    }

    public void GroundSlam()
    {
        groundSlamObj = Instantiate(slamPrefab, transform.position, Quaternion.identity, transform);

        Debug.Log("Ground slam");
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

    IEnumerator ResetDash(float duration)
    {
        yield return new WaitForSeconds(duration);
        isDashing = false;
        navMeshAgent.enabled = true;
    }

    public void MagicAttack()
    {
        lastSpAttackTime = Time.time;
        animator.SetTrigger("magic");
    }

    public void MagicStart()
    {
        castObj = Instantiate(castPrefab, leftHandPos.transform.position, Quaternion.identity, leftHandPos.transform);

        //castObj = Instantiate(castPrefab, spawnFireballPos.transform.position, Quaternion.identity);
        Debug.Log("Fireball spawned");
        //castObj.transform.parent = spawnFireballPos.transform;
    }

    public void MagicEnd()
    {
        castObj.transform.parent = null;

        // Look directly at the player
        Vector3 direction = (playerObject.transform.position + new Vector3(0, 1f, 0)) - castObj.transform.position;
        castObj.transform.rotation = Quaternion.LookRotation(direction.normalized);

        //castObj.GetComponent<SFB_MoveOverTime>().enabled = true;
        castObj.GetComponent<MoveOverTime>().enabled = true;
        Debug.Log("Magic thrown");
    }
}
