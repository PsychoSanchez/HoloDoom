using System;
using System.Collections.Generic;
using Assets.Scripts.Monsters;
using HoloToolkit.Sharing;
using HoloToolkit.Unity;
using UnityEngine;


public struct SpawnPoint
{
    public Vector3 position;
    public Quaternion rotation;
}

public class UpdateTimer
{
    public float Delay = 1.0f;
    public float TimePassed = 0f;
    public event EventHandler onTimeout;
    public void Tick(float delta)
    {
        TimePassed += delta;
        if (TimePassed >= Delay)
        {
            TimePassed -= Delay;

            var handler = onTimeout;
            if (handler != null)
            {
                handler(this, null);
            }
        }
    }
    public UpdateTimer(float delay)
    {
        Delay = delay;
    }
}

public enum UpdateTimerTypes
{
    Enemy = 0,
    Medkit,
    Ammo,
    Armor
}

public class GameManager : Singleton<GameManager>
{
    const float RAY_DEBUG_TIME = 100f;
    [Serializable]
    public struct WaveLevel
    {
        public int EnemyAmount;
        public float TimeBetweenEnemySpawn;
        public float TimeBetweenAmmoSpawn;
        public float TimeBetweenMedkitSpawn;
        public float TimeBetweenArmorSpawn;
    }
    public WaveLevel[] Waves;
    AudioSource audioSource;
    public Cacodemon monster;
    public Ammo[] Ammo;
    public Medkit[] Medkits;
    public Armor[] Armor;
    public int StartWave = 0;
    public event EventHandler GameStarted;
    public event EventHandler GameStopped;
    public Transform SpawnTransform;
    public float MaxSpawnDistance = 5.0f;
    public float MinSpawnDistance = 1.0f;
    public int SpawnDirections = 32;

    bool bPlaying = false;
    bool bSpawnPointsFound = false;
    int nCurrentWave;
    int nWaveEnemysSpawned;
    int nWaveEnemysKilled;
    long lLocalUserId;
    float lStartAngle = 0;
    float lEndAngle = 360;
    List<float> spawnDirections = new List<float>();
    Dictionary<UpdateTimerTypes, UpdateTimer> timers = new Dictionary<UpdateTimerTypes, UpdateTimer>();
    RaycastHit hit;
    UpdateTimer startTimer;

    // Use this for initialization
    void Start()
    {
        nCurrentWave = StartWave;
        lLocalUserId = SharingStage.Instance.Manager.GetLocalUser().GetID();
        InitWave();
    }

    private void InitWave()
    {
        var wave = Waves[nCurrentWave];
        var enemyTimer = new UpdateTimer(wave.TimeBetweenEnemySpawn);
        enemyTimer.onTimeout += SpawnEnemy;
        var ammoTimer = new UpdateTimer(wave.TimeBetweenAmmoSpawn);
        ammoTimer.onTimeout += SpawnAmmo;
        var armorTimer = new UpdateTimer(wave.TimeBetweenArmorSpawn);
        armorTimer.onTimeout += SpawnArmor;
        var medkitTimer = new UpdateTimer(wave.TimeBetweenMedkitSpawn);
        medkitTimer.onTimeout += SpawnMedkit;
        timers.Add(UpdateTimerTypes.Enemy, enemyTimer);
        timers.Add(UpdateTimerTypes.Ammo, ammoTimer);
        timers.Add(UpdateTimerTypes.Armor, armorTimer);
        timers.Add(UpdateTimerTypes.Medkit, medkitTimer);
    }

    // Update is called once per frame
    void Update()
    {
        switch (AppStateManager.Instance.GetAppState())
        {
            case AppState.Playing:
                if (!bSpawnPointsFound)
                {
                    GetAvailbleSpawnAngles();
                }

                ProcessGame();
                break;
            case AppState.Ready:
            default:
                break;
        }
    }

    public void EnemyKilled()
    {
        nWaveEnemysKilled++;
        var wave = Waves[this.nCurrentWave];
        if (wave.EnemyAmount != nWaveEnemysKilled)
        {
            return;
        }
        StartNextWave();
    }

    private void StartNextWave()
    {
        nCurrentWave++;
        if (nCurrentWave > Waves.Length)
        {
            UIManager.Instance.LogMessage("Game complete, Gratz");
            CompleteGame();
            return;
        }
        UIManager.Instance.LogMessage("Wave " + (nCurrentWave + 1) + " starts!");

        ResetGameParameters();
    }

    private void CompleteGame()
    {
        // Show complete sign or blow some shit
    }

    private void ResetGameParameters()
    {
        var wave = Waves[nCurrentWave];
        var et = timers[UpdateTimerTypes.Enemy];
        et.TimePassed = 0;
        et.Delay = wave.TimeBetweenEnemySpawn;

        var at = timers[UpdateTimerTypes.Armor];
        at.TimePassed = 0;
        at.Delay = wave.TimeBetweenArmorSpawn;

        var amt = timers[UpdateTimerTypes.Ammo];
        amt.TimePassed = 0;
        amt.Delay = wave.TimeBetweenAmmoSpawn;

        var mt = timers[UpdateTimerTypes.Medkit];
        mt.TimePassed = 0;
        mt.Delay = wave.TimeBetweenMedkitSpawn;

        nWaveEnemysKilled = 0;
        nWaveEnemysSpawned = 0;
    }

