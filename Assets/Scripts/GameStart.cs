using System;
using Assets.Scripts.Monsters;
using HoloToolkit.Unity;
using UnityEngine;

public class GameStart : Singleton<GameStart>
{
    AudioSource audioSource;
    public Cacodemon monster;
    public Ammo[] Ammo;
    public Medkit[] Medkits;
    public Armor[] Armor;
    public float timeBetweenEnemySpawn = 7.0f;
    public float timeBetweenAmmoSpawn = 12.0f;
    public float timeBetweenMedkitSpawn = 25.0f;
    public float timeBetweenArmorSpawn = 30.0f;
    public event EventHandler GameStarted;
    public event EventHandler GameStoped;
    public Transform SpawnTransform;

    bool bPlaying = false;
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
        if (AppStateManager.Instance.GetCurrentAppState() != AppState.Ready)
        {
            return;
        }

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

        if (lastMedkitSpawn >= timeBetweenMedkitSpawn)
        {
            SpawnMedkit();
        }

        if (lastArmorSpawn >= timeBetweenArmorSpawn)
        {
            SpawnArmor();
        }
    }

    public void StartGame()
    {
        EventHandler connectedEvent = GameStarted;
        if (connectedEvent != null)
        {
            connectedEvent(this, EventArgs.Empty);
        }
    }

    public void StopGame()
    {
        bPlaying = false;
        EventHandler connectedEvent = GameStoped;
        if (connectedEvent != null)
        {
            connectedEvent(this, EventArgs.Empty);
        }
    }
    public void SpawnAmmo()
    {
        lastAmmoSpawn = 0f;
        var sp = GetSpawnPosition(2.0f);
        var ammo = this.Ammo[UnityEngine.Random.Range(0, this.Ammo.Length - 1)];
        Instantiate(ammo, sp.position, sp.rotation);
    }

    public void SpawnMedkit()
    {
        lastMedkitSpawn = 0f;
        var sp = GetSpawnPosition(2.0f);
        var medkit = this.Medkits[UnityEngine.Random.Range(0, this.Medkits.Length - 1)];
        Instantiate(medkit, sp.position, sp.rotation);

    }

    public void SpawnArmor()
    {
        lastArmorSpawn = 0f;
        var sp = GetSpawnPosition(2.0f);
        var armor = this.Armor[UnityEngine.Random.Range(0, this.Armor.Length - 1)];
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
        float ang = UnityEngine.Random.value * 360;
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
        if (SpawnTransform != null)
        {
            center.x = SpawnTransform.position.x;
            center.z = SpawnTransform.position.z;
        }
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