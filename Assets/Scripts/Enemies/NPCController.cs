using UnityEngine;
using UnityEngine.UI;
using UnityEngine.AI;
using System.Collections;

public class NPCController : Enemy
{

    private NavMeshAgent navMeshAgent;
    private GameObject playerObject;

    public float roamCooldownTime;
    public float maxRoamDistance;

    public float maxRange = 2f;

    public int characterState;
    private float horizontal, vertical;        

    private float attackCooldown;    
    
    public float startWaitTime = 4;
    public float timeToRotate = 2;
    public float speedWalk = 1;
    public float speedRun = 2;

    public float viewRadius = 15;
    public float viewAngle = 90;
    public LayerMask playerMask;
    public LayerMask obstacleMask;
    public float meshResolution = 1f;
    public int edgeIterations = 4;
    public float edgeDistance = 0.5f;

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

    bool freeze = false;

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
        navMeshAgent.SetDestination(waypoints[m_currentWayPointIndex].position);

    }
    
    void Update()
    {
        if (freeze)
            return;

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

    public void Freeze(bool val)
    {
        freeze = val;
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

            vertical = transform.position.x - m_PlayerPosition.x;
            horizontal = transform.position.z - m_PlayerPosition.z;

            // Check distance to player for attack
            if (Vector3.Distance(transform.position, m_PlayerPosition) <= 2)
            {
                navMeshAgent.isStopped = true;

                if (Time.time > attackCooldown)
                {
                   string att = "attack" + Random.Range(1, 3);
                    animator.SetTrigger(att);
                    Debug.Log(att);
                    attackCooldown = Time.time + 2f;
                    Attack(damage);
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

    private void Attack(float damage)
    {
        //Attack audio
        //int index = Random.Range(1, 8);
        //AudioManager.instance.Play("Swipe" + index);

        StartCoroutine(Waiter(0.6f));

        // Add your player damage logic here if applicable
        
    }

    IEnumerator Waiter(float duration)
    {
        Transform attackPos = GameObject.FindGameObjectWithTag("enemyAttackPos").transform;

        yield return new WaitForSeconds(duration);

        LayerMask playerLayer = LayerMask.GetMask("Player");
        Collider[] hitObjects = Physics.OverlapSphere(attackPos.position, maxRange, playerLayer);

        Debug.Log("Perform enemy Test1");
        foreach (Collider obj in hitObjects)
        {

            PlayerController playerControl = obj.GetComponent<PlayerController>();
            Debug.Log("Perform enemy Test2 : " + obj);
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

                Debug.Log("Perform enemy Test3");

                //enemyRb.AddForce(knockbackDirection.normalized * knockbackForce, ForceMode.Impulse);
            }
        }
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

            vertical = transform.position.x - waypoints[m_currentWayPointIndex].position.x;
            horizontal = transform.position.z - waypoints[m_currentWayPointIndex].position.z;

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

        vertical = transform.position.x - player.x;
        horizontal = transform.position.z - player.z;

        if (Vector3.Distance(transform.position, player) <= 0.3)
        {
            if (m_WaitTime <= 0)
            {
                m_PlayerNear = false;
                Move(speedWalk);
                navMeshAgent.SetDestination(waypoints[m_currentWayPointIndex].position);

                vertical = transform.position.x - waypoints[m_currentWayPointIndex].position.x;
                horizontal = transform.position.z - waypoints[m_currentWayPointIndex].position.z;

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