    private void ProcessGame()
    {
        foreach (var key in timers.Keys)
        {
            if (!timers.ContainsKey(key))
            {
                return;
            }
            timers[key].Tick(Time.deltaTime);
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
        EventHandler connectedEvent = GameStopped;
        if (connectedEvent != null)
        {
            connectedEvent(this, EventArgs.Empty);
        }
    }
    public void SpawnAmmo(object sender, EventArgs e)
    {
        var sp = GetSpawnPosition(2.0f);
        var ammo = this.Ammo[UnityEngine.Random.Range(0, this.Ammo.Length - 1)];
        Instantiate(ammo, sp.position, sp.rotation);
    }

    public void SpawnMedkit(object sender, EventArgs e)
    {
        var sp = GetSpawnPosition(2.0f);
        var medkit = this.Medkits[UnityEngine.Random.Range(0, this.Medkits.Length - 1)];
        Instantiate(medkit, sp.position, sp.rotation);
    }

    public void SpawnArmor(object sender, EventArgs e)
    {
        var sp = GetSpawnPosition(2.0f);
        var armor = this.Armor[UnityEngine.Random.Range(0, this.Armor.Length - 1)];
        Instantiate(armor, sp.position, sp.rotation);
    }

    public void SpawnEnemy(object sender, EventArgs e)
    {
        if (lLocalUserId != AppStateManager.Instance.HeadUserID)
        {
            return;
        }

        var wave = Waves[this.nCurrentWave];
        if (this.nWaveEnemysSpawned >= wave.EnemyAmount)
        {
            return;
        }
        nWaveEnemysSpawned++;
        EnemyManager.Instance.SpawnEnemy(EnemyManager.EnemyTypes.Cacodemon, GetSpawnPosition(MaxSpawnDistance, MinSpawnDistance));
    }

    private void GetAvailbleSpawnAngles()
    {
        var anchor = SpawnTransform.transform.position;
        for (int i = 1; i < SpawnDirections + 1; i++)
        {
            var angle = (float)i / (float)SpawnDirections * 360;
            Vector3 pos;

            pos.x = MaxSpawnDistance * Mathf.Sin(angle * Mathf.Deg2Rad);
            pos.z = MaxSpawnDistance * Mathf.Cos(angle * Mathf.Deg2Rad);
            pos.y = anchor.y;


            if (Physics.Raycast(anchor, pos, out hit, MaxSpawnDistance))
            {
                print("Found an object - distance: " + hit.point);
                if (Vector3.Distance(anchor, hit.point) <= MinSpawnDistance)
                {
                    Debug.DrawRay(anchor, pos, Color.red, RAY_DEBUG_TIME);
                    print("Too close");
                }
                else
                {
                    Debug.DrawRay(anchor, pos, Color.yellow, RAY_DEBUG_TIME);
                    spawnDirections.Add(angle);
                    print("ok");
                }
            }
            else
            {
                Debug.DrawRay(anchor, pos, Color.green, RAY_DEBUG_TIME);
                spawnDirections.Add(angle);
            }
        }
        bSpawnPointsFound = true;
    }

    Vector3 GetSpawnDirection(Vector3 center, float radius)
    {
        var index = (int)UnityEngine.Random.Range(0, spawnDirections.Count);
        float ang = spawnDirections[index];
        Vector3 pos;
        pos.x = radius * Mathf.Sin(ang * Mathf.Deg2Rad);
        pos.z = radius * Mathf.Cos(ang * Mathf.Deg2Rad);
        pos.y = center.y;
        return pos;
    }

    /// <summary>
    /// Calculates and returns position in local space
    /// </summary>
    /// <param name="radius">Radius to find point in</param>
    /// <returns></returns>
    SpawnPoint GetSpawnPosition(float radius)
    {
        SpawnPoint temp = new SpawnPoint();
        Vector3 center = SpawnTransform.position;
        if (SpawnTransform != null)
        {
            center.x = SpawnTransform.position.x;
            center.z = SpawnTransform.position.z;
        }
        var position = GetSpawnDirection(center, radius);
        Debug.DrawRay(center, position, Color.green, 10);

        temp.position = center + position;
        if (Physics.Raycast(center, position, out hit, radius))
        {
            print("Found an object - distance: " + hit.point);
            temp.position = hit.point;
        }

        temp.rotation = Quaternion.FromToRotation(Vector3.forward, center - position);
        return temp;
    }

    SpawnPoint GetSpawnPosition(float radius, float minDistance)
    {
        Vector3 center = SpawnTransform.position;
        float dist;
        SpawnPoint temp = new SpawnPoint();
        Vector3 position;
        Int16 failCount = 0;
        do
        {
            position = GetSpawnDirection(center, radius);
            temp.position = center + position;
            if (Physics.Raycast(center, position, out hit, radius))
            {
                print("Found an object - distance: " + hit.point);
                temp.position = hit.point;
            }
            dist = Vector3.Distance(temp.position, SpawnTransform.position);
            failCount++;
        } while (dist < minDistance && failCount < 3);

        Debug.DrawRay(center, position, Color.white, RAY_DEBUG_TIME);
        temp.rotation = Quaternion.FromToRotation(center, position);
        return temp;
    }
}
