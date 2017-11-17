using UnityEngine;
using UnityEngine.UI;
using System;
using HoloToolkit.Sharing;

public class PlayerHealth : OverridableMonoBehaviour
{
    public int StartHealth = 100;
    public int MaxHealth = 100;
    public int StartArmor = 0;
    public int MaxArmor = 200;
    public int RespawnTime = 5;
    public Image DamageImage;
    public AudioSource DamageAudio;
    public GameObject DeathScreen;

    bool isDead = false;
    int currentHealth;
    int currentArmor;
    float spawnTime;

    // Use this for initialization
    void Start()
    {
        currentHealth = StartHealth;
        currentArmor = StartArmor;
        CustomMessages.Instance.MessageHandlers[CustomMessages.GameMessageID.RemoteUserReceiveDamage] = TakeDamageFromPlayer;
        UpdateStatus();
    }

    private void TakeDamageFromPlayer(NetworkInMessage msg)
    {
        var remoteUserId = msg.ReadInt64();
        var damagedUserId = msg.ReadInt64();
        if (CustomMessages.Instance.localUserID != damagedUserId)
        {
            return;
        }
        var dmgAmt = msg.ReadInt32();
        UIManager.Instance.LogMessage("FRIENDLY FIRE BY USER " + remoteUserId);
        TakeDamage(dmgAmt);
    }

    public void TakeDamage(int damage)
    {
        if (isDead)
        {
            return;
        }

        var ArmorDamage = (int)Math.Round(damage * 0.75);
        var diff = currentArmor - ArmorDamage;

        currentHealth -= (int)Math.Round(damage * 0.25);
        if (diff < 0)
        {
            currentHealth += diff;
            diff = 0;
        }
        currentArmor = diff;

        if (currentHealth < 1)
        {
            currentHealth = 0;
            // Trigger Death screen
            Die();
        }
        UpdateStatus();
        // DamageAudio.Play(); // play damage sound
    }

    public bool AddHealth(int amt)
    {
        if (isDead)
        {
            return false;
        }

        var bMaxHP = (currentHealth + amt) == MaxHealth;

        if (!bMaxHP)
        {
            currentHealth += amt;
            if (currentHealth >= MaxHealth)
            {
                currentHealth = MaxHealth;
            }
            UpdateStatus();
        }

        UpdateEmotions();

        return !bMaxHP;
    }

    public bool AddArmor(int amt)
    {
        if (isDead)
        {
            return false;
        }
        var bMaxArmor = (currentArmor + amt) == MaxArmor;

        if (!bMaxArmor)
        {
            currentArmor += amt;
            if (currentArmor >= MaxArmor)
            {
                currentArmor = MaxArmor;
            }
            UpdateStatus();
        }

        return !bMaxArmor;
    }

    private void UpdateEmotions()
    {
        // ChangeFaces Animations
        //if (currentHealth > 80)
        //{

        //}
        //else if(currentHealth > 60)
        //{

        //}
        //else if (currentHealth > 40)
        //{

        //}
        //else if (currentHealth > 20)
        //{

        //}
    }

    private void UpdateStatus()
    {
        UIManager.Instance.UpdateHealthAndAmmoCounters(currentHealth, currentArmor);
        CustomMessages.Instance.SendUserHealthUpdate(currentHealth);
    }

    public void Die()
    {
        isDead = true;
        UIManager.Instance.LogMessage("Respawning in 5 seconds");
        UIManager.Instance.SetMode(UIMode.Death);
        if (DeathScreen == null)
        {
            return;
        }
        DeathScreen.SetActive(true);
        // Toggle death screen
    }

    void Update()
    {
        if (!isDead)
        {
            return;
        }

        spawnTime += Time.deltaTime;
        if (spawnTime < RespawnTime)
        {
            return;
        }

        Respawn();
    }

    private void Respawn()
    {
        spawnTime = 0f;
        UIManager.Instance.LogMessage("FIGHT!");
        UIManager.Instance.SetMode(UIMode.Game);
        isDead = false;
        currentHealth = 100;
        UpdateStatus();
    }
}
