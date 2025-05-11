using UnityEngine;
using UnityEngine.UI;
using UnityEngine.AI;
using System.Collections;

public class AncientQueenController : Enemy
{
    private NavMeshAgent navMeshAgent;
    private GameObject playerObject;
    public float maxRange = 5f;
    public float speedWalk = 1f;

    private bool isAttacking = false;
    private bool isMagicAttacking = false;

    public GameObject weaponPos;
    public Weapon BossWeapon;

    private GameObject weapon;

    private float attackCooldown;
    private float vertical, horizontal;

    // General Magic Cooldown
    public float spMagicTriggerDistance = 7f;
    public float spMagicCooldown = 10f;
    private float lastSpMagicTime = -Mathf.Infinity;

    // Jutsu Magic Settings
    public float jutsuMagicChance = 0.3f;
    public float jutsuMagicDamage = 50f;

    // Combo Magic Settings
    public float comboMagicChance = 0.3f;
    public float comboMagicDamage = 20f;

    // Defensive Magic Settings
    public float defensiveMagicChance = 0.4f;
    public float defensiveMagicDuration = 1.5f;

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
            AudioManager.instance.Play("FleshPierce" + Random.Range(1, 10));
        }
        else
        {
            AudioManager.instance.Play("Gore" + Random.Range(1, 6));
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
        if (dir != Vector3.zero && !isAttacking && !isMagicAttacking)
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
                Attack(damage);
                StartCoroutine(ResumeChaseAfterAttack(5f));
            }

            TrySpecialMagic(distance);
        }
        else
        {
            TrySpecialMagic(distance);

            if (!isAttacking && !isMagicAttacking)
            {
                navMeshAgent.isStopped = false;
                navMeshAgent.speed = speedWalk;
                navMeshAgent.SetDestination(playerObject.transform.position);
            }
        }

        vertical = navMeshAgent.velocity.magnitude;
    }

    private void TrySpecialMagic(float distance)
    {
        if (!isMagicAttacking && Time.time >= lastSpMagicTime + spMagicCooldown && distance <= spMagicTriggerDistance)
        {
            float roll = Random.value;
            isMagicAttacking = true;
            lastSpMagicTime = Time.time;

            if (roll < jutsuMagicChance)
            {
                StartCoroutine(PerformJutsuMagic());
            }
            else if (roll < jutsuMagicChance + comboMagicChance)
            {
                StartCoroutine(PerformComboMagic());
            }
            else
            {
                StartCoroutine(PerformDefensiveMagic());
            }
        }
    }

    private IEnumerator PerformJutsuMagic()
    {
        Debug.Log("Jutsu Magic Activated");
        animator.SetTrigger("jutsuMagic"); // animation event should trigger actual effect
        yield return new WaitForSeconds(2.75f); // delay for casting
                                                // TODO: Implement Jutsu Magic Effect here (e.g. fireball, lightning, etc)
        Debug.Log("Jutsu Magic Hit");           
        isMagicAttacking = false;
    }

    private IEnumerator PerformComboMagic()
    {
        Debug.Log("Combo Magic Activated");
        animator.SetTrigger("comboMagic");
        yield return new WaitForSeconds(1.75f);
        Debug.Log("First Hit");
        yield return new WaitForSeconds(0.75f);
        Debug.Log("Second Hit");
        yield return new WaitForSeconds(1f);
        Debug.Log("Third Hit");
        // TODO: Implement Combo Magic Effect here (e.g. multiple explosions, pulses)
        Debug.Log("Combo Magic Finished");
        isMagicAttacking = false;
    }

    private IEnumerator PerformDefensiveMagic()
    {
        animator.SetTrigger("spMagic2");
        // TODO: Apply buff, shield, or invincibility here
        yield return new WaitForSeconds(1f); // Wind up time
        
        Debug.Log("Defensive Magic Active");
        
        yield return new WaitForSeconds(defensiveMagicDuration);
        
        // TODO: Remove shield/buff
        Debug.Log("Defensive Magic Ended");
        isMagicAttacking = false;
    }

    private IEnumerator ResumeChaseAfterAttack(float duration)
    {
        yield return new WaitForSeconds(duration);
        navMeshAgent.isStopped = false;
        isAttacking = false;
    }

    private void Attack(float damage)
    {
        attackCooldown = Time.time + 5f;

        int attackIndex = Random.Range(1, 3);
        animator.SetTrigger("attack" + attackIndex);

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
}