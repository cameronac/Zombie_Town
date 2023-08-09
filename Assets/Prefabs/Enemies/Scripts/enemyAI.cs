using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using UnityEngine;
using UnityEngine.AI;
using static UnityEngine.GraphicsBuffer;
using UnityEngine.UIElements;

public class enemyAI : MonoBehaviour, IDamage
{
    enum STATE { roam, chase, death }

    STATE currentState = STATE.roam;

    [SerializeField] Renderer model;
    [SerializeField] float currentHP, maxHP = 10f;
    [SerializeField] float distance = 10f;

    private NavMeshAgent enemyMob;
    RaycastHit hit;

    //collider/trigger
    bool playerInRange;

    public GameObject player;
    public float damage = 1;
    public float enemyDistanceRun = 4f;
    public float faceSpeed = 300f;
    public float waitTime = 3;

    //patrolling enemy
    public float distanceToPlayer;
    public Transform[] wayPoints;
    int wayPointIndex;
    Vector3 target;

    //attacks
    public float timeBetweenAttacks = .5f;
    bool alreadyAttacked = false;
    bool playerInAttackRange = false;

    //-----------------Main Methods-----------------//

    void Start()    //called before first frame update
    {
        //starts enemy at maxHealth;
        currentHP = maxHP;
        
        enemyMob = GetComponent<NavMeshAgent>();
        UpdateDestinations();
    }

    void Update()   //Updates Every Frame
    {
        if (!playerInRange && !playerInAttackRange)
        {
            PatrolTheArea();
        }
        else if (playerInRange && !playerInAttackRange)
        {
            FollowPlayer();
            if (playerInRange && playerInAttackRange)
            {
                AttackPlayer();
            }
        }
        UpdateState();

        //switch (currentState)
        //{
        //    //roam - default state
        //    case STATE.roam:
        //    if(!playerInRange && !playerInAttackRange)
        //    {
        //        PatrolTheArea();
        //    }
        //    break;

        //    //if enemy damaged - chase
        //    case STATE.chase:
        //    if(playerInRange && !playerInAttackRange)
        //    {
        //        FollowPlayer();
        //    }
        //    if(playerInRange && playerInAttackRange)
        //    {
        //        FollowPlayer();
        //        AttackPlayer();
        //    }
        //    break;

        //}
        //UpdateState();
    }
    //---------------------------------


    //States: Main Methods-------------
    void PatrolTheArea()
    {
        //checks to see if distance from target is less than 1m
        if (Vector3.Distance(transform.position, target) < 1)
        {
            //call functions to wander
            IterateWayPointIndex();
            UpdateDestinations();
        }
    }

    void FollowPlayer()
    {
        enemyMob.SetDestination(gameManager.instance.player.transform.position);
    }

    void AttackPlayer()
    {
        if (hit.collider.tag == "Player")
        {
            IDamage iDamage = hit.collider.GetComponent<IDamage>();

            if (iDamage != null && !alreadyAttacked)
            {
                iDamage.TakeDamage(damage);
            }
            else if(alreadyAttacked)
            {
                ResetAttack();
            }
        }
    }

    private void ResetAttack()
    {
        alreadyAttacked = false;
    }

    private void enemyDamaged()
    {
        TakeDamage(damage);
    }

    //private void AttackThePlayer()
    //{
    //    enemyMob.SetDestination(transform.position);

    //    if(!alreadyAttacked)
    //    {
    //        alreadyAttacked = true;
    //        Invoke(nameof(ResetAttack), timeBetweenAttacks);
    //    }
    //}



    //---------------------------------

    //Updates State of AI--------------
    void UpdateState()
    {
        Vector3 direction = (gameManager.instance.player.transform.position - transform.position).normalized;
        Ray ray = new Ray(transform.position, direction);
        
        bool isHit = Physics.Raycast(ray, out hit, distance);
        bool canSeePlayer = false;

        //Check if the player is Seen
        if (isHit)
        {
            if (hit.transform.tag == "Player")
            {
                canSeePlayer = true;
            }
        }

        //Change AI State
        if (canSeePlayer)
        {
            currentState = STATE.chase;
        }
        else if(!canSeePlayer)
        {
            currentState = STATE.roam;
        }

    }
    //---------------------------------

    //Sphere Collider/Trigger-----------
    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = true;
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = false;
        }
    }
    //---------------------------------


    //----Helper Methods-----//

    void UpdateDestinations()
    {
        //get position of current waypoint sets equal to target
        target = wayPoints[wayPointIndex].position;

        //sets navmesh destination to the target
        enemyMob.SetDestination(target);
    }

    void IterateWayPointIndex()
    {
        //increase index by 1
        wayPointIndex++;

        //once last waypoint is reached, it will revert back to first one
        if(wayPointIndex == wayPoints.Length)
        {
            wayPointIndex = 0;
        }
    }
    //---------------------

    //Taking Damage-------------------
    public void TakeDamage(float damage) //enemy takes damage & apparates(for now)
    {
        currentHP -= damage;
        StartCoroutine(flashDamage());

        if (currentHP <= 0)
        {
            Destroy(gameObject);
        }
    }

    
    IEnumerator flashDamage() //to show damage(for now)
    {
        model.material.color = UnityEngine.Color.red;
        yield return new WaitForSeconds(0.1f);
        model.material.color = UnityEngine.Color.white;
    }
    //--------------------------------
}
