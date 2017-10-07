using System;
using System.Collections;
using System.Collections.Generic;
using Assets.Scripts.Monsters;
using HoloToolkit.Sharing;
using HoloToolkit.Unity;
using UnityEngine;

public class EnemyManager : Singleton<EnemyManager>
{
    public enum EnemyTypes
    {
        Cacodemon = 0,
        Default
    }
    public GameObject Cacodemon;
    private Dictionary<long, GameObject> enemiesPool = new Dictionary<long, GameObject>();
    // Use this for initialization
    void Start()
    {
        CustomMessages.Instance.MessageHandlers[CustomMessages.GameMessageID.SpawnEnemy] = EnemySpawn;
        CustomMessages.Instance.MessageHandlers[CustomMessages.GameMessageID.EnemyTransform] = UpdateEnemyTransform;
        CustomMessages.Instance.MessageHandlers[CustomMessages.GameMessageID.EnemyHit] = EnemyHit;
        CustomMessages.Instance.MessageHandlers[CustomMessages.GameMessageID.PlayerFound] = PlayerFound;
        CustomMessages.Instance.MessageHandlers[CustomMessages.GameMessageID.ShootProjectile] = EnemyShoot;
    }

    private void EnemyShoot(NetworkInMessage msg)
    {
        var userId = msg.ReadInt64();
        Debug.Log(CustomMessages.Instance.localUserID);
        Debug.Log(userId);
        if (CustomMessages.Instance.localUserID == userId)
        {
            return;
        }

        var enemyId = msg.ReadInt64();
        var enemy = TryGetEnemy(enemyId);
        if (enemy == null) return;
        enemy.GetComponent<BaseMonster>().Shoot(CustomMessages.Instance.ReadVector3(msg), CustomMessages.Instance.ReadQuaternion(msg));
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
        enemy.transform.rotation = CustomMessages.Instance.ReadQuaternion(msg);
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
        long userId = msg.ReadInt64();
        long enemyId = msg.ReadInt64();
        if (!enemiesPool.TryGetValue(enemyId, out enemy))
        {
            var demon = Instantiate(Cacodemon, CustomMessages.Instance.ReadVector3(msg), CustomMessages.Instance.ReadQuaternion(msg));
            demon.GetComponent<BaseMonster>().Id = enemyId;
            enemiesPool.Add(enemyId, demon);
        }

        return enemy;
    }

    public void SpawnEnemy(EnemyTypes type, SpawnPoint sp)
    {
        switch (type)
        {
            case EnemyTypes.Cacodemon:
                var demon = Instantiate(Cacodemon, sp.position, sp.rotation);
                var monster = demon.GetComponent<BaseMonster>();
                monster.Id = GenerateId();
                enemiesPool.Add(monster.Id, demon);
                CustomMessages.Instance.SendSpawnEnemy(monster.Id, sp.position, sp.rotation);
                break;
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

    private long GenerateId()
    {
        long id = -1;
        while (id == -1)
        {
            long newId = LongRandom(-10000, 10000, new System.Random());
            if (enemiesPool.ContainsKey(newId) || newId == -1 || newId < -10000 || newId > 10000)
            {
                continue;
            }

            id = newId;
        }

        return id;
    }
}
