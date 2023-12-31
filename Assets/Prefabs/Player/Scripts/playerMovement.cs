//using Autodesk.Fbx;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class playerMovement : MonoBehaviour
{
    //Properties-----------------------

    [Header("Audio")]
    [SerializeField] AudioClip[] footstep_audio;
    [SerializeField] AudioClip[] jump_audio;

    private float footstep_time = 2.5f;

    private float footstep_current = 0f;

    private CharacterController controller;

    private float sprintSpeed = 6.0f;
    private float playerSpeed = 3.5f;
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
    private float stamina = 2;
    private float staminaMax = 2;
    private float staminaDecrease = 0.3f;
    private float staminaIncrease = 0.5f;
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
        move_dir = move_dir.normalized;
    }

    private void Move()
    {
        float currentSpeed = playerSpeed;

        //Reset Gravity
        if (controller.isGrounded) { playerVelocity.y = 0f; }

        //Jumping
        if (isJumping && controller.isGrounded) { 
            playerVelocity.y = Mathf.Sqrt(jumpHeight * -3.0f * gravityValue);

            AudioManager.instance.CreateSoundWithParent(jump_audio[Random.Range(0, jump_audio.Length - 1)], transform.position, transform);
        }

        //Sprinting
        if (canRun) {
            if (stamina <= 0) { StartCoroutine(staminaRecharge()); }

            if (isSprinting && isMoving) { 
                currentSpeed = sprintSpeed;
                stamina -= staminaDecrease * Time.deltaTime;
            } else {
                isSprinting = false;
                if(stamina <= staminaMax)
                {
                    stamina += staminaIncrease * Time.deltaTime;
                }
            }
        }

        stamina = Mathf.Clamp(stamina, 0f, staminaMax);

        if (gameManager.instance != null) {
            if (stamina == 0) {
                gameManager.instance.SetStamina(0);
            } else {
                gameManager.instance.SetStamina(stamina / staminaMax);
            }
        }

        //Apply Gravity
        playerVelocity.y += gravityValue * Time.deltaTime;
        
        //Apply Move Speed
        move_dir = move_dir * currentSpeed;
        Vector3 current_speed = (move_dir + new Vector3(playerVelocity.x, 0, playerVelocity.z)) * Time.deltaTime;

        //Grounded
        if (controller.isGrounded)
        {

            footstep_current += current_speed.magnitude;

            if (footstep_current > footstep_time)
            {
                AudioManager.instance.CreateSoundAtPosition(footstep_audio[Random.Range(0, footstep_audio.Length - 1)], transform.position, 0.05f);
                footstep_current = 0f;
            }
        }

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
