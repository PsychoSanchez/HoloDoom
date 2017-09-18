using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Armor : BasePickup {
    public int ArmorAmount = 25;

    // Use this for initialization
    protected override void Start ()
    {
        base.Start();
        pickUpType = PickupType.Armor;
    }
	

    public int GetArmor()
    {
        PlayPickUpSound();
        return ArmorAmount;
    }
}
