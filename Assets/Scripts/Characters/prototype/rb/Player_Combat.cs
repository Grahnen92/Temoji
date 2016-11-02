﻿using UnityEngine;
using System.Collections;

public class Player_Combat : Combat
{

    //public const int maxHealth = 100;

    // [SyncVar]
    //public int health = maxHealth;

    private GameObject head;
    private GameObject rWing;
    private GameObject lWing;
    private GameObject fWing;
    private GameObject bWing;
    private ParticleSystem smoke;
    private ParticleSystem sparks;
    public  Material head_mat;
    public Material head_damaged_mat;

    void Start()
    {
        head = GameObject.Find("final_prototype_head");
        smoke = transform.GetChild(3).gameObject.GetComponent<ParticleSystem>();
        sparks = transform.GetChild(4).gameObject.GetComponent<ParticleSystem>();
        health = maxHealth;

        rWing = GameObject.Find("final_prototype_rwing");
        lWing = GameObject.Find("final_prototype_lwing");
        fWing = GameObject.Find("final_prototype_fwing");
        bWing = GameObject.Find("final_prototype_bwing");

        //head_mat =  Resources.Load("head", typeof(Material)) as Material;
       // head_damaged_mat = Resources.Load("head_damaged", typeof(Material)) as Material;
    }

    public override void TakeDamage(float amount)
    {
        // if (!isServer)
        //    return;
        print(health);
        health -= amount;
        if (health <= 0)
        {
            health = 0;
            Debug.Log("Player Dead!");
            Destroy(head.transform.parent.GetChild(1).gameObject, 0.1f);
            head.transform.parent.GetChild(1).GetChild(0).parent = null;
            head.transform.parent.GetChild(1).GetChild(1).parent = null;
            head.transform.parent.GetChild(1).GetChild(2).parent = null;
            head.transform.parent.GetChild(1).GetChild(3).parent = null;
            
        }

        float healthNorm = (((float)maxHealth - (float)health) / (float)maxHealth);
        healthNorm = healthNorm * healthNorm;
        var em = smoke.emission;//.rate = chargeRatio * 500;
        em.rate = healthNorm * 6;
        

        em = sparks.emission;
        em.rate = healthNorm * 120;



        if (health < maxHealth / 2)
        {
            head.GetComponent<Renderer>().material = head_damaged_mat;
            rWing.GetComponent<Renderer>().material = head_damaged_mat;
            lWing.GetComponent<Renderer>().material = head_damaged_mat;
            fWing.GetComponent<Renderer>().material = head_damaged_mat;
            bWing.GetComponent<Renderer>().material = head_damaged_mat;
        }

        print("player took damage");
    }

    void Update()
    {
        health += 2 * Time.deltaTime;
    }
}
