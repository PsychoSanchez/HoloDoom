using Assets.Scripts.Monsters;
using UnityEngine;

public class GameStart : MonoBehaviour {
    
    AudioSource audioSource;
    public Cacodemon monster;
    public Ammo[] ammo;
    float lastEnemySpawn;
    float lastAmmoSpawn;
    float timeBetweenEnemySpawn = 5.0f;
    float timeBetweenAmmoSpawn = 5.0f;

    // Use this for initialization
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        lastEnemySpawn += Time.deltaTime;
        lastAmmoSpawn += Time.deltaTime;

        if (lastEnemySpawn >= timeBetweenEnemySpawn)
        {
            lastEnemySpawn = 0f;
            Vector3 center = Camera.main.transform.position;
            Vector3 pos = RandomCircle(center, 5.0f);
            Instantiate(monster, pos,  Quaternion.FromToRotation(Vector3.forward, center - pos));
        }

        if (lastAmmoSpawn >= timeBetweenAmmoSpawn)
        {
            lastAmmoSpawn = 0f;
            Vector3 center = Camera.main.transform.position;
            Vector3 pos = RandomCircle(center, 2.0f);
            var ammo = this.ammo[Random.Range(0, this.ammo.Length - 1)];
            Instantiate(ammo, pos, Quaternion.FromToRotation(Vector3.forward, center - pos));
        }
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
