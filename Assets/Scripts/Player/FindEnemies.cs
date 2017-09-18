using Assets.Scripts.Monsters;
using UnityEngine;

public class FindEnemies : OverridableMonoBehaviour
{
    public float radius = 3;
    private float time = 0.0f;
    public float deltaTime = 0.3f;
    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        time += Time.deltaTime;

        if (!(time >= deltaTime)) return;
        time = time - deltaTime;

        FindEnemy();
    }


    private void FindEnemy()
    {
        Vector3 playerPosition = this.gameObject.transform.position;

        Collider[] intersectingColliders = Physics.OverlapSphere(playerPosition, radius);
        foreach (Collider t in intersectingColliders)
        {
            BaseMonster monster = t.gameObject.GetComponent<BaseMonster>();
            if (monster == null) continue;

            monster.FindPlayer(this.gameObject.transform);
        }
    }
}