using UnityEngine;
using System.Collections;

public class towerShoot_Tower : MonoBehaviour {
    public GameObject bullet;//bulletPrefab
    public Transform bulletSpawn;//bulletSpawn with Prefab
    bool activeShooting = false;
    GameObject target=null;

    // Use this for initialization
    void Start() {
    }

    void spawnBullet()
    {
        Instantiate(bullet, bulletSpawn.position, bulletSpawn.localRotation);//create an empty GameObject as child of cannon, positioned at the face of cannon
    }

    //SituationA(1 cannon, 1 enemy, Escape)
    void OnTriggerEnter(Collider col)
    {
        if (col.gameObject.tag == "enemy")
        {
            if (!activeShooting)
            {
                activeShooting = true;
                target = col.gameObject;//SituationA(1 enemy)when enemy1 enter, target=enemy1;
                                                 //SituationB(2 ~s)target==enemy1 till enemy1 out, aS=False
                                                 //Q: target=new enter(OnTriggerEnter first) or target=stay one(OnTriggerStay first)??
                                                 //Here assume OnTriggerStay called first~~, target here will= new round enemy1
                transform.GetChild(0).LookAt(target.transform.position);//cannon looks at enemy1
                InvokeRepeating("spawnBullet", 0, 0.5f);//cannon shoots enemy1 every 0.5 seconds
            }
        }
    }

    void OnTriggerStay(Collider col)
    {
        if (col.gameObject == target)  //SituationA(1 enemy): if enemy1 stay, col==target=="enemy1"; if enemy1 out, function won't be called, targetName=="none". 
                                       //SituationB(2 ~s): if enemy1 stay, target=="enemy1"; if enemy1 out, aS=False, target==null
                                       //Q: if there are 2 enemies, col will be enemy1 or enemy2(accord. to enter sequence)??????
                                       //Here assume enemy1~~~~~~
                                       //if enemy1 stay, col==target=="enemy1"; if enemy1 out, aS=False, col(enemy2)!=target(null)
        {
            transform.GetChild(0).LookAt(target.transform.position);//cannon traces enemy1(because OnTriggerStay called per frame)
        }
        else if (col.gameObject.tag == "enemy")// if enemy1 out, aS=False, col(enemy2)!=target(null)
        {
            activeShooting = true;
            target = col.gameObject;//SituationB(2 ~s)target==enemy2 till enemy2 out, aS=False
            transform.GetChild(0).LookAt(target.transform.position);//cannon look at enemy2
            InvokeRepeating("spawnBullet", 0, 0.5f);//cannon shoots enemy2 every 0.5 seconds
        }
    }

    void OnTriggerExit(Collider col)
    {
        if (col.gameObject == target)//
        {
            activeShooting = false;
            target = null;
            CancelInvoke("spawnBullet");
        }
    }





    void Update() {
    }

}
