using System.Collections;
using System.Collections.Generic;
using Assets.Scripts.Monsters;
using UnityEngine;

// TODO: Use in arcore/arkit version
public class EnemyAI : MonoBehaviour
{
    private BaseMonster monster;


    // Use this for initialization
    void Start()
    {
        monster = GetComponent<BaseMonster>();
    }

    // Update is called once per frame
    void Update()
    {

    }
}
