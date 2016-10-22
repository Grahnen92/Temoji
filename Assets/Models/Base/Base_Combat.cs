﻿using UnityEngine;
using System.Collections;

public class Base_Combat : MonoBehaviour {

    public const int maxHealth = 100;

    public int health = maxHealth;

    public void TakeDamage(int amount)
    {

        health -= amount;
        if (health <= 0)
        {
            health = 0;
            Debug.Log("Dead!");
            GameManager.base_alive = false;
            Destroy(gameObject);
        }
    }
}
