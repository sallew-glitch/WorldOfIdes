using UnityEngine;
using UnityEngine.UI;
using UnityEngine.AI;
using System.Collections;

public class SkeletonMageController : Enemy
{

    private NavMeshAgent navMeshAgent;
    private GameObject playerObject;

    public float maxRange = 6f;

    public GameObject weaponPos;
    public GameObject EnemyWeapon;

    private GameObject weapon;

    public int characterState;
    private float horizontal, vertical;        

    private float attackCooldown;    
    
    public float startWaitTime = 4;
    public float timeToRotate = 2;
    public float speedWalk = 1.5f;
    public float speedRun = 3;

    private float viewRadius = 15;
    private float viewAngle = 90;
    public LayerMask playerMask;
    public LayerMask obstacleMask;

    public Transform[] waypoints; 
    int m_currentWayPointIndex;

    Vector3 playerLastPosition = Vector3.zero;
    Vector3 m_PlayerPosition;

    float m_WaitTime;
    float m_TimeToRotate;
    bool m_PlayerInRange;
    bool m_PlayerNear;
    bool m_IsPatrol;
    bool m_CaughtPlayer;

    public GameObject rightHandPos;
    public GameObject attack1Prefab;
    private GameObject attack1Obj;

    public GameObject attack2Prefab;
    private GameObject attack2Obj;

    private void Awake()
    {
        health = 100f;
        damage = 10f;
    }
    void Start()
    {
        animator = GetComponent<Animator>();
        navMeshAgent = GetComponent<NavMeshAgent>();
        playerObject = GameObject.FindGameObjectWithTag("Player");
        if (playerObject == null) print("No object with player tag");

        healthSlider.maxValue = 100;
        healthSlider.value = health;
      
        m_PlayerPosition = Vector3.zero;
        m_IsPatrol = true;
        m_CaughtPlayer = false;
        m_PlayerInRange = false;
        m_WaitTime = startWaitTime;
        m_TimeToRotate = timeToRotate;

        m_currentWayPointIndex = 0;
        navMeshAgent.isStopped = false;
        navMeshAgent.speed = speedWalk;

        if(waypoints.Length > 0)
            navMeshAgent.SetDestination(waypoints[m_currentWayPointIndex].position);

        SpawnWeapon();
    }

    private void SpawnWeapon()
    {
        weapon = Instantiate(EnemyWeapon);
        
        if (weapon == null)
        {
            Debug.LogError("Weapon instantiation failed: returned null.");
            return;
        }

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
        }
        
        EnvironmentView();

