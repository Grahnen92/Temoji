using UnityEngine;
using System.Collections;
//using UnityEngine.Networking;

public class Enemy1_Combat : Combat
{

    //public const int maxHealth = 10;

    //[SyncVar]
    // public int health = maxHealth;
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
            Destroy(gameObject);
            Debug.Log("Dead!");
        }


    }
}
