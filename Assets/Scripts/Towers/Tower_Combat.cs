using UnityEngine;
using System.Collections;

public class Tower_Combat : Combat {

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
            if(tag == "SlowingTower")
            {
                Destroy(gameObject);
                gameObject.GetComponent<SlowingTower>().Death();
            }
            else if (tag == "ProjectileTower")
            {
                Destroy(gameObject);
            }

            
        }

        print("took damage");
    }
}
