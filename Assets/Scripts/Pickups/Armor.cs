using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Armor : BasePickup {
    public int ArmorAmount = 25;

    // Use this for initialization
    void Start ()
    {
        pickUpType = PickupType.Armor;
    }
	
	// Update is called once per frame
	void Update () {
		
	}

    public int GetArmor()
    {
        PlayPickUpSound();
        return ArmorAmount;
    }
}
