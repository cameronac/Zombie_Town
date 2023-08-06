using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class playerMovement : MonoBehaviour
{
    //Properties-----------------------
    private CharacterController controller;

    private float sprintMod = 2f;
    private float playerSpeed = 3.0f;
    private float jumpHeight = 0.5f;
    private float gravityValue = -9.8f;

    private bool groundedPlayer;
    private Vector3 playerVelocity;
    private Vector3 move_dir;

    //Movement Conditions
    private bool isSprinting = false;
    private bool isJumping = false;
    private bool isMoving = false;

    //Stamina
    private float stamina = 1;
    private float stamina_max = 1;
    private float stamina_decrease = 1f;
    private float stamina_increase = 1f;
    //---------------------------------

    //Main Methods---------------------
    private void Start()
    {
        controller = GetComponent<CharacterController>();
    }

    void Update()
    {
        UpdateInput();
        Move();
    }
    //---------------------------------

    //Custom Methods-------------------
    private void UpdateInput()
    {
        isSprinting = Input.GetButton("Sprint");    //Sprint
        isJumping = Input.GetButton("Jump");    //Jump
        move_dir = Input.GetAxisRaw("Horizontal") * transform.right + Input.GetAxisRaw("Vertical") * transform.forward;
    }

    private void Move()
    {
        float currentSpeed = playerSpeed;

        //Reset Gravity
        if (controller.isGrounded) { playerVelocity.y = 0f; }

        //Jumping
        if (isJumping && controller.isGrounded) { playerVelocity.y = Mathf.Sqrt(jumpHeight * -3.0f * gravityValue); }

        //Sprinting
        if (isSprinting && isMoving && stamina > 0) { 
            currentSpeed = currentSpeed * sprintMod;
            stamina -= stamina_decrease * Time.deltaTime;
        } else {
            stamina += stamina_increase * Time.deltaTime;
        }

        stamina = Mathf.Clamp(stamina, 0f, 1f);
        gameManager.instance.SetStamina(stamina);

        //Apply Gravity
        playerVelocity.y += gravityValue * Time.deltaTime;
        
        //Apply Move Speed
        move_dir = move_dir * currentSpeed;

        //Move
        controller.Move((move_dir + playerVelocity) * Time.deltaTime);

        if (move_dir.magnitude > 0)
        {
            isMoving = true;
        } else {
            isMoving = false;
        }
    }
    //---------------------------------
}
