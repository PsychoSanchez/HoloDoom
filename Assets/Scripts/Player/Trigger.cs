using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Trigger : MonoBehaviour {

    void OnTriggerEnter(Collider collider)
    {
        if (collider.gameObject.name == "HoloLensCamera")
        {
            PlayerHealth playerHealth = collider.gameObject.GetComponent<PlayerHealth>();
            if(playerHealth == null) return;
            Projectile projectile = this.gameObject.GetComponent<Projectile>();
            if(projectile == null) return;
            playerHealth.TakeDamage(projectile.damage);
        }
    }
}
