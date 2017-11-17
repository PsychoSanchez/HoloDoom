using System;
using System.Collections;
using System.Collections.Generic;
using Assets.Scripts.Monsters;
using HoloToolkit.Sharing;
using HoloToolkit.Unity;
using UnityEngine;

public class EnemyManager : Singleton<EnemyManager>
{
    const float ENEMY_SYNC_TIME_PERIOD = .033f;
    public enum EnemyTypes
    {
        Cacodemon = 0,
        Default
    }
    public GameObject Cacodemon;
    public event EventHandler EnemyAdded;
    private Dictionary<long, GameObject> enemiesPool = new Dictionary<long, GameObject>();
    private Dictionary<long, GameObject> projectiles = new Dictionary<long, GameObject>();
    float lastUpdate = 0f;
    // Use this for initialization
    void Start()
    {
        CustomMessages.Instance.MessageHandlers[CustomMessages.GameMessageID.SpawnEnemy] = EnemySpawn;
        CustomMessages.Instance.MessageHandlers[CustomMessages.GameMessageID.EnemyTransform] = UpdateEnemyTransform;
        CustomMessages.Instance.MessageHandlers[CustomMessages.GameMessageID.EnemyHit] = EnemyHit;
        CustomMessages.Instance.MessageHandlers[CustomMessages.GameMessageID.PlayerFound] = PlayerFound;
        CustomMessages.Instance.MessageHandlers[CustomMessages.GameMessageID.ShootProjectile] = EnemyShoot;
        CustomMessages.Instance.MessageHandlers[CustomMessages.GameMessageID.ResetStage] = RemoveEnemies;
    }

    void Update()
    {
        lastUpdate += Time.deltaTime;

        if (lastUpdate > ENEMY_SYNC_TIME_PERIOD)
        {
            lastUpdate = 0;
            SendEnemyTransform();
        }
    }

    private void SendEnemyTransform()
    {
        Vector3 position;
        long enemyId;
        foreach (long key in enemiesPool.Keys)
        {
            enemyId = enemiesPool[key].GetComponent<BaseMonster>().Id;
            position = enemiesPool[key].transform.position;
            CustomMessages.Instance.SendEnemyTransform(enemyId, position);
        }
    }

    private void EnemyShoot(NetworkInMessage msg)
    {
        var userId = msg.ReadInt64();
        if (CustomMessages.Instance.localUserID == userId)
        {
            return;
        }

        var enemyId = msg.ReadInt64();
        var projId = msg.ReadInt64();
        var enemy = TryGetEnemy(enemyId);
        if (enemy == null) return;
        enemy.GetComponent<BaseMonster>().Shoot(projId, CustomMessages.Instance.ReadVector3(msg), CustomMessages.Instance.ReadQuaternion(msg));
    }

    public void AddProjectile(long id, GameObject proj)
    {
        if (!projectiles.ContainsKey(id))
        {
            return;
        }
        projectiles.Add(id, proj);
    }

    public void DestroyProjectile(NetworkInMessage msg)
    {
        var userId = msg.ReadInt64();
        if (userId == CustomMessages.Instance.localUserID)
        {
            return;
        }

        var projId = msg.ReadInt64();
        if (!projectiles.ContainsKey(projId))
        {
            return;
        }
        Destroy(projectiles[projId]);
        projectiles.Remove(projId);
    }

    private void PlayerFound(NetworkInMessage msg)
    {
        var userId = msg.ReadInt64();
        var enemyId = msg.ReadInt64();
        var enemy = TryGetEnemy(enemyId);
        if (enemy == null) return;
        enemy.GetComponent<BaseMonster>().FindPlayer(userId);
    }

    private void UpdateEnemyTransform(NetworkInMessage msg)
    {
        var enemy = GetEnemy(msg);
        enemy.transform.position = CustomMessages.Instance.ReadVector3(msg);
    }

    private void EnemyHit(NetworkInMessage msg)
    {
        var userId = msg.ReadInt64();
        if (userId == CustomMessages.Instance.localUserID)
        {
            return;
        }
        var enemyId = msg.ReadInt64();
        var enemy = TryGetEnemy(enemyId);
        if (enemy == null) return;
        var amt = msg.ReadInt32();
        enemy.GetComponent<BaseMonster>().GetHit(amt);
    }

    private void EnemySpawn(NetworkInMessage msg)
    {
        GetEnemy(msg);
    }

    private GameObject GetEnemy(NetworkInMessage msg)
    {
        GameObject enemy;
        msg.ReadInt64();
        long enemyId = msg.ReadInt64();
        if (!enemiesPool.TryGetValue(enemyId, out enemy))
        {
            var demon = Instantiate(Cacodemon, CustomMessages.Instance.ReadVector3(msg), CustomMessages.Instance.ReadQuaternion(msg));
            demon.GetComponent<BaseMonster>().Id = enemyId;
            enemiesPool.Add(enemyId, demon);
        }

        return enemy;
    }

    private void RemoveEnemies(NetworkInMessage msg)
    {
        foreach (var key in enemiesPool.Keys)
        {
            GameObject.DestroyImmediate(enemiesPool[key]);
            // enemiesPool.Remove(key);
        }
        enemiesPool.Clear();

        foreach (var key in projectiles.Keys)
        {
            var proj = projectiles[key];
            if (proj != null)
            {
                GameObject.DestroyImmediate(projectiles[key]);
            }
            // projectiles.Remove(key);
        }
        projectiles.Clear();
    }

    public void ResetStage()
    {
        CustomMessages.Instance.SendResetStage();
    }

    public void SpawnEnemy(EnemyTypes type, SpawnPoint sp)
    {
        switch (type)
        {
            case EnemyTypes.Cacodemon:
                var demon = Instantiate(Cacodemon, sp.position, sp.rotation);
                var monster = demon.GetComponent<BaseMonster>();
                monster.Id = GenerateEnemyId();
                enemiesPool.Add(monster.Id, demon);
                CustomMessages.Instance.SendSpawnEnemy(monster.Id, sp.position, sp.rotation);
                break;
            default:
                break;
        }

        var connectedEvent = EnemyAdded;
        if (connectedEvent != null)
        {
            connectedEvent(this, null);
        }
    }


    public void DamageTaken(long id, int amt)
    {
        CustomMessages.Instance.SendEnemyHit(id, amt);
    }

    public int GetSpawnedEnemiesCount()
    {
        return enemiesPool.Count;
    }

    public GameObject[] GetEnemiesList()
    {
        GameObject[] enemies = new GameObject[enemiesPool.Count];
        enemiesPool.Values.CopyTo(enemies, 0);
        return enemies;
    }

    private GameObject TryGetEnemy(long enemyId)
    {
        GameObject enemy;
        if (!enemiesPool.TryGetValue(enemyId, out enemy))
        {
            return null;
        }
        return enemy;
    }

    long LongRandom(long min, long max, System.Random rand)
    {
        long result = rand.Next((Int32)(min >> 32), (Int32)(max >> 32));
        result = (result << 32);
        result = result | (long)rand.Next((Int32)min, (Int32)max);
        return result;
    }

    private long GenerateId(Dictionary<long, GameObject> arr)
    {
        long id = -1;
        while (id == -1)
        {
            long newId = LongRandom(-10000, 10000, new System.Random());
            if (arr.ContainsKey(newId) || newId == -1 || newId < -10000 || newId > 10000)
            {
                continue;
            }

            id = newId;
        }

        return id;
    }
    public long GenerateEnemyId()
    {
        return GenerateId(enemiesPool);
    }

    public long GenerateProjectileId()
    {
        return GenerateId(projectiles);
    }
}
