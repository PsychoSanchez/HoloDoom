using Assets.Scripts.Common;
using Assets.Scripts.Monsters;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameStart : MonoBehaviour {
    
    AudioSource audioSource;
    public Cacodemon monster;
    float lastSpawn;
    float timeBetweenSpawn = 5000.0f;

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
            //if (monster != null && !monster.IsAlive())
            //{
                //Instantiate(monster, (Camera.main.transform.position + (Camera.main.transform.forward * 1.0f)));
            //}
        }
       
    }
}
