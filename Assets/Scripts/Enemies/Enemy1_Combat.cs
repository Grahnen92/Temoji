﻿using UnityEngine;
using System.Collections;
//using UnityEngine.Networking;

public class Enemy1_Combat : Combat
{

    //public const int maxHealth = 10;

    //[SyncVar]
    // public int health = maxHealth;

    public GameObject explosion_prefab;
    void Start()
    {
        health = maxHealth;
        explosion_prefab = Resources.Load("bot_explosion") as GameObject;
    }
    

    public override void TakeDamage(float amount)
    {

        health -= amount;
        if (health <= 0)
        {
            health = 0;
            Destroy(gameObject);
            Debug.Log("Dead!");

            GameObject explosion = Instantiate(explosion_prefab) as GameObject;
            explosion.transform.position = transform.position;
            explosion.transform.localScale = new Vector3(0.5f, 0.5f, 0.5f);

            if (!transform.parent)
                return;

            GameObject tmpParent = transform.parent.gameObject;

            //outer child right
            GameObject tmpChildRight = tmpParent.transform.GetChild(1).GetChild(1).gameObject;
            tmpChildRight.transform.parent = null;
            tmpChildRight.AddComponent<SphereCollider>();
            tmpChildRight.AddComponent<Rigidbody>();
            tmpChildRight.layer = 22;
            tmpChildRight.AddComponent<DestroyTimer>().destructionTime = 4.0f;


            //outer child left
            GameObject tmpChildLeft = tmpParent.transform.GetChild(1).GetChild(0).gameObject;
            tmpChildLeft.transform.parent = null;
            tmpChildLeft.AddComponent<SphereCollider>();
            tmpChildLeft.AddComponent<Rigidbody>();
            tmpChildLeft.layer = 22;
            tmpChildLeft.AddComponent<DestroyTimer>().destructionTime = 4.0f;

            //outer child right
            tmpChildRight = tmpParent.transform.GetChild(0).GetChild(1).gameObject;
            tmpChildRight.transform.parent = null;
            tmpChildRight.AddComponent<SphereCollider>();
            tmpChildRight.AddComponent<Rigidbody>();
            tmpChildRight.layer = 22;
            tmpChildRight.AddComponent<DestroyTimer>().destructionTime = 4.0f;

            //outer child left
            tmpChildLeft = tmpParent.transform.GetChild(0).GetChild(0).gameObject;
            tmpChildLeft.transform.parent = null;
            tmpChildLeft.AddComponent<SphereCollider>();
            tmpChildLeft.AddComponent<Rigidbody>();
            tmpChildLeft.layer = 22;
            tmpChildLeft.AddComponent<DestroyTimer>().destructionTime = 4.0f;

            
        }


    }
}
