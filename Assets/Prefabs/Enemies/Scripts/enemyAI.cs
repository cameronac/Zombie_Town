using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.ProBuilder;

public class enemyAI : MonoBehaviour, IDamage
{
    private enum STATE
    { roam, chase, attacked, death }

    private STATE currentState = STATE.roam;
    
    private Color startColor = Color.white;

    [Header("----- Components -----")]
    [SerializeField] GameObject head;
    [SerializeField] GameObject attackEmpty;
    [SerializeField] Renderer model;
    [SerializeField] NavMeshAgent enemyMob;
    [SerializeField] Animator anim;
    [SerializeField] SphereCollider triggerSphere;
    

    [Header("----- Enemy Stats -----")]
    [Range(1, 20)][SerializeField] private float currentHP = 15;
    [Range(1, 20)][SerializeField] private float maxHP = 15;
    [Range(1, 20)][SerializeField] public float damage;
    [Range(1, 10)][SerializeField] private float patrolSpeed;
    [Range(1, 10)][SerializeField] private float chaseSpeed;
    [SerializeField] int animChangeSpeed;

    //State Conditions
    private bool isPlayerSeen = false;
    private bool playerInRange = false;
    private bool isDead = false;
    

    private float hitDistance = 3f;
    private float playerFaceSpeed = 3f;
    
    //sphere collider/trigger - detection & attacking
    private bool canAttack = true;
    private bool wasAttacked = false;
    private float visualDistance = 20f;

    //enemy spawner
    public enemySpawner whereISpawned;

    //patrolling enemy
    private float patrolDist = 10f;
    private Vector3 startPos;
    private bool isPatrolTimer = false;
    private bool isAttackedTimer = false;
    public float distanceToPlayer;
    public Transform[] wayPoints;

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

        //Triggers
        if (currentState != STATE.chase) {
            UpdateEars();
        }

        UpdateVision();
        UpdateState();

        //State Actions
        switch(currentState)
        {
            case STATE.roam:
                enemyMob.stoppingDistance = 0.0f;
                PatrolTheArea();
                break;

            case STATE.chase:
                enemyMob.speed = chaseSpeed;
                enemyMob.stoppingDistance = 2.25f;
                ChasePlayer();
                break;

            case STATE.attacked:
                enemyMob.speed = chaseSpeed;
                enemyMob.stoppingDistance = 2.25f;
                ChaseToPoint();
                break;

            case STATE.death:
                break;
        }
    }
    //-------------------------------------------



    //States-------------------------------------

    //Patrol-------------------------------------
    private void PatrolTheArea()
    {
        enemyMob.speed = patrolSpeed;

        if (!isPatrolTimer)
        {
            StartCoroutine(GetRandomPatrolPoint());
        }
    }
    private IEnumerator GetRandomPatrolPoint()
    {
        isPatrolTimer = true;
        yield return new WaitForSeconds(Random.Range(3, 8));
        isPatrolTimer = false;

        if (!wasAttacked) {
            Vector3 randomPosition = (Random.insideUnitSphere * patrolDist) + startPos;
            NavMeshHit hit;
            NavMesh.SamplePosition(randomPosition, out hit, patrolDist, 1);
            enemyMob.SetDestination(hit.position);
        }
    }
    //-------------------------------------------

    //Chase--------------------------------------
    private void ChasePlayer() {

        enemyMob.SetDestination(gameManager.instance.player.transform.position);
        bool inHitDistance = Vector3.Distance(gameManager.instance.player.transform.position, transform.position) <= hitDistance;

        if (inHitDistance) { FacePlayer(); }

        if (canAttack && inHitDistance)
        {
            canAttack = false;
            anim.SetTrigger("attackPlayer");
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

    //Attacked-----------------------------------
    private void ChaseToPoint()
    {
        if (Vector3.Distance(transform.position, enemyMob.destination) <= 2f)
        {
            wasAttacked = false;
        }
    }
    //-------------------------------------------
    //-------------------------------------------


    //Helper Methods-----------------------------
    private void FacePlayer()
    {
        Quaternion rot = Quaternion.LookRotation(gameManager.instance.player.transform.position - transform.position);
        transform.rotation = Quaternion.Lerp(transform.rotation, rot, Time.deltaTime * playerFaceSpeed);
    }

    private void UpdateVision()
    {
        GameObject player = gameManager.instance.player;
        Vector3 playerDirection = (player.transform.position - head.transform.position).normalized;
        
        //Raycast to the object
        RaycastHit hit;
        Ray ray = new Ray(head.transform.position, playerDirection);
        
        bool isHit = Physics.Raycast(ray, out hit, visualDistance);
        float angleToPlayer = Vector3.Dot(playerDirection, transform.forward);
        
        if (isHit && hit.collider.tag == "Player" && angleToPlayer > 0) {
            isPlayerSeen = true;

        } else if (!playerInRange) {
            isPlayerSeen = false;
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
        if (isDead) {
            currentState = STATE.death;

        } else if (isPlayerSeen) {
            currentState = STATE.chase;

        } else if (wasAttacked) {
            currentState = STATE.attacked;

        } else {
            currentState = STATE.roam;
        }
    }
    //---------------------------------

    //Taking Damage-------------------
    public void TakeDamage(float damage) //enemy takes damage & apparates(for now)
    {
        currentHP -= damage;
        StartCoroutine(flashDamage());

        if (currentHP <= 0) {
            StopAllCoroutines();
            GetComponent<CapsuleCollider>().enabled = false;
            StartCoroutine(Death());

        } else {
            if (currentState != STATE.chase) {
                enemyMob.SetDestination(gameManager.instance.player.transform.position);
                wasAttacked = true;
            }
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
    private IEnumerator flashDamage() //to show damage(for now)
    {
        model.material.color = UnityEngine.Color.red;
        yield return new WaitForSeconds(0.1f);
        model.material.color = startColor;
    }

    private IEnumerator Death()
    {
        isDead = true;
        anim.SetTrigger("enemyDeath");
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