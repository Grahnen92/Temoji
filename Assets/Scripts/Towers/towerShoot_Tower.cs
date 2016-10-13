using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class towerShoot_Tower : MonoBehaviour {
    public GameObject bullet;//bulletPrefab
    public Transform bulletSpawn;//bulletSpawn with Prefab
    float bulletSpeed = 1;
    bool activeShooting = false;//Must have this flag, otherwise "spawnBullet" will be invoked many times, actually increasing frequency!!!
    List<GameObject> enemyList = new List<GameObject>();//put entering enemies into a list, linked one by one according to entering sequence,
                                                        //always take the top one, if any one out/die, it will be removed from the list
    void Start() { }

    void spawnBullet()
    {
        Vector3 direction = transform.GetChild(1).forward;
        print(direction);
        GameObject tmp_bullet=Instantiate(bullet, bulletSpawn.position, bulletSpawn.localRotation) as GameObject;//create an empty GameObject as a child of cannon, 
                                                                                                      //position it at the face of cannon
        tmp_bullet.GetComponent<Rigidbody>().AddForce(direction * bulletSpeed);
    }

    //SituationI[1 enemy OR 2 enemies(enemy[0] or enemy[1] escape/die first), 1 cannon]
    void OnTriggerEnter(Collider col)
    {
        print("enemyEnter: " + col.gameObject);
        print("enemyCount before Enter: " + enemyList.Count);
        enemyList.Add(col.gameObject);//add enemy into list, at the last position
        print("enemyCount after Enter: " + enemyList.Count);
        if (!activeShooting)//situationA(1 enemy): if enemy[0] enter, shoot enemy[0]
                            //situationB(2 ~s): "spawnBullet" & "LookAt" won't be invoked when enemy[not 0] enter;
                            //if enemy[0] out/die, "spawnBullet" cancleInvoked, enemy[0] removed, aS==false, LET "spawnBullet" & "LookAt" invoked  
        {
            print("activeShooting before Enter: " + activeShooting);
            if (enemyList[0] != null)
                transform.GetChild(1).LookAt(enemyList[0].transform.position);//cannon looks at enemy[0]
            //print("look at: " + enemyList[0].transform.position);
            InvokeRepeating("spawnBullet", 0, 1);//cannon shoots enemy[0] every 0.5 seconds
            activeShooting = true;
            print("activeShooting after Enter: " + activeShooting);
        }
    }

    void OnTriggerStay(Collider col)
    {
        //SituationA(1 enemy): if enemy[0] stay, shoot enemy[0]; if enemy[0] out/die, won't called
        //SituationB(2 ~s): if enemy[0] stay, shoot enemy[0]; if enemy[0] out/die, shoot current[0], which is enemy[1];
        if (enemyList[0] != null)
            transform.GetChild(1).LookAt(enemyList[0].transform.position);//cannon traces enemy[0](because OnTriggerStay called per frame)
        //print("look at: " + enemyList[0].transform.position);
        if (!activeShooting)
        {
            print("enemyStay: " + col.gameObject);
            print("activeShooting before Stay: " + activeShooting);
            InvokeRepeating("spawnBullet", 0, 1);
            activeShooting = true;
            print("activeShooting after Stay: " + activeShooting);
        }
    }

    void OnTriggerExit(Collider col)
    {
        print("enemyExit: " + col.gameObject);
        //SituationA(1 enemy): if enemy[0] out, "spawnBullet" cancleInvoked, enemy[0] removed, aS==false
        //SituationB(2 ~s): if enemy[not 0] out, remove it from list, "spawnBullet" still invoked, aS stil==true;
        //if enemy[0] out, remove it from list, LET "spawnBullet" cancleInvoked, aS==false
        if (col.gameObject == enemyList[0])
        {
            print("activeShooting before Exit: " + activeShooting);
            CancelInvoke("spawnBullet");
            activeShooting = false;
            print("activeShooting after Exit: " + activeShooting);
        }
        print("enemyCount before Exit: " + enemyList.Count);
        enemyList.Remove(col.gameObject);//Must remove, then original 2nd one automatically becomes 1st one
        print("enemyCount after Exit: " + enemyList.Count);
        if (enemyList.Count != 0)
            print("enemyNewHead: " + enemyList[0]);

    }

    void Update()//if enemy[0] die, LET "spawnBullet" cancleInvoked, aS==false
    {
        while (enemyList.Count != 0 && enemyList[0] == null)//remove until enemy[0] != null
        {
            print("activeShooting before Die: " + activeShooting);
            CancelInvoke("spawnBullet");
            activeShooting = false;
            print("activeShooting after Die: " + activeShooting);
            print("enemyCount before Die: " + enemyList.Count);
            enemyList.Remove(enemyList[0]);
            print("enemyCount after Die: " + enemyList.Count);
            if (enemyList.Count != 0)
                print("enemyNewHead: " + enemyList[0]);
        }
    }


    //SituationII[1 enemy OR 2 enemies(enemy[0] or enemy[1] escape/die first), 2 cannons]
    //if more than 1 cannon, all the functions(spawnBullet, OnTriggerEnter, OnTriggerStay, OnTriggerExit, Update)
    //will be on cannons instead of tower, and get rid of "GetChild(1)"
}
