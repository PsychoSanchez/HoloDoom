using UnityEngine;

public class Ammo : BasePickup
{
    public int AmmoAmount = 10;
    public WeaponType WeaponType = 0;


	// Use this for initialization
	protected override void Start () {
        base.Start();
        pickUpType = PickupType.Ammo;
    }
	
	// Update is called once per frame
	protected override void Update () {
        base.Update();
    }

    public WeaponType GetWeaponType()
    {
        return WeaponType;
    }

    public int GetAmmo()
    {
        PlayPickUpSound();
        return AmmoAmount;
    }
    
}

public enum WeaponType
{
    Shotgun = 0,
    Pistol
}
