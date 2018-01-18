using System;
using System.Collections;
using System.Collections.Generic;
using ItemsModel;
using UnityEngine;

namespace ItemsModel
{

    [Serializable]
    public enum WeaponType
    {
        Shotgun = 0,
        DBShotgun,
        Pistol,
        Grenade
    }

    [Serializable]
    public enum ConsumableType
    {
        SmallMedkit = 0,
        BigMedkit,
        Regeneration,
        God,
        TimeSlow,
        InfiniteAmmo
    }

    [Serializable]
    public enum AmmoType
    {
        Grenade = 0,
        PistolBullet,
        SMGBullet,
        ShotgunShells
    }

    [Serializable]
    public class Consumable
    {
        public ConsumableType Type;
        public string Name;
        public int Count;
    }

    [Serializable]
    public class WeaponItem
    {
        public WeaponType Type;
        public string Name;
        public Weapon Ref;
        public bool Unlocked;
    }

    [Serializable]
    public class AmmoItem
    {
        public AmmoType Type;
        public string Name;
        public int Count;
    }
}

public class PlayerInventory : MonoBehaviour
{
    public AmmoItem[] Ammo;
    public WeaponItem[] Weapons;
    public Consumable[] Consumables;
    public int SelectedWeaponIndex = 0;

    private WeaponItem currentWeapon;
    Dictionary<AmmoType, AmmoItem> _ammo = new Dictionary<AmmoType, AmmoItem>();

    // Use this for initialization
    void Start()
    {
        InitAmmoDictionary();
        currentWeapon = Weapons[SelectedWeaponIndex];
    }

    private void InitAmmoDictionary()
    {
        for (int i = 0; i < this.Ammo.Length; i++)
        {
            var a = this.Ammo[i];
            if (_ammo.ContainsKey(a.Type))
            {
                _ammo[a.Type].Count += a.Count;
                continue;
            }
            _ammo.Add(a.Type, a);
        }

        this.Ammo = null;
    }

    private WeaponItem GetWeapon(WeaponType type)
    {
        WeaponItem weapon = null;
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

    public int GetAmmoCount(AmmoType type)
    {
        return this._ammo[type].Count;
    }

    /** Method to ask ammo from inventory
        For example: we have double barrel shotgun and we need 2 shells, 
        but we have only 1 and so inventory we'll return only 1 shell.
        Same if we have no ammo, method will return 0
     */
    public int GetAmmo(AmmoType type, int count)
    {
        if (!_ammo.ContainsKey(type))
        {
            return 0;
        }
        var a = _ammo[type];
        if (a.Count - count > -1)
        {
            a.Count -= count;
            return count;
        }

        return a.Count;
    }

    /* 
		Add consumable to list
	 */
    public void AddConsumable(ConsumableType type, int count = 1)
    {
        Consumable cons;
        for (int i = 0; i < Consumables.Length; i++)
        {
            cons = Consumables[i];
            if (cons.Type == type)
            {
                cons.Count += count;
                break;
            }
        }
    }

    public bool UseConsumable(ConsumableType type)
    {
        // Find consumable
        // Check if exist, check if count > 0
        // Reduce by 1, return used state
        return true;
    }

    /**
    Will unlock weapon if its locked and add ammo to it 
    or will add 10 shells to unlocked weapon */
    public void UnlockWeaponOrAddAmmo(WeaponType type)
    {
        WeaponItem weapon = GetWeapon(type);

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
        WeaponItem weapon = GetWeapon(type);
        if (weapon == null)
        {
            return;
        }

        currentWeapon = weapon;
    }
}