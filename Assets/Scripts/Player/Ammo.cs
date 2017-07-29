using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ammo : MonoBehaviour {

    public int AmmoAmount = 10;
    public WeaponType WeaponType = 0;

	// Use this for initialization
	void Start () {
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public WeaponType GetWeaponType()
    {
        return WeaponType;
    }

    public int GetAmmo()
    {
        return AmmoAmount;
    }
    
}

public enum WeaponType
{
    Shotgun = 0,
    Pistol
}
