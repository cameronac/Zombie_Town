using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.ProBuilder;

public class enemyAI : MonoBehaviour, IDamage
{
    private enum STATE
    { roam, chase, death }

    private STATE currentState = STATE.roam;
    
    private Color startColor = Color.white;

    [Header("----- Components -----")]
    [SerializeField] private Renderer model;
    [SerializeField] NavMeshAgent enemyMob;
    [SerializeField] Animator anim;
    [SerializeField] SphereCollider triggerSphere;
    private bool isPlayerSeen = false;

    [Header("----- Enemy Stats -----")]
    [Range(1, 20)][SerializeField] private float currentHP;
    [Range(1, 20)][SerializeField] private float maxHP;
    [Range(1, 20)][SerializeField] public float damage;
    [Range(1, 10)][SerializeField] private float patrolSpeed;
    [Range(1, 10)][SerializeField] private float chaseSpeed;
    [SerializeField] int animChangeSpeed;

    private float hitDistance = 6f;
    private float distance = 10f;
    private float hitRate = 0.5f;
    private float playerFaceSpeed = 2f;
    
    //sphere collider/trigger - detection & attacking
    private bool playerInRange;
    private bool canAttack = true;

    //enemy spawner
    public enemySpawner whereISpawned;

    //patrolling enemy
    private float patrolDist = 10f;
    private float visionArea = 0.5f;
    private Vector3 startPos;
    private bool isPatrolTimer = false;
    public float distanceToPlayer;
    public Transform[] wayPoints;
    private int wayPointIndex;
    private Vector3 target;

    //-----------------Main Methods-----------------//

    private void Start()    //called before first frame update
    {
        if (anim.GetComponentInChildren<Animator>().CompareTag("Alerted"))
        {
            enemyMob.enabled = false;
        }

        startPos = transform.position;
        startColor = GetComponent<MeshRenderer>().sharedMaterial.color;

        //starts enemy at maxHealth;
        currentHP = maxHP;

        enemyMob = GetComponent<NavMeshAgent>();
        anim = GetComponentInChildren<Animator>();
    }

    private void Update()
    {
        //Animation Speed
        float agentVel = 0;
        if (enemyMob.velocity.magnitude > 0)
        {
            agentVel = enemyMob.velocity.magnitude / chaseSpeed;
        }

        anim.SetFloat("speed", Mathf.Lerp(anim.GetFloat("speed"), agentVel, Time.deltaTime * animChangeSpeed));

        /*if (enemyMob.isActiveAndEnabled && enemyMob.CompareTag("Alerted"))
        {
            enemyMob.enabled = true;
        }*/

        UpdateVision();

        if (currentState != STATE.death)
        {
            if (isPlayerSeen)
            {
                enemyMob.stoppingDistance = 2.25f;
                ChasePlayer();
                AttackPlayer();
            }
            else if (!isPlayerSeen)
            {
                enemyMob.stoppingDistance = 0.0f;
                PatrolTheArea();
            }

            UpdateState();
        }
    }
    //---------------------------------

    //States: Main Methods-------------
    private void PatrolTheArea()
    {
        enemyMob.speed = patrolSpeed;

        if (!isPatrolTimer)
        {
            StartCoroutine(GetRandomPatrolPoint());
        }
    }

    private void ChasePlayer()
    {
        enemyMob.speed = chaseSpeed;

        if (gameManager.instance != null)
        {
            enemyMob.SetDestination(gameManager.instance.player.transform.position);
        }
    }

    private void AttackPlayer()
    {
        bool inHitDistance = Vector3.Distance(gameManager.instance.player.transform.position, transform.position) <= hitDistance;
        
        if (inHitDistance) 
        {
            FacePlayer();
        }

        if (canAttack && inHitDistance)
        {
            StartCoroutine(attack());
        }
    }
    //---------------------------------

    //Helper Methods-----------------------------
    private void FacePlayer()
    {
        Quaternion rot = Quaternion.LookRotation(gameManager.instance.player.transform.position - transform.position);
        transform.rotation = Quaternion.Lerp(transform.rotation, rot, Time.deltaTime * playerFaceSpeed);
    }

    private void UpdateVision()
    {
        GameObject player = gameManager.instance.player;
        Vector3 playerDirection = (player.transform.position - transform.position).normalized;

        //Raycast to the object
        RaycastHit hit;
        Ray ray = new Ray(transform.position, playerDirection);

        bool isHit = Physics.Raycast(ray, out hit, triggerSphere.radius);
        float angleToPlayer = Vector3.Dot(playerDirection, transform.forward);

        //Object in Sphere
        if (isHit && hit.collider.tag == "Player" && angleToPlayer > 0)
        {
            isPlayerSeen = true;
        }
    }
    //-------------------------------------------

    //Updates State of AI--------------
    private void UpdateState()
    {
        //change AI State
        if (isPlayerSeen)
        {
            currentState = STATE.chase;
        }
        else if (!isPlayerSeen)
        {
            currentState = STATE.roam;
        }
    }
    //---------------------------------

    //Taking Damage-------------------
    public void TakeDamage(float damage) //enemy takes damage & apparates(for now)
    {
        currentHP -= damage;
        
        StartCoroutine(flashDamage());

        if (currentHP <= 0)
        {
            StopAllCoroutines();
            GetComponent<CapsuleCollider>().enabled = false;
            StartCoroutine(isDead());
        } else {
            enemyMob.SetDestination(gameManager.instance.player.transform.position);
        }
    }
    //---------------------------------

    //Sphere Collider/Trigger-----------
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = false;
        }
    }
    //---------------------------------


    //IEnumerators--------------------
    private IEnumerator GetRandomPatrolPoint()
    {
        isPatrolTimer = true;
        yield return new WaitForSeconds(Random.Range(3, 8));
        isPatrolTimer = false;

        Vector3 randomPosition = (Random.insideUnitSphere * patrolDist) + startPos;
        NavMeshHit hit;
        NavMesh.SamplePosition(randomPosition, out hit, patrolDist, 1);
        enemyMob.SetDestination(hit.position);

        //target = new Vector3(newX + startPos.x, startPos.y, newZ + startPos.z);
        
        //enemyMob.SetDestination(target);
    }

    private IEnumerator attack()
    {
        canAttack = false;
        yield return new WaitForSeconds(hitRate);
        canAttack = true;

        if (gameManager.instance != null)
        {
            anim.SetTrigger("attackPlayer");
        }
    }

    public void Attack()    //The Attack Animation Calls This Method At A Specific Frame
    {
        RaycastHit hit;
        Vector3 playerPos = gameManager.instance.player.transform.position;
        Vector3 direction = (playerPos - transform.position).normalized;
        bool isHit = Physics.Raycast(new Ray(transform.position, direction), out hit, 2.5f);

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

    private IEnumerator flashDamage() //to show damage(for now)
    {
        model.material.color = UnityEngine.Color.red;
        yield return new WaitForSeconds(0.1f);
        model.material.color = startColor;
    }

    private IEnumerator isDead()
    {
        anim.SetTrigger("enemyDeath");
        currentState = STATE.death;
        enemyMob.enabled = false;

        if (whereISpawned != null) 
        {
            whereISpawned.enemiesDead();
        }

        Destroy(GetComponent<CapsuleCollider>());
        Destroy(GetComponent<CapsuleCollider>());

        yield return new WaitForSeconds(10f);
        Destroy(gameObject);
    }
    //--------------------------------


    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawLine(transform.position, transform.forward * 2 + transform.position);
    }

}