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
    [SerializeField] float HP, maxHealth = 10f;
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

    //-----------------Main Methods-----------------//

    void Start()    //called before first frame update
    {
        //starts enemy at maxHealth;
        HP = maxHealth;
        
        enemyMob = GetComponent<NavMeshAgent>();
        UpdateDestinations();
    }

    void Update()   //Updates Every Frame
    {
            switch (currentState)
            {
                //roam - default state
                case STATE.roam:
                    PatrolTheArea();
                    break;

                //if enemy damaged - chase
                case STATE.chase:
                    if (playerInRange) //possibly needs to be changed
                    {
                        FollowPlayer();
                        //AttackPlayer(); -- when placed here player health depletes when enemy looks/collision happens
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
            //AttackPlayer(); -- when placed here player's health depletes on game load
        }
        else if(!canSeePlayer)
        {
            WaitingPeriod(waitTime); //wait 3 seconds(roughly)

            //possibly something here to reset the enemy to start patrolling from last stored point on the path

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

    IEnumerator WaitingPeriod(float waitTime)
    {
        yield return new WaitForSeconds(waitTime);
    }

    //Taking Damage-------------------
    public void TakeDamage(float damage) //enemy takes damage & apparates(for now)
    {
        HP -= damage;
        StartCoroutine(flashDamage());

        if (HP <= 0)
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
