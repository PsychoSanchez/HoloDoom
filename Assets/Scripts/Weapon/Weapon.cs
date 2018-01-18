using System;
using System.Collections.Generic;
using Assets.Scripts.Monsters;
using ItemsModel;
using UnityEngine;

public class Weapon : OverridableMonoBehaviour
{
    public AmmoType AmmoType = AmmoType.PistolBullet;
    public int DefaultAmmoPerShot = 1;
    public Sprite[] WeaponSprites;
    public AudioSource GunAudio;
    public float ShootDelay = 0.15f;
    public float Range = 100f;
    public int BaseDamage = 20;
    public bool IsReloading
    {
        get
        {
            return !bCanShoot;
        }
    }

    protected CustomAnimator animator;
    protected bool bCanShoot = true;
    protected float lastShot;
    protected RaycastHit shotHit;

    // Use this for initialization
    protected void Start()
    {
        animator = new CustomAnimator(8, transform.GetChild(0).GetComponent<SpriteRenderer>());
        animator.AddAnimationSequence("Shoot", WeaponSprites);
    }

    // Update is called once per frame
    protected void Update()
    {
        animator.Update(Time.deltaTime);
        lastShot += Time.deltaTime;
        if (lastShot >= ShootDelay)
        {
            bCanShoot = true;
        }
    }

    public virtual void Shoot(Ray headRay)
    {
        var damage = DefaultAmmoPerShot * BaseDamage;
        _Shoot(headRay, damage);
    }

    public virtual void Shoot(Ray headRay, int ammoPerShot)
    {
        if (ammoPerShot > DefaultAmmoPerShot || ammoPerShot == 0)
        {
            // Play clatz sound
            return;
        }
        var damage = ammoPerShot * BaseDamage;
        _Shoot(headRay, damage);
    }

    protected void _Shoot(Ray headRay, int damage)
    {
        if (!bCanShoot)
        {
            return;
        }

        lastShot = 0f;
        bCanShoot = false;

        GunAudio.Play();
        animator.PlayOnce("Shoot");

        // Debug.DrawRay(headRay.origin, headRay.direction, Color.white,  20.0f, false);
        if (!Physics.Raycast(headRay, out shotHit, Range * 10))
        {
            return;
        }

        switch (shotHit.collider.tag)
        {
            case "Enemy":
                BaseMonster monster = shotHit.collider.GetComponent<BaseMonster>();

                // If its not monster 
                if (monster == null)
                {
                    var enemyHealth = shotHit.collider.GetComponent<Durability>();
                    enemyHealth.TakeDamage(damage);

                    return;
                }

                monster.GetHit(damage);
                EnemyManager.Instance.DamageTaken(monster.Id, damage);
                break;
            case "RemotePlayer":
                RemotePlayerHealth rp = shotHit.collider.GetComponent<RemotePlayerHealth>();

                if (rp == null)
                {
                    return;
                }

                rp.TakeDamage(damage / 5);
                break;
        }
    }
}