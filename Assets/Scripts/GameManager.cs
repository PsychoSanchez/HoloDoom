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
    public event EventHandler GameStoped;
    public Transform SpawnTransform;

    bool bPlaying = false;
    int currentWave;
    int waveEnemysSpawned;
    int waveEnemysKilled;
    long localUserId;
    Dictionary<UpdateTimerTypes, UpdateTimer> timers = new Dictionary<UpdateTimerTypes, UpdateTimer>();
    RaycastHit hit;

    // Use this for initialization
    void Start()
    {
        currentWave = StartWave;
        localUserId = SharingStage.Instance.Manager.GetLocalUser().GetID();
        InitWave();
    }

    private void InitWave()
    {
        var wave = Waves[currentWave];
        var enemyTimer = new UpdateTimer(wave.TimeBetweenEnemySpawn);
        enemyTimer.onTimeout += SpawmEnemy;
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
        if (AppStateManager.Instance.GetCurrentAppState() != AppState.Ready)
        {
            return;
        }

        // if(AppStateManager.Instance.)

        ProcessGame();
    }

    public void EnemyKilled()
    {
        waveEnemysKilled++;
        var wave = Waves[this.currentWave];
        if (wave.EnemyAmount != waveEnemysKilled)
        {
            return;
        }
        StartNextWave();
    }

    private void StartNextWave()
    {
        currentWave++;
        if (currentWave > Waves.Length)
        {
            UIManager.Instance.LogMessage("Game complete, Gratz");
            CompleteGame();
            return;
        }
        UIManager.Instance.LogMessage("Next wave " + (currentWave + 1));

        ResetGameParameters();
    }

    private void CompleteGame()
    {
        // Show complete sign or blow some shit
    }

    private void ResetGameParameters()
    {
        var wave = Waves[currentWave];
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

        waveEnemysKilled = 0;
        waveEnemysSpawned = 0;
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
        EventHandler connectedEvent = GameStoped;
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

    public void SpawmEnemy(object sender, EventArgs e)
    {
        if (localUserId != AppStateManager.Instance.HeadUserID)
        {
            return;
        }

        var wave = Waves[this.currentWave];
        if (this.waveEnemysSpawned >= wave.EnemyAmount)
        {
            return;
        }
        waveEnemysSpawned++;
        EnemyManager.Instance.SpawnEnemy(EnemyManager.EnemyTypes.Cacodemon, GetSpawnPosition(5.0f));
    }

    Vector3 RandomCircle(Vector3 center, float radius)
    {
        float ang = UnityEngine.Random.value * 360;
        Vector3 pos;
        pos.x = center.x + radius * Mathf.Sin(ang * Mathf.Deg2Rad);
        pos.z = center.z + radius * Mathf.Cos(ang * Mathf.Deg2Rad);
        pos.y = center.y;
        return pos;
    }

    /// <summary>
    ///     Calculates and returns position in local space
    /// </summary>
    /// <param name="radius">Radius to find point in</param>
    /// <returns></returns>
    SpawnPoint GetSpawnPosition(float radius)
    {
        SpawnPoint temp = new SpawnPoint();
        Vector3 center = Camera.main.transform.position;
        if (SpawnTransform != null)
        {
            center.x = SpawnTransform.position.x;
            center.z = SpawnTransform.position.z;
        }
        var position = RandomCircle(center, radius);
        //Debug.DrawRay(center, position, Color.green, 10);

        if (Physics.Raycast(center, position, out hit, radius))
        {
            print("Found an object - distance: " + hit.point);
            position = hit.point;
        }

        temp.position = position;
        temp.rotation = Quaternion.FromToRotation(Vector3.forward, center - position);
        return temp;
    }
}
