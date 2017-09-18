using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Medkit : BasePickup
{
    public int HealthAmount = 25;
    public MedkitType Type;

    // Use this for initialization
    protected override void Start()
    {
        base.Start();
        pickUpType = PickupType.Medkit;
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