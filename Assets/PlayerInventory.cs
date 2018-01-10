using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ConsumableType
{
    SmallMedkit = 0,
    BigMedkit,
    Regeneration,
    God,
    TimeSlow,
    InfiniteAmmo
}
public enum AmmoType
{
    Grenade = 0,
    PistolBullet,
    SMGBullet,
    ShotgunBullet
}
public struct Consumable
{
    public ConsumableType type;
    public int count;
}

public class WeaponInfo
{
    public WeaponType Type;
    public string Name;
    public Weapon Ref;
    public bool Unlocked;
}

public class PlayerInventory : MonoBehaviour
{
    public WeaponInfo[] Weapons;
    public List<Consumable> Consumables;
    public WeaponInfo CurrentWeapon;

    // Use this for initialization
    void Start()
    {

    }

    /* 
		Add consumable to list
	 */
    public void AddConsumable(ConsumableType type, int count = 1)
    {
        var index = Consumables.FindIndex((c) => c.type == type);

        if (index < 0)
        {
            Consumables.Add(new Consumable
            {
                type = type,
                count = count
            });
            return;
        }

        var consumable = Consumables[index];
        Consumables[index] = new Consumable
        {
            type = consumable.type,
            count = consumable.count
        };
    }

    public bool UseConsumable(ConsumableType type)
    {
		// Find consumable
		// Check if exist, check if count > 0
		// Reduce by 1, return used state
		return true;
    }

    public void UnlockWeapon(WeaponType type)
    {
        WeaponInfo weapon = GetWeapon(type);

        if (weapon == null)
        {
            return;
        }

        // Unlock weapon and add 10 ammo
        weapon.Unlocked = true;
        weapon.Ref.AddAmmo(10);
    }

    public void ChangeWeapon(WeaponType type)
    {
        WeaponInfo weapon = GetWeapon(type);
        if (weapon == null)
        {
            return;
        }

        CurrentWeapon = weapon;
    }

    private WeaponInfo GetWeapon(WeaponType type)
    {
        WeaponInfo weapon = null;
        for (int i = 0; i < Weapons.Length; i++)
        {
            if (Weapons[i].Type == type)
            {
                weapon = Weapons[i];
                break;
            }
        }

        return weapon;
    }
}