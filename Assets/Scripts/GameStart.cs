using Assets.Scripts.Monsters;
using UnityEngine;

public class GameStart : MonoBehaviour {
    
    AudioSource audioSource;
    public Cacodemon monster;
    public Ammo[] Ammo;
    public Medkit[] Medkits;
    public Armor[] Armor;
    public float timeBetweenEnemySpawn = 7.0f;
    public float timeBetweenAmmoSpawn = 12.0f;
    public float timeBetweenMedkitSpawn = 25.0f;
    public float timeBetweenArmorSpawn = 30.0f;

    float lastEnemySpawn;
    float lastAmmoSpawn;
    float lastArmorSpawn;
    float lastMedkitSpawn;

    // Use this for initialization
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        lastEnemySpawn += Time.deltaTime;
        lastAmmoSpawn += Time.deltaTime;
        lastArmorSpawn += Time.deltaTime;
        lastMedkitSpawn += Time.deltaTime;

        if (lastEnemySpawn >= timeBetweenEnemySpawn)
        {
            SpawmEnemy();
        }

        if (lastAmmoSpawn >= timeBetweenAmmoSpawn)
        {
            SpawnAmmo();
        }

        if(lastMedkitSpawn >= timeBetweenMedkitSpawn)
        {
            SpawnMedkit();
        }

        if(lastArmorSpawn >= timeBetweenArmorSpawn)
        {
            SpawnArmor();
        }
    }

    public void SpawnAmmo()
    {
        lastAmmoSpawn = 0f;
        Vector3 center = Camera.main.transform.position;
        Vector3 pos = RandomCircle(center, 2.0f);
        var ammo = this.Ammo[Random.Range(0, this.Ammo.Length - 1)];
        Instantiate(ammo, pos, Quaternion.FromToRotation(Vector3.forward, center - pos));
    }

    public void SpawnMedkit()
    {
        lastMedkitSpawn = 0f;
        Vector3 center = Camera.main.transform.position;
        Vector3 pos = RandomCircle(center, 2.0f);
        var medkit = this.Medkits[Random.Range(0, this.Medkits.Length - 1)];
        Instantiate(medkit, pos, Quaternion.FromToRotation(Vector3.forward, center - pos));

    }

    public void SpawnArmor()
    {
        lastArmorSpawn = 0f;
        Vector3 center = Camera.main.transform.position;
        Vector3 pos = RandomCircle(center, 2.0f);
        var armor = this.Armor[Random.Range(0, this.Armor.Length - 1)];
        Instantiate(armor, pos, Quaternion.FromToRotation(Vector3.forward, center - pos));
    }

    public void SpawmEnemy()
    {
        lastEnemySpawn = 0f;
        Vector3 center = Camera.main.transform.position;
        Vector3 pos = RandomCircle(center, 5.0f);
        Instantiate(monster, pos, Quaternion.FromToRotation(Vector3.forward, center - pos));
    }


    Vector3 RandomCircle(Vector3 center, float radius)
    {
        float ang = Random.value * 360;
        Vector3 pos;
        pos.x = center.x + radius * Mathf.Sin(ang * Mathf.Deg2Rad);
        pos.z = center.z + radius * Mathf.Cos(ang * Mathf.Deg2Rad);
        pos.y = center.y;
        return pos;
    }
}
