﻿using UnityEngine;
using System.Collections;

public class enemy_hover_bullet : MonoBehaviour
{

    //public GameObject explosion_prefab;

    // Use this for initialization
    void Start()
    {
        Destroy(gameObject, 1.5f);
    }

    void OnCollisionEnter(Collision coll)//bullet hits something, bullet disappears
    {
        Destroy(gameObject);

        Combat tmpCombat = coll.gameObject.GetComponent<Combat>();

        if (tmpCombat)
            tmpCombat.TakeDamage(3.0f);
        //explosion_prefab = Resources.Load("bot_explosion") as GameObject;


        ////Base_Combat base_combat = baseObject.GetComponent<Base_Combat>();
        /////base_combat.TakeDamage(10);
        //GameObject explosion = Instantiate(explosion_prefab) as GameObject;
        //explosion.transform.position = transform.position;
        //explosion.transform.localScale = new Vector3(0.5f, 0.5f, 0.5f);
    }

    void Update()
    {

    }

}
