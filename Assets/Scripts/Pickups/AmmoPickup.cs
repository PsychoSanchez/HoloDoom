using ItemsModel;
using UnityEngine;

public class AmmoPickup : BasePickup
{
    public int AmmoAmount = 10;
    public AmmoType Type = AmmoType.ShotgunShells; 


	// Use this for initialization
	protected override void Start () {
        base.Start();
        pickUpType = PickupType.Ammo;
    }
	
	// Update is called once per frame
	protected override void Update () {
        base.Update();
    }

    public AmmoType GetAmmoType()
    {
        return Type;
    }

    public int GetAmmo()
    {
        PlayPickUpSound();
        return AmmoAmount;
    }
    
}
