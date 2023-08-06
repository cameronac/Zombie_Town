using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using static enemyAI;

public class enemyAI : MonoBehaviour, IDamage
{
    [SerializeField] Renderer model;
    [SerializeField] NavMeshAgent agent;
    [SerializeField] float maxHP;
    [SerializeField] int speed;
    [SerializeField] int facePlayerSpeed;

    bool playerSeen;
    Transform enemyTarget;
    Vector3 playerDirection;
    public int totalEnemies = 20;
    public bool enemySpawned = true;


    //Start is called before the first frame update
    void Start()
    {

    }

    //Update is called once per frame
    void Update()
    {
        //check to see if player is around - collision detection, if so chase & attempt to kill, otherwise wander

    }

    //wandering around       
    void stagnantEnemy()
    {
        //wandering around code
        
    }

    //chasing the player
    void huntingPlayer()
    {
        //chase player code

    }

    //enemy chases rotates & chases player - trigger/collision
    void enemyContact()
    {
        //collision detected
        if(playerSeen)
        {
            //triggered rotation
            Quaternion rot = Quaternion.LookRotation(playerDirection);
            transform.rotation = Quaternion.Lerp(transform.rotation, rot, Time.deltaTime * facePlayerSpeed);

            //call function to chase player
            huntingPlayer();
        }
        else if(!playerSeen)
        {
            stagnantEnemy();
        }

    }

    //enemy takes damage & apparates(for now)
    public void TakeDamage(float damage)
    {
        maxHP -= damage;

        if (maxHP <= 0)
        {
            Destroy(gameObject);
        }
    }

}
