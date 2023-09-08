using System.Collections;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Rendering.PostProcessing;

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
    private bool canSeePlayer = false;
    private int playerFaceSpeed = 120;
    private Vector3 playerDirection;

    [Header("----- Enemy Stats -----")]
    [Range(1, 20)][SerializeField] private float currentHP;
    [Range(1, 20)][SerializeField] private float maxHP;
    [Range(1, 20)][SerializeField] public float damage;
    [Range(1, 10)][SerializeField] private float patrolSpeed;
    [Range(1, 10)][SerializeField] private float chaseSpeed;
    [SerializeField] int animChangeSpeed;
    private float hitDistance = 2.5f;
    private float distance = 10f;
    private float hitRate = 0.5f;
    
    //sphere collider/trigger - detection & attacking
    private bool playerInRange;
    private bool canAttack = true;

    //enemy spawner
    public enemySpawner whereISpawned;

    //patrolling enemy
    private Vector3 startPos;
    private float pointRange = 10;
    private bool isPatrolTimer = false;
    public float distanceToPlayer;
    public Transform[] wayPoints;
    private int wayPointIndex;
    private Vector3 target;

    //-----------------Main Methods-----------------//

    private void Start()    //called before first frame update
    {
        startPos = transform.position;
        //startColor = GetComponent<MeshRenderer>().sharedMaterial.color;

        //starts enemy at maxHealth;
        currentHP = maxHP;

        enemyMob = GetComponent<NavMeshAgent>();
        anim = GetComponentInChildren<Animator>();

        if (anim.GetComponentInChildren<Animator>().CompareTag("Alerted"))
        {
            enemyMob.enabled = false;
        }
    }

    private void Update()   //updates Every Frame
    {
        float agentVel = 0;
        if (enemyMob.velocity.magnitude > 0)
        {
            agentVel = enemyMob.velocity.magnitude / chaseSpeed;
        }

        anim.SetFloat("speed", Mathf.Lerp(anim.GetFloat("speed"), agentVel, Time.deltaTime * animChangeSpeed));

        if (enemyMob.isActiveAndEnabled && anim.GetComponentInChildren<Animator>().CompareTag("Alerted"))
        {
            enemyMob.enabled = true;
        }

        if (currentState != STATE.death)
        {
            if (playerInRange)
            {
                ChasePlayer();
                AttackPlayer();
            }
            else if (!playerInRange)
            {
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
            //face player
            Quaternion rot = Quaternion.LookRotation(gameManager.instance.player.transform.position - transform.position);
            transform.rotation = Quaternion.Lerp(transform.rotation, rot, Time.deltaTime * playerFaceSpeed);
           
            enemyMob.SetDestination(gameManager.instance.player.transform.position);
        }
    }

    private void AttackPlayer()
    {
        if (canAttack && Vector3.Distance(gameManager.instance.player.transform.position, transform.position) <= hitDistance)
        {
            StartCoroutine(attack());
        }
    }
    //---------------------------------

    //Updates State of AI--------------
    private void UpdateState()
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
        else if (!canSeePlayer)
        {
            currentState = STATE.roam;
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

        //on trigger enter, animation will play normally?
        if (anim.GetComponentInChildren<Animator>().CompareTag("Alerted"))
        {
            anim.SetTrigger("isActivated");
            enemyMob.enabled = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = false;
            enemyMob.stoppingDistance = 0;
        }

        if (anim.GetComponentInChildren<Animator>().CompareTag("Alerted"))
        {
            enemyMob.enabled = false;
        }
    }
    //---------------------------------

    //Taking Damage-------------------
    public void TakeDamage(float damage) //enemy takes damage & apparates(for now)
    {
        currentHP -= damage;
        enemyMob.SetDestination(gameManager.instance.player.transform.position);
        StartCoroutine(flashDamage());

        if (currentHP <= 0)
        {
            StopAllCoroutines();
            GetComponent<CapsuleCollider>().enabled = false;
            StartCoroutine(isDead());
        }
    }
    //---------------------------------

    //IEnumerators--------------------
    private IEnumerator GetRandomPatrolPoint()
    {
        isPatrolTimer = true;
        yield return new WaitForSeconds(Random.Range(3, 8));
        isPatrolTimer = false;

        float newX = Random.Range(-pointRange, pointRange);
        float newZ = Random.Range(-pointRange, pointRange);

        target = new Vector3(newX + startPos.x, startPos.y, newZ + startPos.z);
        
        enemyMob.SetDestination(target);
    }

    private IEnumerator attack()
    {
        canAttack = false;
        yield return new WaitForSeconds(hitRate);
        canAttack = true;

        if (gameManager.instance != null)
        {
            RaycastHit hit;
            Vector3 playerPos = gameManager.instance.player.transform.position;
            Vector3 direction = (playerPos - transform.position).normalized;
            bool isHit = Physics.Raycast(new Ray(transform.position, direction), out hit, 2f);

            anim.SetTrigger("attackPlayer");

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

        yield return new WaitForSeconds(10f);
        Destroy(gameObject);
    }
    //--------------------------------
}