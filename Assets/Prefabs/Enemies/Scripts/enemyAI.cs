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
    //public Transform player;

    public float HP = 5;
    public float damage = 1;
    public float distance;

    Vector3 playerDirection;
    bool playerDetected = false;
    

    //Start is called before the first frame update
    void Start()
    {
        enemyMob = GetComponent<NavMeshAgent>();
    }

    //Update is called once per frame
    void Update()
    {
        distance = Vector3.Distance(transform.position, player.transform.position);

        followPlayer();
    }

    //wandering around       
    void stagnantEnemy()
    {
        //wandering around code
        //NavMeshAgent agent = GetComponent<NavMeshAgent>();
        //agent.destination = endLocation.position;
    }

    //facing/following the player
    void followPlayer()
    {
        if(distance < enemyDistanceRun)
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

    //to show damage(temp)
    IEnumerator flashDamage()
    {
        model.material.color = Color.red;
        yield return new WaitForSeconds(0.1f);
        model.material.color = Color.white;
    }
}