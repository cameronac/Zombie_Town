using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using static IPickup;

public class playerState : MonoBehaviour, IPickup, IDamage
{
    float health = 100;
    float healthMax = 100;

    [SerializeField] float interact_distance = 1.5f;
    [SerializeField] bool has_pistol = false;
    [SerializeField] bool has_knife = false;
    
    [SerializeField] int bandages = 0;
    [SerializeField] int first_aid_kits = 0;

    [SerializeField] GameObject pistol;
    private playerShoot pShoot;
    private CharacterController characterController;
    public static playerState instance;

    private Vector3 startPosition;
    
    public List<int> KeyItems = new List<int>();
   
    void Start()
    {
        instance = this;
        startPosition = transform.position;
        pShoot = GetComponent<playerShoot>();
        characterController = GetComponent<CharacterController>();
        Respawn();  
    }
    
    void Update()
    {
        if (Input.GetButtonDown("Interact"))
        {
            RaycastHit interactHit;
            bool isHit = Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward, out interactHit, interact_distance);
            
            if (isHit)
            {
                if (interactHit.collider)
                {
                    IInteract iInteract = interactHit.collider.GetComponent<IInteract>();

                    if (iInteract != null) {
                        iInteract.pressed();
                    }
                }
            }
        }

        if (!has_pistol)
        {
            pShoot.enabled = false;
            pistol.SetActive(false);
        } else {
            pShoot.enabled = true;
            pistol.SetActive(true);
        }


    }

    public void PickupItem(Items type)
    {
        switch(type)
        {
            case Items.knife:
                has_knife = true;
                break;

            case Items.pistol:
                has_pistol = true;
                break;
        }
    }

    public void PickupFirstAid(FirstAid type, int amount)
    {
        switch(type)
        {
            case FirstAid.bandage:
                if (gameManager.instance != null)
                {
                    health += 40;
                    gameManager.instance.SetHealth(0.4f);
                }
                break;

            case FirstAid.first_aid_kit:
                if (gameManager.instance != null)
                {
                    health = 100;
                    gameManager.instance.SetHealth(1);
                }

                break;
        }
    }

    public void PickupAmmo(Ammo type, int amount)
    {
        switch(type)
        {
            case Ammo.pistol:
                if (pShoot != null)
                {
                    pShoot.AddAmmo(amount);
                }
                break;
        }
    }

    public void PickupKeyItem(int ID) 
    {
        KeyItems.Add(ID);
    }

    public void TakeDamage(float amount)
    {
        bool isDead = false;
        health -= amount;

        if (health <= 0)
        {
            isDead = true;
        }

        if (isDead)
        {
            if (gameManager.instance != null) {
                gameManager.instance.SetHealth(0);
                gameManager.instance.youLose();
            }
        }
        else
        {
            if (gameManager.instance != null) {
                gameManager.instance.SetHealth(health / healthMax);
            }
        }
    }
   
    public void Respawn()
    {
        characterController.enabled = false;
        if (gameManager.instance != null)
        {
            if (gameManager.instance.playerSpawnPos != null)
            {
                transform.position = gameManager.instance.playerSpawnPos.transform.position;
            }
            else
            {
                transform.position = startPosition;
            }

            health = healthMax;
            gameManager.instance.SetHealth(1);

        } else {
            transform.position = startPosition;
        }

        characterController.enabled = true;
    }
}
