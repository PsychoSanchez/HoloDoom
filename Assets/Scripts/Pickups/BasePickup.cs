using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum PickupType
{
    Medkit = 0,
    Armor,
    Ammo,
    Weapon,
    Bandages
}

public enum MedkitType
{
    Small,
    Medium,
    Big
}


public class BasePickup : OverridableMonoBehaviour
{
    public float LifeTime = 10.0f;
    public AudioClip PickupSound;
    public AudioClip SpawnSound;
    private float currentLifeTime;
    protected PickupType pickUpType;
    AudioSource audioSource;


    // Use this for initialization
    protected virtual void Start()
    {
		currentLifeTime = 0.0f;
		audioSource = GetComponent<AudioSource>();
        if (audioSource != null && SpawnSound != null)
        {
            audioSource.PlayOneShot(SpawnSound, 0.7F);
        }
    }

    // Update is called once per frame
    protected virtual void Update()
    {
        currentLifeTime += Time.deltaTime;
        if (currentLifeTime >= this.LifeTime)
        {
            currentLifeTime = 0.0f;
            Destroy(gameObject, 0.5f);
        }
    }

    public PickupType GetPickUpType()
    {
        return this.pickUpType;
    }

    protected void PlayPickUpSound()
	{
		audioSource = GetComponent<AudioSource>();
		if (audioSource == null || PickupSound == null)
        {
            Debug.Log("No pickup Sound");
            return;
        }
		AudioSource.PlayClipAtPoint(PickupSound, this.transform.position, 0.7F);
    }
}