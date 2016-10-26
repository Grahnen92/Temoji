using UnityEngine;
using System.Collections;

public class Base_Combat : Combat {

    // public const int maxHealth = 100;

    //public int health = maxHealth;

    void Start()
    {
        health = maxHealth;
    }

    public override void TakeDamage(int amount)
    {

        health -= amount;
        if (health <= 0)
        {
            health = 0;
            Debug.Log("Dead!");
            GameManager.base_alive = false;
            Destroy(gameObject);
        }

        print("took damage");
    }
}
