using UnityEngine;
using System.Collections;
//using UnityEngine.Networking;

public class Collectable_Combat : Combat
{

    public GameObject collectable;
    //public const int maxHealth = 10;

    //[SyncVar]
    //public int health = maxHealth;

    public override void TakeDamage(int amount)
    {
        //if (!isServer)
        //  return;
        //        print("take damage amount======" + amount);
        health -= amount;
        float x = Random.Range(-1.0f, 1.0f);
        float z = Random.Range(-1.0f, 1.0f);
        float y = 2.0f;
        Vector3 offset = new Vector3(x, y, z);
        offset.Normalize();
        offset *= 4.0f;
        Vector3 col_spawn = transform.position + offset;


        GameObject new_col = (GameObject)Instantiate(collectable, col_spawn, Quaternion.identity);

        if (health <= 0)
        {
            health = 0;
            Destroy(gameObject);
            Debug.Log("Dead!");
        }
    }

    public void TakeDamage(int amount, Vector3 collisionPoint)
    {
        //if (!isServer)
        //  return;
        //        print("take damage amount======" + amount);
        health -= amount;
        // float x = Random.Range(-1.0f, 1.0f);
        // float z = Random.Range(-1.0f, 1.0f);
        // float y = 2.0f;
        // Vector3 offset = new Vector3(x, y, z);
        // offset.Normalize();
        //offset *= 4.0f;
        // Vector3 col_spawn = transform.position + offset;
        GameObject new_col = (GameObject)Instantiate(collectable, collisionPoint, Quaternion.identity);


        if (health <= 0)
        {
            health = 0;
            Destroy(gameObject);
            Debug.Log("Dead!");
        }

    }
}
