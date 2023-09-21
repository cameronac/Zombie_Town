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

    [Header("----- Audio -----")]
    [SerializeField] AudioClip[] damaged_audio;
    [SerializeField] AudioClip[] growl_audio;
    [SerializeField] AudioClip[] walk_audio;
    [SerializeField] AudioClip[] death_audio;
    [SerializeField] AudioClip[] random_audio;
    
    [Header("----- Components -----")]
    [SerializeField] GameObject head_mesh;
    [SerializeField] GameObject headCollider;
    [SerializeField] GameObject head;
    [SerializeField] GameObject attackEmpty;
    [SerializeField] NavMeshAgent enemyMob;
    [SerializeField] Animator anim;
    [SerializeField] SphereCollider triggerSphere;
    

    [Header("----- Enemy Stats -----")]
    [Range(1, 20)][SerializeField] public float currentHP = 15;
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

    //patrolling enemy
    private float patrolDist = 10f;
    private Vector3 startPos;
    private bool isPatrolTimer = false;
    public float distanceToPlayer;

    //-----------------Main Methods-----------------//

    private void Start() {  //called before first frame update
        startPos = transform.position;

        enemyMob = GetComponent<NavMeshAgent>();
        anim = GetComponentInChildren<Animator>();
    }

    private void Update()
    {
        //Animation Speed
        float agentVel = 0;
        if (enemyMob.velocity.magnitude > 0)
        {
            agentVel = enemyMob.velocity.magnitude / 2.25f;
        }

        anim.SetFloat("speed", Mathf.Lerp(anim.GetFloat("speed"), agentVel, Time.deltaTime * animChangeSpeed));

        //Triggers
        if (!isDead) {
            if (currentState != STATE.chase) {
                UpdateEars();
            }

            UpdateVision();
        }

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

        if (!isPatrolTimer && Vector3.Distance(transform.position, enemyMob.destination) <= 1.0f)
        {
            StartCoroutine(GetRandomPatrolPoint());
        }
    }
    private IEnumerator GetRandomPatrolPoint()
    {
        isPatrolTimer = true;
        yield return new WaitForSeconds(Random.Range(1, 3));
        isPatrolTimer = false;

        if (!wasAttacked && !isDead) {
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

            if (!isPlayerSeen)
            {
                AudioManager.instance.CreateSoundWithParent(growl_audio[Random.Range(0, growl_audio.Length)], transform.position, transform, 1, "zombie");
            }

            isPlayerSeen = true;

        } else if (!playerInRange) {
            isPlayerSeen = false;
        }
    }

    private void UpdateEars()
    {
        if (playerInRange && gameManager.instance.p_playerShoot.IsGunShot())
        {
            if (!isPlayerSeen)
            {
                AudioManager.instance.CreateSoundAtPosition(growl_audio[Random.Range(0, growl_audio.Length)], transform.position, 1, "zombie");
            }

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

    //Events---------------------------
    public void Stepped()
    {
        AudioManager.instance.CreateSoundAtPosition(walk_audio[Random.Range(0, walk_audio.Length)], transform.position, 0.1f, "zombie");
    }
    public void TakeDamage(float damage) //enemy takes damage & apparates(for now)
    {
        if (!isDead) {
            currentHP -= damage;

            if (currentHP > 0) {
                AudioManager.instance.CreateSoundAtPosition(damaged_audio[Random.Range(0, damaged_audio.Length)], transform.position, .75f, "zombie");
                if (playerInRange)
                {
                    isPlayerSeen = true;
                } else if (currentState != STATE.chase) {
                    enemyMob.SetDestination(gameManager.instance.player.transform.position);
                    wasAttacked = true;
                } 
          
            } else {
                isDead = true;
                StartCoroutine(Death());
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
    private IEnumerator Death()
    {
        AudioManager.instance.CreateSoundAtPosition(death_audio[Random.Range(0, death_audio.Length)], head_mesh.transform.position, 1, "zombie");
        anim.SetTrigger("enemyDeath");
        enemyMob.enabled = false;

        Destroy(headCollider);
        Destroy(GetComponent<CapsuleCollider>());
        Destroy(GetComponent<CapsuleCollider>());

        yield return new WaitForSeconds(10f);
        Destroy(gameObject);
    }
    //--------------------------------
}