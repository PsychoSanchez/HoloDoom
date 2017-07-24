using System;
using System.Collections;
using System.Collections.Generic;
using Assets.Scripts.Monsters;
using UnityEngine;

[Serializable]
public class Weapon : MonoBehaviour {

    [SerializeField]
    public Animator WeaponSprite;
    public AudioSource GunAudio;
    public float ShootDelay = 0.15f;
    public float Range = 100f;
    public int Damage = 20;

    //mb protected???
    int shootableMask;
    bool canShoot = true;
    float lastShoot;
    Ray shootRay;
    RaycastHit shootHit;

    // Use this for initialization
    protected void Start () {
        // WeaponSprite.enabled = false;
        shootableMask = LayerMask.GetMask("Shootable");
    }
	
	// Update is called once per frame
	protected void Update () {
        lastShoot += Time.deltaTime;
        if (lastShoot >= ShootDelay)
        {
            canShoot = true;
        }
	}

    public virtual void Shoot(Ray headRay)
    {
        if (!canShoot)
        {
            return;
        }
        lastShoot = 0f;
        canShoot = false;

        GunAudio.Play();
        WeaponSprite.Play("Shoot");

        var tempObject = GameObject.FindGameObjectWithTag("Enemy");
        var enemy = tempObject.GetComponent<Cacodemon>();
        enemy.GetHit(100);

        //shootRay.origin = transform.position;
        //shootRay.direction = transform.forward;
        
        if (Physics.Raycast(headRay, out shootHit, Range, shootableMask))
        {
           // Shoot enemies
        }
    }
}

public class Shotgun : Weapon
{
    protected void Start()
    {
        base.Start();
    }

    protected void Update()
    {
        base.Update();
    }
}
