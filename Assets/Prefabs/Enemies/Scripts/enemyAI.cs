using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using static UnityEngine.GraphicsBuffer;
using UnityEngine.UIElements;
using Unity.VisualScripting;
using UnityEditor.Rendering;
using Unity.Burst.CompilerServices;

public class enemyAI : MonoBehaviour, IDamage
{
    enum STATE { roam, chase, death }

    STATE currentState = STATE.roam;

    [SerializeField] Renderer model;
    [SerializeField] float currentHP, maxHP = 10f;
    [SerializeField] float patrolSpd;
    [SerializeField] float chaseSpd;
    float distance = 10f;
    float hitRate = 0.5f;

    private NavMeshAgent enemyMob;
    Color startColor = Color.white;

    //collider/trigger
    bool playerInRange;
    bool canAttack = true;

    public GameObject player;
    public float damage = 20;
    bool canSeePlayer = false;
    int playerFaceSpeed = 120;
    Vector3 playerDirection;

    //patrolling enemy
    private Vector3 startPos;
    float pointRange = 10;
    bool isPatrolTimer = false;
    public float distanceToPlayer;
    public Transform[] wayPoints;
    int wayPointIndex;
    Vector3 target;

    //-----------------Main Methods-----------------//

    void Start()    //called before first frame update
    {
        startPos = transform.position;
        startColor = GetComponent<MeshRenderer>().sharedMaterial.color;
        //starts enemy at maxHealth;
        currentHP = maxHP;
        
        enemyMob = GetComponent<NavMeshAgent>();
        UpdateDestinations();
    }

    void Update()   //updates Every Frame
    {
        if(playerInRange)
        {
            FollowPlayer();
            AttackPlayer();
        }
        else if(!playerInRange)
        {
            PatrolTheArea();
        }

        UpdateState();
    }
    //---------------------------------


    //States: Main Methods-------------
    void PatrolTheArea()
    {
        enemyMob.speed = patrolSpd;
        if (!isPatrolTimer)
        {
            StartCoroutine(GetRandomPatrolPoint());
        }
        //checks to see if distance from target is less than 1m
        /*if (Vector3.Distance(transform.position, target) < 1)
        {
            IterateWayPointIndex();
            UpdateDestinations();
        }*/
    }

    void FollowPlayer()
    {
        enemyMob.speed = chaseSpd;

        if (gameManager.instance != null) {
            Quaternion rot = Quaternion.LookRotation(gameManager.instance.player.transform.position - transform.position);
            transform.rotation = Quaternion.Lerp(transform.rotation, rot, Time.deltaTime * playerFaceSpeed);

            enemyMob.SetDestination(gameManager.instance.player.transform.position);
        }
    }

    void AttackPlayer()
    {
        if (canAttack)
        {
            StartCoroutine(attack());
        }
    }

    //---------------------------------

    //Updates State of AI--------------
    void UpdateState()
    {
        RaycastHit hit;
        Vector3 direction = (gameManager.instance.player.transform.position - transform.position).normalized;
        Ray ray = new Ray(transform.position, direction);
        
        bool isHit = Physics.Raycast(ray, out hit, distance);
        

        //check if the player is Seen
        if (isHit)
        {
            if (hit.transform.tag == "Player")
            {
                canSeePlayer = true;
            }
        }

        //change AI State
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
        //target = wayPoints[wayPointIndex].position;
            
        //sets navmesh destination to the target
        enemyMob.SetDestination(target);
    }

    void IterateWayPointIndex()
    {
        if (wayPoints.Length > 0) 
        {
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


    //IEnumerators--------------------
    IEnumerator GetRandomPatrolPoint()
    {
        isPatrolTimer = true;
        yield return new WaitForSeconds(Random.Range(5, 10));
        isPatrolTimer = false;
        float newX = Random.Range(-pointRange, pointRange);
        float newZ = Random.Range(-pointRange, pointRange);

        target = new Vector3(newX + startPos.x, startPos.y, newZ + startPos.z);
        enemyMob.SetDestination(target);
    }

    IEnumerator attack()
    {
        canAttack = false;
        yield return new WaitForSeconds(hitRate);
        canAttack = true;

        if (gameManager.instance != null)
        {
            RaycastHit hit;
            Vector3 playerPos = gameManager.instance.player.transform.position;
            Vector3 direction = (playerPos - transform.position).normalized;
            //Use For hit detection once fixed -> //bool isHit = Physics.BoxCast(transform.position, new Vector3(0.25f, 0.25f, 0.25f), (playerPos - transform.position).normalized, out hit);
            bool isHit = Physics.Raycast(new Ray(transform.position, direction), out hit, 2f);

            //if collider hit player - attack
            if (isHit)
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
        }
    }

    IEnumerator flashDamage() //to show damage(for now)
    {
        model.material.color = UnityEngine.Color.red;
        yield return new WaitForSeconds(0.1f);
        model.material.color = startColor;
    }
    //--------------------------------
}
