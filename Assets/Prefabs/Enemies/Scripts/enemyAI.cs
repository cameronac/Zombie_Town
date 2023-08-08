using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class enemyAI : MonoBehaviour, IDamage
{
    enum STATE { roam, chase }

    STATE currentState = STATE.roam;

    [SerializeField] Renderer model;
    private NavMeshAgent enemyMob;
    public GameObject player;
    public Collider collision;

    public float enemyDistanceRun = 4f;
    public float faceSpeed = 120f;

    [SerializeField] float HP, maxHealth = 20f;
    [SerializeField] float distance = 25f;
    public float damage = 1;
    public float distanceToPlayer;

    //patrolling enemy
    Vector3 target;
    int wayPointIndex;
    public Transform[] wayPoints;


    //Main Methods---------------------
    void Start()    //called before first frame update
    {
        //starts enemy at maxHealth;
        HP = maxHealth;
        
        enemyMob = GetComponent<NavMeshAgent>();
        UpdateDestinations();
    }

    void Update()   //Updates Every Frame
    {
        //need to have both patrol and chase player 
        switch(currentState)
        {
            case STATE.chase:
                FollowPlayer();
                break;

            case STATE.roam:
                PatrolTheArea();
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
        //distanceToPlayer = Vector3.Distance(transform.position, player.transform.position);

        //Vector3 directionToPlayer = transform.position - player.transform.position;
        //Vector3 newPosition = transform.position - directionToPlayer;

        enemyMob.SetDestination(gameManager.instance.player.transform.position);
    }

    //---------------------------------

    //Updates State of AI--------------
    void UpdateState()
    {
        RaycastHit hit;
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
        } else {
            currentState = STATE.roam;
        }

    }
    //---------------------------------


    //Helper Methods

    //wandering around methods
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

    //void facePlayer()
    //{
    //    Quaternion rot = Quaternion.LookRotation(playerDirection);
    //    transform.rotation = Quaternion.Lerp(transform.rotation, rot, Time.deltaTime * faceSpeed);
    //}

    //facing/following the player

    //attacking the player
    void playerAttack()
    {
       //not yet implemented
    }

    //enemy takes damage & apparates(for now)
    public void TakeDamage(float damage)
    {
        HP -= damage;
        StartCoroutine(flashDamage());

        if (HP <= 0)
        {
            Destroy(gameObject);
        }
    }

    //to show damage(for now)
    IEnumerator flashDamage()
    {
        model.material.color = Color.red;
        yield return new WaitForSeconds(0.1f);
        model.material.color = Color.white;
    }
}
