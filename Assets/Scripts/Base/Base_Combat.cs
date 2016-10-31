using UnityEngine;
using System.Collections;

public class Base_Combat : Combat {

    // public const int maxHealth = 100;

    //public int health = maxHealth;
    public Material damaged_material;

    void Start()
    {
        health = maxHealth;
        ParticleSystem ps = transform.GetChild(13).gameObject.GetComponent<ParticleSystem>();
        var em = ps.emission;//.rate = chargeRatio * 500;
        em.rate = 0.0f;
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
        if (health <= 3.0f * maxHealth / 4.0f)
        {
            transform.GetChild(5).gameObject.AddComponent<BoxCollider>();
            transform.GetChild(5).gameObject.AddComponent<Rigidbody>();

            transform.GetChild(9).gameObject.GetComponentInChildren<ParticleSystem>().Play();

        }
        if (health <= 2.0f * maxHealth / 3.0f)
        {
            transform.GetChild(0).GetComponent<Renderer>().enabled = false;
            transform.GetChild(1).GetComponent<Renderer>().enabled = true;
        }
        if(health <= maxHealth / 2.0f)
        {
            transform.GetChild(0).GetComponent<Renderer>().material = damaged_material;
            transform.GetChild(1).GetComponent<Renderer>().material = damaged_material;
            transform.GetChild(2).GetComponent<Renderer>().material = damaged_material;

            transform.GetChild(3).GetComponent<Renderer>().enabled = false;
            transform.GetChild(4).GetComponent<Renderer>().enabled = true;

            transform.GetChild(6).gameObject.AddComponent<BoxCollider>();
            transform.GetChild(6).gameObject.AddComponent<Rigidbody>();

            transform.GetChild(10).gameObject.GetComponentInChildren<ParticleSystem>().Play();
        }
        if (health <= 1.0f * maxHealth / 3.0f)
        {
            transform.GetChild(1).GetComponent<Renderer>().enabled = false;
            transform.GetChild(2).GetComponent<Renderer>().enabled = true;
        }
        if (health <= 1.0f * maxHealth / 4.0f)
        {
            transform.GetChild(7).gameObject.AddComponent<BoxCollider>();
            transform.GetChild(7).gameObject.AddComponent<Rigidbody>();

            transform.GetChild(11).gameObject.GetComponentInChildren<ParticleSystem>().Play();
        }
        float healthNorm = (((float)maxHealth - (float)health) / (float)maxHealth);
        healthNorm = healthNorm * healthNorm;
        ParticleSystem ps = transform.GetChild(13).gameObject.GetComponent<ParticleSystem>();
        var em = ps.emission;//.rate = chargeRatio * 500;
        em.rate = healthNorm * 35;


        print("took damage");
    }
}
