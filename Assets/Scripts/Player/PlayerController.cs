using System.Collections;
using System.Collections.Generic;
using UnityEngine.VR.WSA.Input;
using UnityEngine;
using UnityEngine.UI;
using System;

public class PlayerController : OverridableMonoBehaviour
{
    private GestureRecognizer recognizer;
    private PlayerInventory inventory;
    private GameObject lastCollision;

    // Use this for initialization
    void Start()
    {
        InitRecognizer();
        inventory = GetComponent<PlayerInventory>();
    }

    private void InitRecognizer()
    {
        recognizer = new GestureRecognizer();
        recognizer.SetRecognizableGestures(GestureSettings.Tap);
        recognizer.TappedEvent += Recognizer_TappedEvent;
        recognizer.StartCapturingGestures();
    }

    private void Recognizer_TappedEvent(InteractionSourceKind source, int tapCount, Ray headRay)
    {
        OnClick(headRay);
    }

    private void OnClick(Ray headRay)
    {
        switch (AppStateManager.Instance.GetAppState())
        {
            case AppState.Playing:
                // Get weapon
                var weapon = inventory.CurrentWeapon;
                if (weapon == null)
                {
                    return;
                }
                if (!weapon.IsReloading)
                {
                    // Get ammo                    
                    var ammo = inventory.GetAmmo(weapon.AmmoType, weapon.DefaultAmmoPerShot);
                    // Shoot
                    weapon.Shoot(headRay, ammo);
                }
                break;
            case AppState.WaitingForAnchor:
                AnchorPlacement.Instance.SendMessage("OnSelect");
                break;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            OnClick(new Ray(Camera.main.transform.position, Camera.main.transform.forward));
        }
        else if (Input.GetKeyDown(KeyCode.N))
        {
            // NextWeapon();
        }
        else if (Input.GetKeyDown(KeyCode.M))
        {
            AppStateManager.Instance.SetAppState(AppState.Playing);
        }
        else if (Input.GetKeyDown(KeyCode.R))
        {
            AppStateManager.Instance.SetAppState(AppState.Ready);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        // Removes collision with same object several times
        if (other.tag != "Pickup" || other.gameObject == lastCollision)
        {
            return;
        }
        lastCollision = other.gameObject;
        PickupObject(other);
    }

    private void PickupObject(Collider other)
    {
        var type = other.GetComponent<BasePickup>().GetPickUpType();

        switch (type)
        {
            case PickupType.Ammo:
                PickupAmmo(other);
                break;
            case PickupType.Medkit:
                PickupMedkit(other);
                break;
            case PickupType.Armor:
                PickupArmor(other);
                break;
            default:
                break;
        }

        if (other.gameObject == null)
        {
            return;
        }

        Destroy(other.gameObject);
    }

    private void PickupAmmo(Collider other)
    {
        var ammo = other.GetComponent<AmmoPickup>();

        if (ammo == null)
        {
            return;
        }

        inventory.AddAmmo(ammo.Type ,ammo.GetAmmo());
    }

    private void PickupArmor(Collider other)
    {
        var armor = other.GetComponent<ArmorPickup>();

        if (armor == null)
        {
            return;
        }

        this.GetComponent<PlayerHealth>().AddArmor(armor.GetArmor());

    }

    private void PickupMedkit(Collider other)
    {
        var health = other.GetComponent<MedkitPickup>();

        if (health == null)
        {
            return;
        }

        this.GetComponent<PlayerHealth>().AddHealth(health.GetHealth());
    }
}
