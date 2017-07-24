using Assets.Scripts.Common;
using Assets.Scripts.Monsters;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameStart : MonoBehaviour {
    
    AudioSource audioSource;
    public Cacodemon monster;
    float lastSpawn;
    float timeBetweenSpawn = 5.0f;

    // Use this for initialization
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        lastSpawn += Time.deltaTime;
        if (lastSpawn >= timeBetweenSpawn)
        {
            lastSpawn = 0f;
            Vector3 center = Camera.main.transform.position;
            Vector3 pos = RandomCircle(center, 2.0f);
            Instantiate(monster, pos,  Quaternion.FromToRotation(Vector3.forward, center - pos));
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
