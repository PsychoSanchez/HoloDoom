using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Trigger : MonoBehaviour {

    void OnTriggerEnter(Collider collider)
    {
        if (collider.gameObject.name != "Player") return;

        if (!(collider is BoxCollider)) return;
        PlayerHealth playerHealth = collider.gameObject.GetComponent<PlayerHealth>();
        if(playerHealth == null) return;
        Projectile projectile = this.gameObject.GetComponent<Projectile>();
        if(projectile == null) return;
        playerHealth.TakeDamage(projectile.damage);
    }
}
