using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using UnityEngine.Experimental.GlobalIllumination;
using static IPickup;

public class playerState : MonoBehaviour, IPickup, IDamage
{
    float health = 100;
    float healthMax = 100;

    [Header("Audio")]
    [SerializeField] AudioSource audio_source;
    [SerializeField] AudioClip[] player_hurt_audio;

    [Header("Other")]
    [SerializeField] GameObject flash_light;
    [SerializeField] float interact_distance = 1.5f;
    [SerializeField] bool has_pistol = false;
    [SerializeField] bool has_knife = false;

    [SerializeField] GameObject weaponHolder;
    private playerShoot pShoot;
    private CharacterController characterController;
    public static playerState instance;

    private Vector3 startPosition;

    
    [SerializeField] int meds;
    public List<int> KeyItems = new List<int>();
    
    public enum heldItems {pistol = 0, shotgun, knife, meds}
    public heldItems currItem;
    void Start()
    {
        currItem = 0;
        instance = this;
        startPosition = transform.position;
        pShoot = GetComponent<playerShoot>();
        characterController = GetComponent<CharacterController>();
        Respawn();  
    }
    
    void Update()
    {
        RaycastHit interactHit;
        bool isHit = Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward, out interactHit, interact_distance);
       
        if (isHit)
        {
            if (interactHit.collider)
            {
                IInteract iInteract = interactHit.collider.GetComponent<IInteract>();

                if (iInteract != null) {
                    gameManager.instance.ToggleInteract(isHit);

                    if (Input.GetButtonDown("Interact"))
                        iInteract.pressed();
                }
            }
        }
        else {
            gameManager.instance.ToggleInteract(false);
        }


        if (Input.GetButtonDown("Toggle Flashlight"))
        {
            flash_light.SetActive(!flash_light.activeSelf);
        }

        if (Input.GetAxis("Mouse ScrollWheel") > 0f)
        {
            ToggleItem(true);
        }
        else if (Input.GetAxis("Mouse ScrollWheel") < 0f)
        {
            ToggleItem(false);
        }

        if (!has_pistol)
        {
            pShoot.enabled = false;
            weaponHolder.SetActive(false);
        } else {
            pShoot.enabled = true;
            weaponHolder.SetActive(true);
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

        audio_source.clip = player_hurt_audio[Random.Range(0, player_hurt_audio.Length - 1)];
        audio_source.Play();

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

    public void ToggleItem(bool move)
    {
        if(!move && currItem == 0)
        {
            
        }
    }
    public bool has_key(int ID)
    {
        if(KeyItems.Contains(ID))
        {
            return true;
        }
        return false;
    }
}
