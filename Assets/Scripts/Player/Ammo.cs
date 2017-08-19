using UnityEngine;

public class Ammo : MonoBehaviour {

    public int AmmoAmount = 10;
    public WeaponType WeaponType = 0;
    public float LifeTime = 20.0f;

    private float currentLifeTime;

	// Use this for initialization
	void Start () {
        currentLifeTime = 0;
    }
	
	// Update is called once per frame
	void Update () {
        currentLifeTime += Time.deltaTime;
        if(currentLifeTime >= this.LifeTime)
        {
            currentLifeTime = 0.0f;
            Destroy(gameObject, 0.5f);
        }
    }

    public WeaponType GetWeaponType()
    {
        return WeaponType;
    }

    public int GetAmmo()
    {
        return AmmoAmount;
    }
    
}

public enum WeaponType
{
    Shotgun = 0,
    Pistol
}
