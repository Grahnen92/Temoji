using UnityEngine;
using System.Collections;
//using UnityEngine.Networking;

public class Collectable_Combat : Combat
{

    public GameObject collectable;
    public int nrOfCollsOnDeath = 1;
    //public const int maxHealth = 10;

    //[SyncVar]
    //public int health = maxHealth;

    public override void TakeDamage(float amount)
    {
        //if (!isServer)
        //  return;
        health -= amount;

        if (health <= 0)
        {
            health = 0;

            for(int i = 0; i < nrOfCollsOnDeath; i++)
            {
                float x = Random.Range(-1.0f, 1.0f);
                float z = Random.Range(-1.0f, 1.0f);
                float y = 2.0f;
                Vector3 offset = new Vector3(x, y, z);
                offset.Normalize();
                offset *= 4.0f;
                Vector3 col_spawn = transform.position + offset;


                GameObject new_col = (GameObject)Instantiate(collectable, col_spawn, Quaternion.identity);
            }

            Destroy(gameObject);
            Debug.Log("Dead!");
        }
    }

    public void TakeDamage(float amount, Vector3 collisionPoint)
    {
        //if (!isServer)
        //  return;
        health -= amount;


        if (health <= 0)
        {
            health = 0;
            for (int i = 0; i < nrOfCollsOnDeath; i++)
            {
                //GameObject new_col = (GameObject)Instantiate(collectable, collisionPoint, Quaternion.identity);
                float x = Random.Range(-1.0f, 1.0f);
                float z = Random.Range(-1.0f, 1.0f);
                float y = 2.0f;
                Vector3 offset = new Vector3(x, y, z);
                offset.Normalize();
                offset *= 4.0f;
                Vector3 col_spawn = transform.position + offset;


                GameObject new_col = (GameObject)Instantiate(collectable, col_spawn, Quaternion.identity);
            }

            Destroy(gameObject);
            Debug.Log("Dead!");
        }

    }
}
