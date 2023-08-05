using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

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

    //Start is called before the first frame update
    void Start()
    {
        enemySpawn();
    }

    //Update is called once per frame
    void Update()
    {
        //check to see if player is around, if so chase & attempt to kill, otherwise remain idle or wander
    }

    //spawn in enemies
    void enemySpawn()
    {
        //spawn



    }

    //enemy idles & wanders
    void stagnantEnemy()
    {
        //idle - not moving


        //wandering around



    }

    void huntingPlayer()
    {

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

            //chase player
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
