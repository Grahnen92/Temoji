using UnityEngine;
using System.Collections;
//using UnityEngine.Networking;

public class Enemy1_Combat : MonoBehaviour
{

    public const int maxHealth = 10;

    //[SyncVar]
    public int health = maxHealth;

    public void TakeDamage(int amount)
    {
        //if (!isServer)
          //  return;
//        print("take damage amount======" + amount);
        health -= amount;
        if (health <= 0)
        {
            health = 0;
            Destroy(gameObject);
            Debug.Log("Dead!");
        }
    }
}
