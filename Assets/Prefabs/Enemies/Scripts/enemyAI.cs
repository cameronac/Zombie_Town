using System;
using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using UnityEngine;
using UnityEngine.AI;

public class enemyAI : MonoBehaviour, IDamage
{
    [SerializeField] Renderer flashingModel;
    NavMeshAgent agent;
    public Transform endLocation;

    [SerializeField] int HP;
    [SerializeField] int enemySpeed;

    public int damage = 1;
    

    //Start is called before the first frame update
    void Start()
    {
        NavMeshAgent agent = GetComponent<NavMeshAgent>();
        agent.destination = endLocation.position;


        
            
    }

    //Update is called once per frame
    void Update()
    {
        //collision detected - missing detection area 

    }

    //wandering around       
    void stagnantEnemy()
    {
        //wandering around code
        //navmesh - path(get/set curr path)
        //navmesh - angularSpeed(max turning speed in degrees while following the path)
        //navmesh - autoBraking(should the agent brake auto to avoid overshooting the destination point?)
        //navmesh - destination(gets/attemps to set the destination of agent in world-space units)
        //navmesh - nextPosition(gets/sets the sim pos of the agent)
        //navmesh bool - updatePosition(get/set the transform pos is sync w simulated agent pos, default is true)

        //CalculatePath() - calculates path to a specified point & stores the resulting path
        //Move() - applies relative movement to current position
        //Raycast() - trace a straight path toward a target pos in the NavMesh without moving the agent
        //ResetPath() - clears current path
        //SetDestination() - set/updates the destination thus triggering the calculation for a new path
        //Warp() - warps the agent to provided position
        //NavMeshPath() - NavMeshPath constructor
        //HighQualityObstacleAvoidance() - enable highest precision, highest performance impact

        //transform - the Transform attached to the GameObject
        //Instantiate - clones the object original & returns the clone itself

    }


    //chasing the player
    void huntingPlayer()
    {
        //chase after the player code
    }

    //attacking the player
    void playerAttack()
    {
        //not yet implemented

    }

    //facing/follwoing the player
    void followPlayer()
    {
        //triggered rotation
        UnityEngine.Quaternion rot = UnityEngine.Quaternion.LookRotation(playerDirection);
        transform.rotation = UnityEngine.Quaternion.Lerp(transform.rotation, rot, Time.deltaTime * faceSpeed);

        //updating position to follow player
        UnityEngine.Vector3 lookAtPlayer = player.position;
        lookAtPlayer.y = transform.position.y;
        transform.LookAt(lookAtPlayer);
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

    float speed = 10;
    public LayerMask collisionMask;

    public void SetSpeed(float newSpeed)
    {
        speed = newSpeed;
    }
}
