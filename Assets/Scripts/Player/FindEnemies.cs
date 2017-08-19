using Assets.Scripts.Monsters;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FindEnemies : MonoBehaviour {
    public float radius = 10;
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {

        Vector3 playerPosition = this.gameObject.transform.position;

        Collider[] intersectingColliders = Physics.OverlapSphere(playerPosition, radius);
        for(int i = 0; i < intersectingColliders.Length; i++)
        {
            Debug.Log(intersectingColliders);
            BaseMonster monster = intersectingColliders[i].gameObject.GetComponent<BaseMonster>();
            if(monster!= null)
            {
                monster.FindPlayer(playerPosition);
            }
        }
    }
}
