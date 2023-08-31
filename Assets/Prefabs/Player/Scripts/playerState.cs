using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using UnityEngine.Experimental.GlobalIllumination;
using static IPickup;

public class playerState : MonoBehaviour, IPickup, IDamage
{
    public float health = 100;
    public float healthMax = 100;

    [Header("Audio")]
    [SerializeField] AudioSource audio_source;
    [SerializeField] AudioClip[] player_hurt_audio;

    [Header("Weapons")]
    [SerializeField] GameObject PistolHold;
    [SerializeField] GameObject ShotgunHold;
    [SerializeField] public GameObject KnifeHold;
    [SerializeField] GameObject MedsHold;

    [Header("Other")]
    [SerializeField] GameObject flash_light;
    [SerializeField] float interact_distance = 1.5f;
    [SerializeField] bool has_pistol = false;
    [SerializeField] bool has_shotgun = false;
    [SerializeField] bool has_knife = false;

    private playerShoot pShoot;
    private CharacterController characterController;
    public static playerState instance;

    private Vector3 startPosition;

    
    [SerializeField] public int medCount;
    public List<int> KeyItems = new List<int>();
    
    public enum heldItems {pistol, shotgun, knife, meds}
    public heldItems currItem;
    void Start()
    {
        PistolHold.SetActive(false);
        ShotgunHold.SetActive(false);
        KnifeHold.SetActive(false);
        MedsHold.SetActive(false);

        currItem = (heldItems)2;
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

        //if (!has_pistol)
        //{
        //    pShoot.enabled = false;
        //    weaponHolder.SetActive(false);
        //}
        //else
        //{
        //    pShoot.enabled = true;
        //    weaponHolder.SetActive(true);
            
        //}
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
                    medCount++;
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
        //reset everything
        pShoot.enabled = false;
        PistolHold.SetActive(false);
        ShotgunHold.SetActive(false);
        KnifeHold.SetActive(false);
        MedsHold.SetActive(false);

        int tryMove = (int)currItem;

        if(move)
            tryMove++;
        else
            tryMove--;

        if (tryMove <= -1)
            tryMove = 3;
        else if (tryMove >= 4)
            tryMove = 0;
      
        currItem = (heldItems)tryMove;
        switch(currItem)
        {
            case heldItems.pistol: //pistol
                if(has_pistol)
                {
                    pShoot.enabled = true;
                    PistolHold.SetActive(true);
                }
                else
                    pShoot.enabled = false;
                break;
            case heldItems.shotgun: //shotgun
                if (has_shotgun)
                {
                    pShoot.enabled = true;
                    ShotgunHold.SetActive(true);
                }
                else
                    pShoot.enabled = false;
                break;
            case heldItems.knife:
                if (has_knife)
                {
                    pShoot.enabled = true;
                    KnifeHold.SetActive(true);
                }
                else
                    pShoot.enabled = false;
                break;
            case heldItems.meds:
                if (medCount > 0)
                {
                    pShoot.enabled = true;
                    MedsHold.SetActive(true);
                }
                else
                    pShoot.enabled = false;
                break;
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
