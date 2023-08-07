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
    private bool canRun = true;
    private float stamina = 1;
    private float staminaMax = 1;
    private float staminaDecrease = 0.3f;
    private float staminaIncrease = 0.2f;
    private float staminaRefreshTime = 1.5f;
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
        if (canRun) {
            if (Input.GetButtonDown("Sprint"))
            {
                isSprinting = true;
            } 

            if (Input.GetButtonUp("Sprint"))
            {
                isSprinting = false;
            }
        }

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
        if (canRun) {
            if (stamina <= 0) { StartCoroutine(staminaRecharge()); }

            if (isSprinting && isMoving) { 
                currentSpeed = currentSpeed * sprintMod;
                stamina -= staminaDecrease * Time.deltaTime;
            } else {
                isSprinting = false;
                stamina += staminaIncrease * Time.deltaTime;
            }
        }

        stamina = Mathf.Clamp(stamina, 0f, staminaMax);
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

    //Enumerators----------------------
    IEnumerator staminaRecharge()
    {
        isSprinting = false;
        canRun = false;
        yield return new WaitForSeconds(staminaRefreshTime);
        canRun = true;
    }
    //---------------------------------
}
