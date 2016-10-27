using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class damaging_Tower : MonoBehaviour {
    public GameObject dBullet;//bulletPrefab
    double bulletSpeed = 50;
    bool activeShooting = false;//Must have this flag, otherwise "spawnBullet" will be invoked many times, actually increasing frequency!!!
    List<GameObject> enemyList = new List<GameObject>();//put entering enemies into a list, linked one by one according to entering sequence,
                                                        //always take the top one, if any one out/die, it will be removed from the list
    List<GameObject> bulletList = new List<GameObject>();
    
    double[] height_target = new double[4] { 2.4, 1.5, 1, 0.6 };
    double[] height_current = new double[4];
    double[] height_error = new double[4];
    double[] height_error_pre = new double[4] { 0.0, 0.0, 0.0, 0.0 };
    double[] height_integral = new double[4] { 0.0, 0.0, 0.0, 0.0 };
    double[] height_derivative = new double[4];
    double[] height_adjustment = new double[4];


    void Start()
    {
        //first 4 bullets
        bulletList.Add(Instantiate(dBullet, transform.GetChild(1).position, transform.GetChild(1).localRotation) as GameObject);//create an empty GameObject as a child of cannon
        bulletList.Add(Instantiate(dBullet, transform.GetChild(2).position, transform.GetChild(2).localRotation) as GameObject);
        bulletList.Add(Instantiate(dBullet, transform.GetChild(3).position, transform.GetChild(3).localRotation) as GameObject);
        bulletList.Add(Instantiate(dBullet, transform.GetChild(4).position, transform.GetChild(4).localRotation) as GameObject);
    }

    void bulletShoot()
    {
        Vector3 targetDirection;
        targetDirection = enemyList[0].transform.position - bulletList[0].transform.position;

        bulletList[0].GetComponent<Rigidbody>().AddForce(targetDirection * (float)bulletSpeed);
        bulletList.Remove(bulletList[0]);

    }

    void spawnBullet()
    {
        bulletShoot();
        bulletList.Add(Instantiate(dBullet, transform.GetChild(4).position, transform.GetChild(4).localRotation) as GameObject);//create an empty GameObject as a child of cannon
    }

    void OnTriggerEnter(Collider col)
    {
        enemyList.Add(col.gameObject);
        /*print("enemyEnter: " + col.gameObject);
        print("enemyCount after Enter: " + enemyList.Count);*/

        //ENTER
        //SituationA(1 enemy): if enemy[0] enter, LET "spawnBullet" invoked, aS=true;
        //SituationB(2 ~s): if enemy[not 0] enter, "spawnBullet" won't be invoked;
        ////if enemy[0] out/die, LET "spawnBullet" invoked, aS=true;
        if (!activeShooting)
        {
            InvokeRepeating("spawnBullet", 0, 1);
            activeShooting = true;
        }
    }

    void OnTriggerStay(Collider col)
    {
        //STAY
        //SituationA(1 enemy): if enemy[0] stay, won't called; if enemy[0] out/die, won't called
        //SituationB(2 ~s): if enemy[0] stay, won't called; if enemy[0] out/die, shoot new enemyList[0]
        if (!activeShooting)
        {
            /*print("enemyStay: " + col.gameObject);*/
            InvokeRepeating("spawnBullet", 0, 1);
            activeShooting = true;
        }
    }

    void OnTriggerExit(Collider col)
    {
        /*print("enemyExit: " + col.gameObject);*/

        //EXIT
        //SituationA(1 enemy): if enemy[0] out, remove it from list, LET "spawnBullet" cancleInvoked, aS=false
        //SituationB(2 ~s): if enemy[not 0] out, remove it from list, "spawnBullet" still invoked, aS stil==true;
        ////if enemy[0] out, remove it from list, LET "spawnBullet" cancleInvoked, aS==false
        if (col.gameObject == enemyList[0])
        {
            CancelInvoke("spawnBullet");
            activeShooting = false;
        }
        enemyList.Remove(col.gameObject);
        
        /*print("enemyCount after Exit: " + enemyList.Count);
        if (enemyList.Count != 0) print("enemyNewHead: " + enemyList[0]);*/

    }

    void Update()
    {
        //DIE
        //SituationA(1 enemy): if enemy[0] die, remove it from list, LET "spawnBullet" cancleInvoked, aS=false
        //SituationB(2 ~s): if enemy[not 0] die, ignore it temporally;
        ////if enemy[0] die, remove it from list, LET "spawnBullet" cancleInvoked, aS==false;
        ////keep doing this step until enemyList[0] != null or enemyList is empty.
        while (enemyList.Count != 0 && enemyList[0] == null)
        {
            CancelInvoke("spawnBullet");
            activeShooting = false;
            enemyList.Remove(enemyList[0]);
            
            /*print("enemyCount after Die: " + enemyList.Count);
            if (enemyList.Count != 0) print("enemyNewHead: " + enemyList[0]);*/
        }
        
        //HOVER
        if(bulletList[0] != null)
        {
            transform.GetChild(5).Rotate(0, -80 * Time.deltaTime, 0);
            for (int i = 0; i < 4; i++)
            {
                bulletList[0].transform.localScale = new Vector3(0.3f, 0.3f, 0.3f);
                bulletList[i].transform.Rotate(0, -90 * Time.deltaTime, 0);
                
                height_current[i] = bulletList[i].transform.position.y;
                height_error[i] = height_target[i] - height_current[i];
                height_integral[i] += height_error[i] * Time.deltaTime;
                height_derivative[i] = (height_error[i] - height_error_pre[i]) / Time.deltaTime;

                height_adjustment[i] = 1.0 * height_error[i] + 0.0 * height_integral[i] + 0.5 * height_derivative[i];//how to adjust?
                
                bulletList[i].GetComponent<Rigidbody>().AddForce(Vector3.up * (float)height_adjustment[i]);
                height_error_pre[i] = height_error[i];
            }
        }
    }
}
