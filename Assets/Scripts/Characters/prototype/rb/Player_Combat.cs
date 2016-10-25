using UnityEngine;
using System.Collections;

public class Player_Combat : Combat
{

    //public const int maxHealth = 100;

    // [SyncVar]
    //public int health = maxHealth;

    private GameObject head;
    private ParticleSystem smoke;
    private ParticleSystem sparks;
    private Material head_mat;
    private Material head_damaged_mat;

    void Start()
    {
        head = GameObject.Find("final_prototype_head");
        smoke = head.transform.Find("Smoke").gameObject.GetComponent<ParticleSystem>();
        sparks = head.transform.Find("Sparks").gameObject.GetComponent<ParticleSystem>();
        health = maxHealth;

        head_mat =  Resources.Load("head", typeof(Material)) as Material;
        head_damaged_mat = Resources.Load("head_damaged", typeof(Material)) as Material;
    }

    public override void TakeDamage(int amount)
    {
        // if (!isServer)
        //    return;
        print(health);
        health -= amount;
        if (health <= 0)
        {
            health = 0;
            Debug.Log("Player Dead!");
        }

        float healthNorm = (((float)maxHealth - (float)health) / (float)maxHealth);
        healthNorm = healthNorm * healthNorm;
        print(healthNorm);
        print(maxHealth);
        print(health);
        var em = smoke.emission;//.rate = chargeRatio * 500;
        em.rate = healthNorm * 6;
        

        em = sparks.emission;
        em.rate = healthNorm * 120;



        if (health < maxHealth / 2)
        {
            var rend = head.GetComponent<Renderer>().material = head_damaged_mat;
        }

        print("player took damage");
    }
}
