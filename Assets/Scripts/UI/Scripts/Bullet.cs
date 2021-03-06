﻿using UnityEngine;
using System.Collections;

public class Bullet : MonoBehaviour {

    float velocity = 2f;


    void OnCollisionEnter(Collision collision)
    {
        //explosion = Instantiate(explosion_prefab) as GameObject;
        //explosion.transform.position = transform.position;
        //explosion.transform.localScale = new Vector3(0.5f, 0.5f, 0.5f);
        GameObject hit = collision.gameObject;

        Combat DamageHit = hit.GetComponent<Combat>();

        if (DamageHit)
            DamageHit.TakeDamage(1);

        if (hit.layer == 21)
        {
            if (hit.tag == "Stone")
            {
                GameObject tmpMaterialPart = hit.transform.parent.gameObject;
                tmpMaterialPart.AddComponent<Rigidbody>().mass = 4.0f;
                tmpMaterialPart.AddComponent<DestroyTimer>().destructionTime = 30.0f;
                tmpMaterialPart.layer = 12;
                //for (int i = 0; i < tmpMaterialPart.transform.childCount; i++)
                //{
                //    tmpMaterialPart.transform.GetChild(i).gameObject.layer = 12;
                //}

                tmpMaterialPart.transform.GetChild(0).gameObject.layer = 12;

                GameObject tmpTopPart = hit.transform.parent.parent.GetChild(0).gameObject;
                tmpMaterialPart.transform.parent = null;
                if (tmpTopPart.layer != 12)
                //if (tmpTopPart.transform.parent.childCount < 2 && tmpT    §opPart.ToString() == "shard1")
                {
                    tmpTopPart.AddComponent<Rigidbody>().mass = 4.0f; ;
                    tmpTopPart.AddComponent<DestroyTimer>().destructionTime = 30.0f;
                    tmpTopPart.layer = 12;
                    //for (int i = 0; i < tmpMaterialPart.transform.childCount; i++)
                    //{
                    //    tmpMaterialPart.transform.GetChild(i).gameObject.layer = 12;
                    //}
                    tmpTopPart.transform.GetChild(0).gameObject.layer = 12;
                }

            }
            else if (hit.tag == "Wood")
            {
                GameObject tmpMaterialParent = hit.transform.parent.parent.gameObject;
                //for (int i = 0; i < tmpMaterialParent.transform.childCount; i++)
                while(tmpMaterialParent.transform.childCount > 0)
                {
                    GameObject tmpMaterialPart = tmpMaterialParent.transform.GetChild(0).gameObject;
                    Rigidbody tmpRB = tmpMaterialPart.AddComponent<Rigidbody>();
                    tmpRB.drag = 1f;
                    tmpRB.angularDrag = 1f;
                    tmpMaterialPart.AddComponent<DestroyTimer>().destructionTime = 30.0f;
                    tmpMaterialPart.layer = 12;
                    //for (int i = 0; i < tmpMaterialPart.transform.childCount; i++)
                    //{
                    //    tmpMaterialPart.transform.GetChild(i).gameObject.layer = 12;
                    //}
                    tmpMaterialPart.transform.GetChild(0).gameObject.layer = 12;
                    tmpMaterialPart.transform.parent = null;
                }
                Destroy(tmpMaterialParent);
            }

        }

        // Destroy(gameObject);
    }

    // Use this for initialization
    void Start () {
        GetComponent<Rigidbody>().velocity = transform.forward * 40;
	}
	
	// Update is called once per frame
	void Update () {
        Destroy(this.gameObject, 0.5f);
	}
}
