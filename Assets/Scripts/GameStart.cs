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
    RaycastHit hit;

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
        var sp = GetSpawnPosition(2.0f);
        var ammo = this.Ammo[Random.Range(0, this.Ammo.Length - 1)];
        Instantiate(ammo, sp.position, sp.rotation);
    }

    public void SpawnMedkit()
    {
        lastMedkitSpawn = 0f;
        var sp = GetSpawnPosition(2.0f);
        var medkit = this.Medkits[Random.Range(0, this.Medkits.Length - 1)];
        Instantiate(medkit, sp.position, sp.rotation);

    }

    public void SpawnArmor()
    {
        lastArmorSpawn = 0f;
        var sp = GetSpawnPosition(2.0f);
        var armor = this.Armor[Random.Range(0, this.Armor.Length - 1)];
        Instantiate(armor, sp.position, sp.rotation);
    }

    public void SpawmEnemy()
    {
        lastEnemySpawn = 0f;
        var sp = GetSpawnPosition(5.0f);
        Instantiate(monster, sp.position, sp.rotation);
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

    SpawnPoint GetSpawnPosition(float radius)
    {
        SpawnPoint temp = new SpawnPoint();
        Vector3 center = Camera.main.transform.position;
        var position = RandomCircle(center, radius);
        //Debug.DrawRay(center, position, Color.green, 10);

        if (Physics.Raycast(center, position, out hit, radius))
        {
            print("Found an object - distance: " + hit.point);
            position = hit.point;
        }

        temp.position = position;
        temp.rotation = Quaternion.FromToRotation(Vector3.forward, center - position);
        return temp;
    }
}

struct SpawnPoint
{
    public Vector3 position;
    public Quaternion rotation;
}