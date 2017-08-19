using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCollider : MonoBehaviour
{
    GameObject lastCollision;
    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

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
        var ammo = other.GetComponent<Ammo>();

        if (ammo == null)
        {
            return;
        }

        this.GetComponent<PlayerShooting>().AddAmmo(ammo.GetAmmo());

    }

    private void PickupArmor(Collider other)
    {
        var armor = other.GetComponent<Armor>();

        if (armor == null)
        {
            return;
        }

        this.GetComponent<PlayerHealth>().AddArmor(armor.GetArmor());

    }

    private void PickupMedkit(Collider other)
    {
        var health = other.GetComponent<Medkit>();

        if (health == null)
        {
            return;
        }

        this.GetComponent<PlayerHealth>().AddHealth(health.GetHealth());
    }
}
