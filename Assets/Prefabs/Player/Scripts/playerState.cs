using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static IPickup;

public class playerState : MonoBehaviour, IPickup, IDamage
{
    float health = 100;

    [SerializeField] bool has_pistol = false;
    [SerializeField] bool has_knife = false;
    
    [SerializeField] int pistol_ammo = 0;
    [SerializeField] int bandages = 0;
    [SerializeField] int first_aid_kits = 0;

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
                bandages += amount;
                break;

            case FirstAid.first_aid_kit:
                first_aid_kits += amount;
                break;
        }
    }

    public void PickupAmmo(Ammo type, int amount)
    {
        switch(type)
        {
            case Ammo.pistol:
                pistol_ammo += amount;
                break;
        }
    }

    public void TakeDamage(float amount)
    {
        health -= amount; 

        if (health <= 0)
        {

        }
    }

}
