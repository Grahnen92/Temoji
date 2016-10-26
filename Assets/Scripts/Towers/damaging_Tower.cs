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
    //height hovering
    double[] height_target = new double[4] { 2.7, 1.8, 1.2, 0.7 };
    double[] height_current = new double[4];
    double[] height_error = new double[4];
    double[] height_error_pre = new double[4] { 0.0, 0.0, 0.0, 0.0 };
    double[] height_integral = new double[4] { 0.0, 0.0, 0.0, 0.0 };
    double[] height_derivative = new double[4];
    double[] height_adjustment = new double[4];

    //double[] height_target = new double[3] { 2.7, 1.8, 0.7 };
    //double[] height_current = new double[3];
    //double[] height_error = new double[3];
    //double[] height_error_pre = new double[3] { 0.0, 0.0, 0.0 };
    //double[] height_integral = new double[3] { 0.0, 0.0, 0.0 };
    //double[] height_derivative = new double[3];
    //double[] height_adjustment = new double[3];

    /*//radius hovering
    double[] radius_target = new double[4] { 0, 2, 1, 0 };
    double[] radius_current = new double[4];
    double[] radius_error = new double[4];
    double[] radius_error_pre = new double[4] { 0.0, 0.0, 0.0, 0.0 };
    double[] radius_integral = new double[4] { 0.0, 0.0, 0.0, 0.0 };
    double[] radius_derivative = new double[4];
    double[] radius_adjustment = new double[4];
    Vector2[] radius_normal_out = new Vector2[4];
    Vector2[] radius_normal_in = new Vector2[4];
    */


    void Start()
    {
        //first 4 bullets: bulletList[0], bulletList[1], bulletList[2], bulletList[3]
        bulletList.Add(Instantiate(dBullet, transform.GetChild(1).position, transform.GetChild(1).localRotation) as GameObject);//create an empty GameObject as a child of cannon
        bulletList.Add(Instantiate(dBullet, transform.GetChild(2).position, transform.GetChild(2).localRotation) as GameObject);
        bulletList.Add(Instantiate(dBullet, transform.GetChild(3).position, transform.GetChild(3).localRotation) as GameObject);
        bulletList.Add(Instantiate(dBullet, transform.GetChild(4).position, transform.GetChild(4).localRotation) as GameObject);
    }

    void spawnBullet()
    {
        Vector3 direction = transform.GetChild(1).forward;
        
        bulletList[0].GetComponent<Rigidbody>().AddForce(direction * (float)bulletSpeed);
        bulletList.Remove(bulletList[0]);

        bulletList.Add(Instantiate(dBullet, transform.GetChild(4).position, transform.GetChild(4).localRotation) as GameObject);//create an empty GameObject as a child of cannon
        //bulletList.Add(Instantiate(dBullet, transform.GetChild(3).position, transform.GetChild(3).localRotation) as GameObject);//create an empty GameObject as a child of cannon
    }

    //SituationI[1 enemy OR 2 enemies(enemy[0] or enemy[1] escape/die first), 1 cannon]
    void OnTriggerEnter(Collider col)
    {
        print("enemyEnter: " + col.gameObject);
        enemyList.Add(col.gameObject);//add enemy into list, at the last position
        print("enemyCount after Enter: " + enemyList.Count);
        if (!activeShooting)//situationA(1 enemy): if enemy[0] enter, shoot enemy[0]
                            //situationB(2 ~s): "spawnBullet" & "LookAt" won't be invoked when enemy[not 0] enter;
                            //if enemy[0] out/die, "spawnBullet" cancleInvoked, enemy[0] removed, aS==false, LET "spawnBullet" & "LookAt" invoked  
        {
            if (enemyList[0] != null)
                transform.GetChild(1).LookAt(enemyList[0].transform.position);//cannon looks at enemy[0]
            InvokeRepeating("spawnBullet", 0, 1);//cannon shoots enemy[0] every 0.5 seconds
            activeShooting = true;
        }
    }

    void OnTriggerStay(Collider col)
    {
        //SituationA(1 enemy): if enemy[0] stay, shoot enemy[0]; if enemy[0] out/die, won't called
        //SituationB(2 ~s): if enemy[0] stay, shoot enemy[0]; if enemy[0] out/die, shoot current[0], which is enemy[1];
        if (enemyList[0] != null)
            transform.GetChild(1).LookAt(enemyList[0].transform.position);//cannon traces enemy[0](because OnTriggerStay called per frame)
        if (!activeShooting)
        {
            print("enemyStay: " + col.gameObject);
            InvokeRepeating("spawnBullet", 0, 1);
            activeShooting = true;
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
            CancelInvoke("spawnBullet");
            activeShooting = false;
        }
        enemyList.Remove(col.gameObject);//Must remove, then original 2nd one automatically becomes 1st one
        print("enemyCount after Exit: " + enemyList.Count);
        if (enemyList.Count != 0)
            print("enemyNewHead: " + enemyList[0]);

    }

    void Update()//if enemy[0] die, LET "spawnBullet" cancleInvoked, aS==false
    {
        while (enemyList.Count != 0 && enemyList[0] == null)//remove until enemy[0] != null
        {
            CancelInvoke("spawnBullet");
            activeShooting = false;
            enemyList.Remove(enemyList[0]);
            print("enemyCount after Die: " + enemyList.Count);
            if (enemyList.Count != 0)
                print("enemyNewHead: " + enemyList[0]);
        }
        ///*
        if(bulletList[0] != null)
        {
            transform.GetChild(5).Rotate(0, -80 * Time.deltaTime, 0);
            for (int i = 0; i < 4; i++)
            //transform.GetChild(4).Rotate(0, -80 * Time.deltaTime, 0);
            //for (int i = 0; i < 3; i++)
            {
                bulletList[0].transform.localScale = new Vector3(0.3f, 0.3f, 0.3f);
                //Rotation
                bulletList[i].transform.Rotate(0, -90 * Time.deltaTime, 0);
                
                //height hovering
                height_current[i] = bulletList[i].transform.position.y;
                height_error[i] = height_target[i] - height_current[i];
                height_integral[i] += height_error[i] * Time.deltaTime;
                height_derivative[i] = (height_error[i] - height_error_pre[i]) / Time.deltaTime;

                height_adjustment[i] = 1.0 * height_error[i] + 0.0 * height_integral[i] + 0.5 * height_derivative[i];//how to adjust?
                
                bulletList[i].GetComponent<Rigidbody>().AddForce(Vector3.up * (float)height_adjustment[i]);
                height_error_pre[i] = height_error[i];

                //bulletList[i].transform.RotateAround(transform.position, Vector3.up, -60 * Time.deltaTime);

                ////radius_out
                //radius_normal_out[i].x = bulletList[i].transform.position.x - transform.position.x;
                //radius_normal_out[i].y = bulletList[i].transform.position.z - transform.position.z;
                //bulletList[i].GetComponent<Rigidbody>().AddForce(radius_normal_out[i]);

                ////radius hovering
                //radius_normal_in[i].x = transform.position.x - bulletList[i].transform.position.x;
                //radius_normal_in[i].y = transform.position.z - bulletList[i].transform.position.z;
                //radius_current[i] = Mathf.Sqrt(Mathf.Pow(bulletList[i].transform.position.x, 2) + Mathf.Pow(bulletList[i].transform.position.z, 2));
                //radius_error[i] = radius_target[i] - radius_current[i];
                //radius_integral[i] += radius_error[i] * Time.deltaTime;
                //radius_derivative[i] = (radius_error[i] - radius_error_pre[i]) / Time.deltaTime;

                //radius_adjustment[i] = 100.0 * radius_error[i] + 0.0 * radius_integral[i] + 50.0 * radius_derivative[i];//how to adjust?
                //radius_adjustment[i] = Mathf.Min(Mathf.Max(0.0f, (float)radius_adjustment[i]), (float)radius_adjustment_max[i]);

                //bulletList[i].GetComponent<Rigidbody>().AddForce(radius_normal_in[i] * (float)radius_adjustment[i]);
                //radius_error_pre[i] = radius_error[i];
            }
        }
    }
}
