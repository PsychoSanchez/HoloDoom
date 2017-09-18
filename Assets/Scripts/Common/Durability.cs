using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Durability : OverridableMonoBehaviour
{
    public int CurrentHealth = 100;
    public int MaxHealth = 100;
    public int MinHealth = 1;
    public bool IsDead = false;

    // Use this for initialization
    void Start()
    {

    }

    public virtual void TakeDamage(int amt)
    {
        if (this.IsDead)
        {
            return;
        }

        CurrentHealth -= amt;

        if (CurrentHealth < MinHealth)
        {
            CurrentHealth = MinHealth - 1;
            // Trigger Death screen
            Die();
        }
    }

    protected virtual void Die()
    {
        IsDead = true;
        Destroy(gameObject);
    }
}
