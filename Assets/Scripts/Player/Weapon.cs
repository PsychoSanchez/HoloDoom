using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Weapon : MonoBehaviour {

    public Animation WeaponSprite;
    public Animation GunLight;
    public AudioSource GunAudio;
    public float ShootDelay = 0.15f;
    public float Range = 100f;
    public int Damage = 20;

    int shootableMask;
    bool canShoot = true;
    float lastShoot;
    Ray shootRay;
    RaycastHit shootHit;

    // Use this for initialization
    void Start () {
        WeaponSprite.enabled = false;
        shootableMask = LayerMask.GetMask("Shootable");
        GunLight.enabled = true;
    }
	
	// Update is called once per frame
	void Update () {
        lastShoot += Time.deltaTime;
        if (lastShoot >= ShootDelay)
        {
            canShoot = true;
        }
	}

    public void Shoot()
    {
        if (!canShoot)
        {
            return;
        }
        lastShoot = 0f;

        GunAudio.Play();
        GunLight.Play();

        shootRay.origin = transform.position;
        shootRay.direction = transform.forward;
        
        if (Physics.Raycast(shootRay, out shootHit, Range, shootableMask))
        {
           // Shoot enemies
        }
    }
}
