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
    [SerializeField] GameObject head;
    [SerializeField] GameObject attackEmpty;
    [SerializeField] Renderer model;
    [SerializeField] NavMeshAgent enemyMob;
    [SerializeField] Animator anim;
    [SerializeField] SphereCollider triggerSphere;
    private bool isPlayerSeen = false;

    [Header("----- Enemy Stats -----")]
    [Range(1, 20)][SerializeField] private float currentHP = 15;
    [Range(1, 20)][SerializeField] private float maxHP = 15;
    [Range(1, 20)][SerializeField] public float damage;
    [Range(1, 10)][SerializeField] private float patrolSpeed;
    [Range(1, 10)][SerializeField] private float chaseSpeed;
    [SerializeField] int animChangeSpeed;

    private float hitDistance = 3f;
    private float hitRate = 0.5f;
    private float playerFaceSpeed = 3f;
    
    //sphere collider/trigger - detection & attacking
    private bool playerInRange;
    private bool canAttack = true;

    //enemy spawner
    public enemySpawner whereISpawned;

    //patrolling enemy
    private float patrolDist = 10f;
    private Vector3 startPos;
    private bool isPatrolTimer = false;
    public float distanceToPlayer;
    public Transform[] wayPoints;
    private int wayPointIndex;
    private Vector3 target;

    //-----------------Main Methods-----------------//

    private void Start() {  //called before first frame update
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

        UpdateVision();
        UpdateEars();

        if (currentState != STATE.death)
        {
            if (isPlayerSeen)
            {
                enemyMob.stoppingDistance = 2.25f;
                ChasePlayer();
            }
            else if (!isPlayerSeen)
            {
                enemyMob.stoppingDistance = 0.0f;
                PatrolTheArea();
            }

            UpdateState();
        }
    }
    //-------------------------------------------

    //Actions------------------------------------
    private void PatrolTheArea()
    {
        enemyMob.speed = patrolSpeed;

        if (!isPatrolTimer)
        {
            StartCoroutine(GetRandomPatrolPoint());
        }
    }

    private void ChasePlayer() {
        //Chase
        enemyMob.speed = chaseSpeed;

        if (gameManager.instance != null)
        {
            enemyMob.SetDestination(gameManager.instance.player.transform.position);
        }

        //Attack
        bool inHitDistance = Vector3.Distance(gameManager.instance.player.transform.position, transform.position) <= hitDistance;

        if (inHitDistance)
        {
            FacePlayer();
        }

        if (canAttack && inHitDistance)
        {
            if (canAttack) {
                canAttack = false;
                anim.SetTrigger("attackPlayer");
            }
        }
    }

    public void Attack() {   //The Attack Animation Calls This Method At A Specific Frame
        Collider[] colliders = Physics.OverlapSphere(attackEmpty.transform.position, 1f);

        for (int i = 0; i < colliders.Length; i++)
        {
            if (colliders[i].tag == "Player")
            {
                IDamage iDamage = colliders[i].gameObject.GetComponent<IDamage>();

                if (iDamage != null)
                {
                    iDamage.TakeDamage(damage);
                }

                break;
            }
        }

        canAttack = true;
    }
    //-------------------------------------------

    //Helper Methods-----------------------------
    private void FacePlayer()
    {
        Quaternion rot = Quaternion.LookRotation(gameManager.instance.player.transform.position - transform.position);
        transform.rotation = Quaternion.Lerp(transform.rotation, rot, Time.deltaTime * playerFaceSpeed);
    }

    private void UpdateVision()
    {
        if (playerInRange)
        {
            GameObject player = gameManager.instance.player;
            Vector3 playerDirection = (player.transform.position - head.transform.position).normalized;

            //Raycast to the object
            RaycastHit hit;
            Ray ray = new Ray(head.transform.position, playerDirection);

            bool isHit = Physics.Raycast(ray, out hit, triggerSphere.radius);
            float angleToPlayer = Vector3.Dot(playerDirection, transform.forward);

            //Object in Sphere
            if (isHit && hit.collider.tag == "Player" && angleToPlayer > 0)
            {
                isPlayerSeen = true;
            }
        }
    }
    
    private void UpdateEars()
    {
        if (playerInRange && gameManager.instance.p_playerShoot.IsGunShot())
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

    //Sphere Trigger-----------
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
}