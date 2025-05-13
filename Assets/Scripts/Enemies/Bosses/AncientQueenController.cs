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

    public GameObject rightHandPos;
    public GameObject leftHandPos;
    
    public GameObject attack1Prefab;
    private GameObject attack1Obj;

    public GameObject attack2Prefab;
    public GameObject attack2PrefabStart;
    private GameObject attack2Obj;

    public GameObject comboPrefab;
    private GameObject comboObj;

    public GameObject jutsuSpawnPos;
    public GameObject jutsuPrefab;
    private GameObject jutsuObj;

    // General Magic Cooldown
    public float spMagicTriggerDistance = 7f;
    public float spMagicCooldown = 10f;
    private float lastSpMagicTime = -Mathf.Infinity;

    // Jutsu Magic Settings
    public float jutsuMagicChance = 0.35f;

    // Combo Magic Settings
    public float comboMagicChance = 0.35f;

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
                //StartCoroutine(PerformJutsuMagic());
                animator.SetTrigger("spMagic1");
                isMagicAttacking = false;
            }
            else if (roll < jutsuMagicChance + comboMagicChance)
            {
                //StartCoroutine(PerformComboMagic());
                animator.SetTrigger("spMagic3");
                isMagicAttacking = false;
            }
        }
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

        //int attackIndex = Random.Range(1, 3);
        int attackIndex = 1;
        animator.SetTrigger("attack" + attackIndex);

        if (isAttacking)
        {
            navMeshAgent.isStopped = true;
            navMeshAgent.velocity = Vector3.zero;
            //StartCoroutine(Waiter(0.6f));
        }
    }

    public void NormalAttack1Start()
    {
        attack1Obj = Instantiate(attack1Prefab, leftHandPos.transform.position, Quaternion.identity, leftHandPos.transform);

        Debug.Log("Normal attack 1");
    }

    public void NormalAttack1End()
    {
        attack1Obj.transform.parent = null;

        // Look directly at the player
        Vector3 direction = (playerObject.transform.position + new Vector3(0, 1f, 0)) - attack1Obj.transform.position;
        attack1Obj.transform.rotation = Quaternion.LookRotation(direction.normalized);

        //castObj.GetComponent<SFB_MoveOverTime>().enabled = true;
        attack1Obj.GetComponent<MoveOverTime>().enabled = true;
    }

    public void NormalAttack2Start()
    {
        GameObject beamStart = Instantiate(attack2PrefabStart, leftHandPos.transform.position, Quaternion.identity);
        GameObject beam = Instantiate(attack2Prefab, leftHandPos.transform.position, Quaternion.identity);
        Vector3 direction = (playerObject.transform.position + new Vector3(0, 1f, 0)) - beam.transform.position;
        beam.transform.rotation = Quaternion.LookRotation(direction.normalized);

        Debug.Log("Normal attack 2");
        
    }

    public void NormalAttack2End()
    {
        /*
        attack2Obj.transform.parent = null;

        // Look directly at the player
        Vector3 direction = (playerObject.transform.position + new Vector3(0, 1f, 0)) - attack2Obj.transform.position;
        attack2Obj.transform.rotation = Quaternion.LookRotation(direction.normalized);

        //castObj.GetComponent<SFB_MoveOverTime>().enabled = true;
        attack2Obj.GetComponent<MoveOverTime>().enabled = true;
        */
    }

    public void StartBeamAttack()
    {
        GameObject beam = Instantiate(attack2Prefab, leftHandPos.transform.position, Quaternion.identity);
        Vector3 direction = (playerObject.transform.position + new Vector3(0, 1f, 0)) - beam.transform.position;
        beam.transform.rotation = Quaternion.LookRotation(direction.normalized);
    }


    public void ComboFirstAttack()
    {
        comboObj = Instantiate(comboPrefab, leftHandPos.transform.position, Quaternion.identity, leftHandPos.transform);
        
        Debug.Log("combo 1");

        comboObj.transform.parent = null;

        // Look directly at the player
        Vector3 direction = (playerObject.transform.position + new Vector3(0, 1f, 0)) - comboObj.transform.position;
        comboObj.transform.rotation = Quaternion.LookRotation(direction.normalized);

        //castObj.GetComponent<SFB_MoveOverTime>().enabled = true;
        comboObj.GetComponent<MoveOverTime>().enabled = true;
        
    }

    public void ComboSecondAttack()
    {
        comboObj = Instantiate(comboPrefab, rightHandPos.transform.position, Quaternion.identity, rightHandPos.transform);

        Debug.Log("combo 2");

        comboObj.transform.parent = null;

        // Look directly at the player
        Vector3 direction = (playerObject.transform.position + new Vector3(0, 1f, 0)) - comboObj.transform.position;
        comboObj.transform.rotation = Quaternion.LookRotation(direction.normalized);

        //castObj.GetComponent<SFB_MoveOverTime>().enabled = true;
        comboObj.GetComponent<MoveOverTime>().enabled = true;
    }

    public void ComboThirdAttack()
    {
        comboObj = Instantiate(comboPrefab, leftHandPos.transform.position, Quaternion.identity, leftHandPos.transform);

        Debug.Log("combo 3");

        comboObj.transform.parent = null;

        // Look directly at the player
        Vector3 direction = (playerObject.transform.position + new Vector3(0, 1f, 0)) - comboObj.transform.position;
        comboObj.transform.rotation = Quaternion.LookRotation(direction.normalized);

        //castObj.GetComponent<SFB_MoveOverTime>().enabled = true;
        comboObj.GetComponent<MoveOverTime>().enabled = true;
    }

    public void JutsuStart()
    {
        jutsuObj = Instantiate(jutsuPrefab, jutsuSpawnPos.transform.position, Quaternion.identity, jutsuSpawnPos.transform);

        Debug.Log("jutsu");
    }

    public void JutsuEnd()
    {
        jutsuObj.transform.parent = null;

        // Look directly at the player
        Vector3 direction = (playerObject.transform.position + new Vector3(0, 1f, 0)) - jutsuObj.transform.position;
        jutsuObj.transform.rotation = Quaternion.LookRotation(direction.normalized);

        //castObj.GetComponent<SFB_MoveOverTime>().enabled = true;
        jutsuObj.GetComponent<MoveOverTime>().enabled = true;
    }
}