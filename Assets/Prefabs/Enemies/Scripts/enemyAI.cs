using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class enemyAI : MonoBehaviour, IDamage
{
    [SerializeField] Renderer model;
    private NavMeshAgent enemyMob;
    public GameObject player;

    public float enemyDistanceRun = 4f;
    public float faceSpeed = 120f;

    Transform endLocation;

    [SerializeField] float HP, maxHealth = 20f;
    public float damage = 1;
    public float distanceToPlayer;

    //patrolling enemy
    Vector3 target;
    int wayPointIndex;
    public Transform[] wayPoints;


    //Start is called before the first frame update
    void Start()
    {
        //starts enemy at maxHealth;
        HP = maxHealth;

        enemyMob = GetComponent<NavMeshAgent>();

        UpdateDestinations();
    }

    //Update is called once per frame
    void Update()
    {
        //checks to see if distance from target is less than 1m
        if(Vector3.Distance(transform.position, target) < 1)
        {
            //call functions to wander
            IterateWayPointIndex();
            UpdateDestinations();
        }

        //chase player
        distanceToPlayer = Vector3.Distance(transform.position, player.transform.position);
        
        followPlayer();
    }

    //wandering around methods
    void UpdateDestinations()
    {
        //sets target to current waypoint
        target = wayPoints[wayPointIndex].position;

        //sets navmesh destination to the target
        enemyMob.SetDestination(target);
    }

    void IterateWayPointIndex()
    {
        //increase index by 1
        wayPointIndex++;

        //if wayPoint is equal to # of waypoints in map, it sets equal to 0
        if(wayPointIndex == wayPoints.Length)
        {
            wayPointIndex = 0;
        }
    }

    void PauseEnemy()
    {
        enemyMob.isStopped = true;
        enemyMob.speed = 0;
    }

    //facing/following the player
    void followPlayer()
    {
        if (distanceToPlayer < enemyDistanceRun)
        {
            Vector3 directionToPlayer = transform.position - player.transform.position;
            Vector3 newPosition = transform.position - directionToPlayer;

            enemyMob.SetDestination(newPosition);
        }
    }

    //attacking the player
    void playerAttack()
    {
        //not yet implemented
    }

    //enemy takes damage & apparates(for now)
    public void TakeDamage(float damage)
    {
        HP -= damage;

        if (HP <= 0)
        {
            flashDamage();
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
