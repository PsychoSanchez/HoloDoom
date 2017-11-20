using System;
using System.Collections;
using System.Collections.Generic;
using Assets.Scripts.Monsters;
using UnityEngine;

public class RadarBehaviourScript : MonoBehaviour
{
    public GameObject Enemy;
    public GameObject Target;
    public Texture Background;
    public Vector2 Size = new Vector2(200, 30);
    private GameObject[] enemies;
    private Dictionary<long, GameObject> enemiesMarkers = new Dictionary<long, GameObject>();
    private bool bEnemyListUpdated = false;
    const float searchAngle = 6f;
    // Use this for initialization
    void Start()
    {
        EnemyManager.Instance.EnemyAdded += EnemyAdded;
        enemies = EnemyManager.Instance.GetEnemiesList();
    }

    private void EnemyAdded(object sender, EventArgs e)
    {
        bEnemyListUpdated = true;
    }

    private void DrawMarker(GameObject target, GameObject marker)
    {
        float angle = CalculateDegree(target);
        float percent = 0f;

        if (angle > searchAngle)
        {
            percent = 1f;
        }
        else if (angle > -searchAngle)
        {
            angle += searchAngle;
            percent = angle / (searchAngle * 2);
        }
        float x = Size.x / 2 - (percent * Size.x);
        marker.transform.localPosition = new Vector3(x, 0, 0);
    }

    private static float CalculateDegree(GameObject target)
    {
        var playerPos = Camera.main.transform.forward;
        var enemyPos = target.transform.position - Camera.main.transform.position;
        var v1 = new Vector2(playerPos.x, playerPos.z);
        var v2 = new Vector2(enemyPos.x, enemyPos.z);
        int sign = Vector3.Cross(v1, v2).z < 0 ? -1 : 1;
        var angle = sign * Vector2.Angle(v1, v2);
        return angle;
    }
    
    void OnEnable()
    {
        foreach (var enemy in enemiesMarkers.Keys)
        {
            DestroyImmediate(enemiesMarkers[enemy]);
        }
        enemiesMarkers.Clear();
        enemies = EnemyManager.Instance.GetEnemiesList();
        bEnemyListUpdated = true;
    }

    private void UpdateMarkers()
    {
        foreach (var enemy in enemies)
        {
            if (enemy == null)
            {
                continue;
            }

            var bm = enemy.GetComponent<BaseMonster>();
            var alive = bm.IsAlive();
            if (!alive)
            {
                if (enemiesMarkers.ContainsKey(bm.Id))
                {
                    DestroyImmediate(enemiesMarkers[bm.Id]);
                    enemiesMarkers.Remove(bm.Id);
                }
                continue;
            }
            DrawMarker(enemy, enemiesMarkers[bm.Id]);
        }
        DrawMarker(AnchorPlacement.Instance.gameObject, Target);
    }

    // Update is called once per frame
    void Update()
    {
        if (bEnemyListUpdated)
        {
            bEnemyListUpdated = false;
            enemies = EnemyManager.Instance.GetEnemiesList();
            UpdateEnemyMarkersPool();
        }
        UpdateMarkers();
    }

    private void UpdateEnemyMarkersPool()
    {
        foreach (var enemy in enemies)
        {
            var bm = enemy.GetComponent<BaseMonster>();
            var alive = bm.IsAlive();
            if (enemiesMarkers.ContainsKey(bm.Id))
            {
                if (!alive)
                {
                    DestroyImmediate(enemiesMarkers[bm.Id]);
                    enemiesMarkers.Remove(bm.Id);
                }
            }
            else if (alive)
            {
                enemiesMarkers.Add(bm.Id, Instantiate(Enemy, Vector3.one, new Quaternion(), gameObject.transform));
            }
        }
    }
}
