using UnityEngine;
using System.Collections;
using System;

public class Mortality : MonoBehaviour {

    public int initialHealth = 100;
    public int health { get; private set; }
    public bool dead { get; private set; }

    // events
    public delegate void OnDeathEvent();
    public event OnDeathEvent OnDeath = delegate { };
    public delegate void OnReviveEvent();
    public event OnReviveEvent OnRevive = delegate { };
    public delegate void OnHealthDownEvent(int value);
    public event OnHealthDownEvent OnHealthDown = delegate { };

    void Start()
    {
        // init
        Revive();
    }

    public void Revive()
    {
        health = initialHealth;
        dead = (health <= 0);
        OnRevive();
    }

    public void DecrementHealth(int amount)
    {
        health -= amount;
        OnHealthDown(amount);

        if (health <= 0 && !dead)
        {
            dead = true;
            OnDeath();
        }
    }
}