        if (!m_IsPatrol)
        {
            Chasing();
        }
        else
        {
            Patroling();
        }
    }

    public void Animations()
    {
        animator.SetFloat("vertical", vertical);
        animator.SetFloat("horizontal", horizontal);
        animator.SetInteger("state", characterState);
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

    //ai video
    public void CaughtPlayer()
    {
        m_CaughtPlayer = true;
    }

    void Move(float speed)
    {
        navMeshAgent.isStopped = false;
        navMeshAgent.speed = speed;
    }

    void Stop()
    {
        navMeshAgent.isStopped = true;
        navMeshAgent.speed = 0;
    }

    private void Chasing()
    {
        if (!m_PlayerInRange) // Check if the player is still in range
        {
            m_IsPatrol = true; // Return to patrol if the player is not in range
            return;
        }

        m_PlayerNear = false;
        playerLastPosition = Vector3.zero;

        if (!m_CaughtPlayer && PlayerManager.Instance.PlayerStatus == PLAYERSTATUS.ALIVE)
        {
            Move(speedRun);
            navMeshAgent.SetDestination(m_PlayerPosition);

            vertical = navMeshAgent.velocity.magnitude;

            float distanceToPlayer = Vector3.Distance(transform.position, m_PlayerPosition);

            // Normal melee attack
            if (distanceToPlayer <= maxRange)
            {
                navMeshAgent.isStopped = true;

                if (Time.time > attackCooldown)
                {
                    //string att = "attack" + Random.Range(1, 3);
                    string att = "attack" + 2;
                    animator.SetTrigger(att);
                    Debug.Log(att);
                    attackCooldown = Time.time + 4f;
                    //Attack(damage);
                }
            }
        }

        // If the NPC reaches the last known position of the player
        if (navMeshAgent.remainingDistance <= navMeshAgent.stoppingDistance)
        {
            if (m_WaitTime <= 0 && !m_CaughtPlayer)
            {
                m_IsPatrol = true;
                Move(speedWalk);
                navMeshAgent.SetDestination(waypoints[m_currentWayPointIndex].position);
                m_WaitTime = startWaitTime;
            }
            else
            {
                Stop();
                m_WaitTime -= Time.deltaTime;
            }
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
            //Rigidbody enemyRb = obj.GetComponent<Rigidbody>();

            if (playerControl != null && !PlayerManager.Instance._playerController.hasHit)
            {
                // Calculate knockback direction
                /*
                Vector3 knockbackDirection = obj.transform.position - transform.position;
                knockbackDirection.y = airknockbackForce; // Keep the knockback horizontal
                */

                // Get collision point
                //Vector3 collisionPoint = obj.ClosestPoint(attackPos.position);
                //enemyControl.SpawnHitVfx(collisionPoint); //VFX for attack

                //hitstop
                HitStop.instance.Stop(0.1f);

                playerControl.hasHit = false;
                playerControl.TakeDamage(damage);

                //enemyRb.AddForce(knockbackDirection.normalized * knockbackForce, ForceMode.Impulse);
            }
        }
    }

    public void Attack1Start()
    {
        attack1Obj = Instantiate(attack1Prefab, rightHandPos.transform.position, Quaternion.identity, rightHandPos.transform);

        Debug.Log("Normal attack 1");
    }

    public void Attack1Throw()
    {
        attack1Obj.transform.parent = null;

        // Look directly at the player
        Vector3 direction = (playerObject.transform.position + new Vector3(0, 1f, 0)) - attack1Obj.transform.position;
        attack1Obj.transform.rotation = Quaternion.LookRotation(direction.normalized);

        //castObj.GetComponent<SFB_MoveOverTime>().enabled = true;
        attack1Obj.GetComponent<MoveOverTime>().enabled = true;
    }

    public void Attack2()
    {
        Vector3 spawnOffset = transform.forward * 1.5f; // optional offset in front of enemy
        Vector3 spawnPos = transform.position + spawnOffset;

        // Rotate so the prefab's X+ faces same direction as enemy's forward
        Quaternion rotation = Quaternion.LookRotation(transform.forward) * Quaternion.Euler(0, -90, 0);

        attack2Obj = Instantiate(attack2Prefab, spawnPos, rotation);


        StartCoroutine(Waiter(0.5f));
        
        Debug.Log("Normal attack 2");
    }

    private void Patroling()
    {
        if (m_PlayerNear) // Check if the player was recently detected
        {
            if (m_TimeToRotate <= 0)
            {
                Move(speedWalk);
                LookingPlayer(playerLastPosition);
            }
            else
            {
                Stop();
                m_TimeToRotate -= Time.deltaTime;
            }
        }
        else
        {
            m_PlayerNear = false;
            playerLastPosition = Vector3.zero;

            // Ensure the NPC is moving towards the current waypoint
            if (waypoints.Length > 0)
            {
                navMeshAgent.SetDestination(waypoints[m_currentWayPointIndex].position);
            }

            vertical = navMeshAgent.velocity.magnitude;

            // Check if the NPC has reached the current waypoint
            if (!navMeshAgent.pathPending && navMeshAgent.remainingDistance <= navMeshAgent.stoppingDistance + 0.1f)
            {
                if (m_WaitTime <= 0)
                {
                    NextPoint(); // Move to the next waypoint
                    Move(speedWalk);
                    m_WaitTime = startWaitTime; // Reset wait time
                }
                else
                {
                    Stop();
                    m_WaitTime -= Time.deltaTime; // Wait before moving to the next point
                }
            }
        }
    }

    void NextPoint()
    {
        if (waypoints.Length == 0) return;
        
        m_currentWayPointIndex = (m_currentWayPointIndex + 1) % waypoints.Length;
        
        navMeshAgent.SetDestination(waypoints[m_currentWayPointIndex].position);
    }



    void LookingPlayer(Vector3 player)
    {
        navMeshAgent.SetDestination(player);

        vertical = navMeshAgent.velocity.magnitude;

        if (Vector3.Distance(transform.position, player) <= 0.3)
        {
            if (m_WaitTime <= 0)
            {
                m_PlayerNear = false;
                Move(speedWalk);
                navMeshAgent.SetDestination(waypoints[m_currentWayPointIndex].position);

                vertical = navMeshAgent.velocity.magnitude;

                m_WaitTime = startWaitTime;
                m_TimeToRotate = timeToRotate;
            }
            else
            {
                Stop();
                m_WaitTime -= Time.deltaTime;
            }
        }
    }

    void EnvironmentView()
    {
        Collider[] playerInRange = Physics.OverlapSphere(transform.position, viewRadius, playerMask);

        m_PlayerInRange = false;

        for (int i = 0; i < playerInRange.Length; i++)
        {
            Transform player = playerInRange[i].transform;
            Vector3 dirToPlayer = (player.position - transform.position).normalized;
            
            if (Vector3.Angle(transform.forward, dirToPlayer) < viewAngle / 2)
            {
                float dstToPlayer = Vector3.Distance(transform.position, player.position);
                
                if (!Physics.Raycast(transform.position, dirToPlayer, dstToPlayer, obstacleMask))
                {
                    m_PlayerInRange = true;
                    m_IsPatrol = false;
                    m_PlayerPosition = player.position; 
                    return; 
                }
            }
        }
        
        if (!m_PlayerInRange)
        {
            m_IsPatrol = true;
            m_PlayerPosition = Vector3.zero; 
        }
    }

    public void OnDrawGizmosSelected()
    {
        Vector3 center = GameObject.FindGameObjectWithTag("enemyAttackPos").transform.position;
        Gizmos.DrawSphere(center, 1f);
    }
}
