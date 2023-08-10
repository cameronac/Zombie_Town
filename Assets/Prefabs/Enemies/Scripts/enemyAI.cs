using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using UnityEngine;
using UnityEngine.AI;
using static UnityEngine.GraphicsBuffer;
using UnityEngine.UIElements;
using Unity.VisualScripting;

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
    float enemyDistanceRun = 4f;
    float faceSpeed = 300f;
    float waitTime = 3;
    bool canSeePlayer = false;

    //patrolling enemy
    public float distanceToPlayer;
    public Transform[] wayPoints;
    int wayPointIndex;
    Vector3 target;

    //attacks
    float timeBetweenAttacks = 1.5f;
    bool attacking;
    bool playerInAttackRange;

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
        //PatrolTheArea();
        //if (!playerInRange && !playerInAttackRange)
        //{
        //    enemyMob.isStopped = false;
        //    //needs to return back to patrolling
        //}
        //else if (playerInRange && !playerInAttackRange)
        //{
        //    FollowPlayer();

        //    if (playerInRange && playerInAttackRange)
        //    {
        //        AttackPlayer();
        //    }
        //}
        //UpdateState();

        switch (currentState)
        {
            //roam - default state
            case STATE.roam:
                PatrolTheArea();
                if (!playerInRange)
                {
                    IterateWayPointIndex();
                }
                break;

            //if enemy damaged - chase
            case STATE.chase:
                if (playerInRange)
                {
                    FollowPlayer();
                    AttackPlayer();
                }
                break;
        }
        UpdateState();
    }
    //---------------------------------


    //States: Main Methods-------------
    void PatrolTheArea()
    {
        //checks to see if distance from target is less than 1m
        if (Vector3.Distance(transform.position, target) < 1)
        {
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

            if (iDamage != null)
            {
                iDamage.TakeDamage(damage);
            }
        }
    }

    //---------------------------------

    //Updates State of AI--------------
    void UpdateState()
    {
        Vector3 direction = (gameManager.instance.player.transform.position - transform.position).normalized;
        Ray ray = new Ray(transform.position, direction);
        
        bool isHit = Physics.Raycast(ray, out hit, distance);
        

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
        if (wayPoints.Length > 0) {

            if (wayPoints[wayPointIndex] != null) {
                //get position of current waypoint sets equal to target
                target = wayPoints[wayPointIndex].position;
            
                //sets navmesh destination to the target
                enemyMob.SetDestination(target);
            }
        }
    }

    void IterateWayPointIndex()
    {
        if (wayPoints.Length > 0) {
            //increase index by 1
            wayPointIndex++;

            //once last waypoint is reached, it will revert back to first one
            if(wayPointIndex == wayPoints.Length)
            {
                wayPointIndex = 0;
            }
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
