using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IPickup
{
    public enum Items { knife, pistol }
    public enum FirstAid { bandage, first_aid_kit }
    public enum Ammo { pistol }

    public void PickupItem(Items type);
    public void PickupFirstAid(FirstAid type, int amount);
    public void PickupAmmo(Ammo type, int amount);
    public void PickupKeyItem(int ID);

}