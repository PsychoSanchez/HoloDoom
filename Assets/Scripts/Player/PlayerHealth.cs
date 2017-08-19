using UnityEngine;
using UnityEngine.UI;

public class PlayerHealth : MonoBehaviour {

    public int StartHealth = 100;
    public int MaxHealth = 100;
    public Text HealthText;
    public Image DamageImage;
    public AudioSource DamageAudio;
    public GameObject DeathScreen;

    bool isDead = false;
    int currentHealth;

    // Use this for initialization
    void Start() {
        currentHealth = StartHealth;
    }

    // Update is called once per frame
    void Update() {

    }

    public void TakeDamage(int damage)
    {
        if (isDead)
        {
            return;
        }
        currentHealth -= damage;
        if(currentHealth < 1)
        {
            currentHealth = 0;
            // Trigger Death screen
            Die();
        }
        HealthText.text = currentHealth.ToString();
        // DamageAudio.Play(); // play damage sound

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

    public void Die()
    {
        isDead = true;
        if(DeathScreen == null)
        {
            return;
        }
        DeathScreen.SetActive(true);
        // Toggle death screeen
    }
}
