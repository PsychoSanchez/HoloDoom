using System;
using Assets.Scripts.Monsters;
using UnityEngine;

public class Weapon : OverridableMonoBehaviour
{
    public Animator WeaponSprite;
    public AudioSource GunAudio;
    public float ShootDelay = 0.15f;
    public float Range = 100f;
    public int Damage = 20;
    public int Ammo = 10;

    //mb protected???
    // MAYBE
    // int shootableMask;
    bool canShoot = true;
    float lastShot;
    RaycastHit shotHit;

    // Use this for initialization
    protected void Start()
    {
        // shootableMask = LayerMask.GetMask("Shootable");
    }

    // Update is called once per frame
    protected void Update()
    {
        lastShot += Time.deltaTime;
        if (lastShot >= ShootDelay)
        {
            canShoot = true;
        }
    }

    public void AddAmmo(int amount)
    {
        this.Ammo += amount;
    }

    public virtual void Shoot(Ray headRay)
    {
        if (!canShoot)
        {
            return;
        }

        if (Ammo <= 0)
        {
            // Play no ammo sound
            return;
        }

        lastShot = 0f;
        canShoot = false;
        this.Ammo -= 1;

        GunAudio.Play();
        WeaponSprite.Play("Shoot");

        // Debug.DrawRay(headRay.origin, headRay.direction, Color.white,  20.0f, false);
        if (!Physics.Raycast(headRay, out shotHit, Range * 10))
        {
            return;
        }

        switch (shotHit.collider.tag)
        {
            case "Enemy":
                BaseMonster monster = shotHit.collider.GetComponent<BaseMonster>();
                if (monster == null)
                {
                    var enemyHealth = shotHit.collider.GetComponent<Durability>();
                    enemyHealth.TakeDamage(Damage);

                    return;
                }
                monster.GetHit(Damage);
                EnemyManager.Instance.DamageTaken(monster.Id, Damage);
                break;
            case "RemotePlayer":
                RemotePlayerHealth rp = shotHit.collider.GetComponent<RemotePlayerHealth>();
                if (rp == null) return;
                rp.TakeDamage(Damage / 2);
                break;
        }
    }
}