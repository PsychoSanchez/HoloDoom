using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Medkit : BasePickup
{
    public int HealthAmount = 25;
    public MedkitType Type;

    // Use this for initialization
    void Start()
    {
        pickUpType = PickupType.Medkit;
    }

    // Update is called once per frame
    void Update() {

    }

    public int GetHealth()
    {
        PlayPickUpSound();
        return HealthAmount;
    }
}

public enum MedkitType
{
    Small,
    Medium,
    Big
}