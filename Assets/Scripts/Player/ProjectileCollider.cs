using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileCollider : OverridableMonoBehaviour
{
    private PlayerHealth _enemyHealth;
    private Projectile _proj;
    void Start()
    {
        _proj = gameObject.GetComponent<Projectile>();
    }
    void OnTriggerEnter(Collider collider)
    {
        if (collider.gameObject.name != "Player") return;

        if (!(collider is BoxCollider)) return;

        if (_enemyHealth == null)
        {
            _enemyHealth = collider.gameObject.GetComponent<PlayerHealth>();
        }

        if (_enemyHealth == null || _proj == null) return;

        _enemyHealth.TakeDamage(_proj.damage);
        
        Destroy(gameObject);
    }
}
