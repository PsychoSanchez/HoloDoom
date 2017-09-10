using UnityEngine;
using UnityEngine.UI;
using System;

public class PlayerHealth : MonoBehaviour
{

    public int StartHealth = 100;
    public int MaxHealth = 100;
    public int StartArmor = 0;
    public int MaxArmor = 200;
    public Text HealthText;
    public Text ArmorText;
    public Image DamageImage;
    public AudioSource DamageAudio;
    public GameObject DeathScreen;

    bool isDead = false;
    int currentHealth;
    int currentArmor;

    // Use this for initialization
    void Start()
    {
        currentHealth = StartHealth;
        currentArmor = StartArmor;
        UpdateUI();
    }

    // Update is called once per frame
    void Update()
    {

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
        UpdateUI();
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
            UpdateUI();
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
            UpdateUI();
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

    private void UpdateUI()
    {
        HealthText.text = currentHealth.ToString();
        ArmorText.text = currentArmor.ToString();
    }

    public void Die()
    {
        isDead = true;
        if (DeathScreen == null)
        {
            return;
        }
        DeathScreen.SetActive(true);
        // Toggle death screeen
    }
}
