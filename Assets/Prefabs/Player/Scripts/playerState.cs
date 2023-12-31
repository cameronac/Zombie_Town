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
    [SerializeField] AudioClip pickup_audio;
    [SerializeField] AudioClip switch_audio;
    [SerializeField] AudioClip[] player_hurt_audio;

    [Header("Weapons")]
    [SerializeField] GameObject PistolHold;
    [SerializeField] GameObject ShotgunHold;
    [SerializeField] public GameObject KnifeHold;
    [SerializeField] public GameObject MedsHold;

    [SerializeField] public int pAmmo = 24;
    public int pMagazine = 0;
    [SerializeField] public int sAmmo = 16;
    public int sMagazine = 0;


    [Header("Other")]
    [SerializeField] GameObject flash_light;
    float interact_distance = 2f;
    [SerializeField] public bool has_pistol = false;
    [SerializeField] public bool has_shotgun = false;

    public playerShoot pShoot;
    private CharacterController characterController;
    public static playerState instance;

    private Vector3 startPosition;
    private Vector3 lastRespawnPosition;

    [SerializeField] public int medCount;
    public List<int> KeyItems = new List<int>();
    [SerializeField] public List<string> destroyItems = new List<string>();

    public int numCarParts;
    
    public enum heldItems {pistol, shotgun, knife, meds}
    public heldItems currItem;

    void Start()
    {
        PistolHold.SetActive(false);
        ShotgunHold.SetActive(false);
        KnifeHold.SetActive(true);
        MedsHold.SetActive(false);

        currItem = (heldItems)2;
        instance = this;
        startPosition = transform.position;
        pShoot = GetComponent<playerShoot>();
        characterController = GetComponent<CharacterController>();
        Respawn();
        gameManager.instance.SetAmmo(0, 0);
    }
     
    void Update()
    {   
        if (Time.timeScale > 0) {
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
                            iInteract.buttonPressed();
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

            if (Input.GetAxis("Mouse ScrollWheel") > 0f && !pShoot.isShooting && !pShoot.isAiming)
            {
                ToggleItem(true);
            }
            else if (Input.GetAxis("Mouse ScrollWheel") < 0f && !pShoot.isShooting && !pShoot.isAiming)
            {
                ToggleItem(false);
            }

            if (Input.GetButtonDown("A1") && !pShoot.isShooting && !pShoot.isAiming)
            {
                ToggleItem(true, 0);
            }

            if (Input.GetButtonDown("A2") && !pShoot.isShooting && !pShoot.isAiming)
            {
                ToggleItem(true, 1);
            }

            if (Input.GetButtonDown("A3") && !pShoot.isShooting && !pShoot.isAiming)
            {
                ToggleItem(true, 2);
            }

            if (Input.GetButtonDown("A4") && !pShoot.isShooting && !pShoot.isAiming)
            {
                ToggleItem(true, 3);
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
    }

    public void PickupItem(Items type)
    {
        switch(type)
        {
            case Items.pistol:
                has_pistol = true;
                currItem = (heldItems)0;
                ShotgunHold.SetActive(false);
                KnifeHold.SetActive(false);
                MedsHold.SetActive(false);

                PistolHold.SetActive(true);
                pShoot.enabled = true;
                gameManager.instance.SetAmmo(pMagazine, pAmmo);
                break;
            case Items.shotgun:
                has_shotgun = true;
                currItem = (heldItems)1;
                KnifeHold.SetActive(false);
                MedsHold.SetActive(false);
                PistolHold.SetActive(false);

                ShotgunHold.SetActive(true);
                pShoot.enabled = true;
                gameManager.instance.SetAmmo(sMagazine, sAmmo);
                break;
        }

        AudioManager.instance.CreateSoundAtPosition(pickup_audio, transform.position, 1);
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
                    if(currItem == heldItems.meds)
                    {
                        gameManager.instance.SetAmmo(0, medCount);
                    }
                }

                break;
        }

        AudioManager.instance.CreateSoundAtPosition(pickup_audio, transform.position, 1);
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

            case Ammo.shotgun:
                if (pShoot != null)
                {
                    pShoot.AddShotgunAmmo(amount);
                }
                break;
        }

        AudioManager.instance.CreateSoundAtPosition(pickup_audio, transform.position, 1);
    }

    public void PickupKeyItem(int ID) 
    {
        KeyItems.Add(ID);
    }

    public void TakeDamage(float amount)
    {
        bool isDead = false;
        StartCoroutine(gameManager.instance.playerFlashDamage());
        health -= amount;

        AudioManager.instance.CreateSoundAtPosition(player_hurt_audio[Random.Range(0, player_hurt_audio.Length - 1)], transform.position);

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

        }
        else
        {
            transform.position = startPosition;
        }

        characterController.enabled = true;
    }
    

    public void ToggleItem(bool move, int num = -1)
    {
        //reset everything
        // Do a while loop to see what the next available enum type is, once found, switch to that, even if it is the same item

        pShoot.enabled = false;
        PistolHold.SetActive(false);
        ShotgunHold.SetActive(false);
        KnifeHold.SetActive(false);
        MedsHold.SetActive(false);
        bool correctMove = false;
        bool playSound = true;
        heldItems prevItem = currItem;

        if(num != -1)
        {
            currItem = (heldItems)num;


            if (currItem == heldItems.pistol && has_pistol)
                correctMove = true;
            else if (currItem == heldItems.shotgun && has_shotgun)
                correctMove = true;
            else if (currItem == heldItems.meds && medCount > 0)
                correctMove = true;
            else if (currItem == heldItems.knife)
                correctMove = true;

            if(!correctMove || currItem == prevItem)
            {
                currItem = prevItem;
                playSound = false;
            }
        }
        else
        {
            while(!correctMove)
            {
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

                if (currItem == heldItems.pistol && has_pistol)            
                    correctMove = true;
                if (currItem == heldItems.shotgun && has_shotgun)
                    correctMove = true;
                if (currItem == heldItems.meds && medCount > 0 )
                    correctMove = true;
                if (currItem == heldItems.knife)
                    correctMove = true;

                if (currItem == prevItem)
                {
                    playSound = false;
                }
            }
        }

        if (playSound)
            AudioManager.instance.CreateOneDimensionalSound(switch_audio);

      
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
                gameManager.instance.SetAmmo(pMagazine, pAmmo);
                break;
            case heldItems.shotgun: //shotgun
                if (has_shotgun)
                {
                    pShoot.enabled = true;
                    ShotgunHold.SetActive(true);
                }
                else
                    pShoot.enabled = false;
                gameManager.instance.SetAmmo(sMagazine, sAmmo);
                break;
            case heldItems.knife:
                pShoot.enabled = true;
                KnifeHold.SetActive(true);
                gameManager.instance.SetAmmo(0, 0);
                break;
            case heldItems.meds:
                if (medCount > 0)
                {
                    pShoot.enabled = true;
                    MedsHold.SetActive(true);
                    gameManager.instance.SetAmmo(0, medCount);
                }
                else
                {
                    pShoot.enabled = false;
                    MedsHold.SetActive(false);
                    gameManager.instance.SetAmmo(0, 0);
                }

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
