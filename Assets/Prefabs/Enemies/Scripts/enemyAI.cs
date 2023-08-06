using System;
using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using UnityEngine;
using UnityEngine.AI;
using static enemyAI;
using static UnityEditor.Experimental.GraphView.GraphView;

public class enemyAI : MonoBehaviour, IDamage
{
    [SerializeField] Renderer model;
    [SerializeField] NavMeshAgent agent;

    [SerializeField] float HP;
    [SerializeField] int enemySpeed;
    [SerializeField] int faceSpeed;

    public Transform playerPosition;
    public float playerDistance;

    private Transform player;
    bool playerDetected;
    UnityEngine.Vector3 playerDirection;

    public int totalEnemies = 20;
    public bool enemySpawned = true;



    //Start is called before the first frame update
    void Start()
    {
        //agent = GetComponent<UnityEngine.AI.NavMeshAgent>();

        playerDetected = false;
        player = GameObject.FindWithTag("Player").transform;

        agent = GetComponent<NavMeshAgent>();
        if (agent == null)
        {
            Destroy(gameObject);
        }
            
    }

    //Update is called once per frame
    void Update()
    {
        //collision detected - missing detection area 
        if (playerDetected)
        {

            //call function to face the player
            agent.isStopped = true;
            facingPlayer();

            //call function to chase player
            huntingPlayer();
        }
        else if (!playerDetected)
        {
            //call function to wander/idle
            agent.isStopped = false;
            stagnantEnemy();
        }
    }

    //wandering around       
    void stagnantEnemy()
    {
        //wandering around code
        agent.SetDestination(player.position);
    }


    //chasing the player
    void huntingPlayer()
    {
        //chase after the player code
        agent.destination = playerPosition.position;
    }

    //attacking the player
    void playerAttack()
    {
        //not yet implemented

        //agent.stoppingDistance = playerDistance;

    }

    //facing the player
    void facingPlayer()
    {
        //triggered rotation
        UnityEngine.Quaternion rot = UnityEngine.Quaternion.LookRotation(playerDirection);
        transform.rotation = UnityEngine.Quaternion.Lerp(transform.rotation, rot, Time.deltaTime * faceSpeed);
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
