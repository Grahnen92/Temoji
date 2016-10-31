﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public class NavigationHover : MonoBehaviour
{
    public GameObject bullet_prefab;
    private int currentTower = -1;
    private List<GameObject> towerList = new List<GameObject>();
    private bool activeReloading = false;
    private float reloadTime = 2.0f;
    private GameObject enemy_hover_bullet;
    private Vector3 En_Bu_position;
    public float bulletSpeed;

    double height_target = 8.1d;//飘多高
    double height_current;
    double height_error;
    double height_error_pre = 0.0;
    double height_integral = 0.0;
    double height_derivative;
    double height_adjustment;

    public GameObject explosion_prefab;
    public static GameObject baseObject;
    public float speed_factor;
    public static Vector3 target_destination;
    public static Vector3 spawn_destination;
    NavMeshAgent nav;
    NavMeshPath navPath;
    Vector3 direction;
    Rigidbody rb;
    private float homedistance=2.0f;


    // Use this for initialization
    void Start()
    {
        print("target_destination in naHo" + target_destination);
        GetComponent<NavMeshAgent>().SetDestination(target_destination);
        //        GetComponent<NavMeshAgent>().avoidancePriority = (int)Random.value * 100;
        GetComponent<NavMeshAgent>().updatePosition = false;
        GetComponent<NavMeshAgent>().updateRotation = false;

        nav = GetComponent<NavMeshAgent>();
        navPath = new NavMeshPath();
        rb = GetComponent<Rigidbody>();


      //  bullet_prefab = Resources.Load("enemy_hover_bullet") as GameObject;

        explosion_prefab = Resources.Load("bot_explosion") as GameObject;
    }

    void OnTriggerEnter(Collider col)
    {
        print("there is a trigger happened");
        towerList.Add(col.gameObject);
        if (currentTower < 0)
            currentTower = towerList.Count - 1;
        if (!activeReloading)
        {
            InvokeRepeating("spawnBullet", 0, reloadTime);
            activeReloading = true;
        }
    }

    void OnTriggerExit(Collider col)
    {
        print("exit the collision ");

        int tmpIndex = towerList.IndexOf(col.gameObject);
        towerList.RemoveAt(tmpIndex);
        if (tmpIndex == currentTower)
        {
            if (towerList.Count > 0)
            {

                for (int i = 0; i < towerList.Count; i++)
                {
                    if (towerList[i] == null)
                    {
                        towerList.RemoveAt(i);
                    }
                    else
                    {
                        currentTower = i;
                        break;
                    }
                }
            }
            else
            {
                currentTower = -1;
            }
        }
    }

    void Update()
    {
        // navmesh working
        float distance = (target_destination - transform.position).magnitude - 3.43f;
        GetComponent<NavMeshAgent>().nextPosition = transform.position;
        nav.SetDestination(target_destination);

        direction = GetComponent<NavMeshAgent>().steeringTarget - transform.position;
        direction.Normalize();

        GetComponent<Rigidbody>().AddForce(direction * 1.0f);

        //gameObject.transform.LookAt(target_destination);


        print("basealive: " + GameManager.base_alive);
        if (!GameManager.base_alive)
        {
            GetComponent<NavMeshAgent>().SetDestination(spawn_destination);

            if (distance <= homedistance)
            {
                Destroy(gameObject); // They are home again
            }
        }

        hover();

        //check if there is an enemy to attack
        if (currentTower != -1)
        {
            //check if the current enemy has died
            if (towerList[currentTower] != null)
            {
                print("there is a tower should be attacted");
                enemyShoot();
            }
            else //current enemy is dead, search enemy list for a new target and enemies from the list if they are dead
            {
                towerList.RemoveAt(currentTower);
                currentTower = -1;
                CancelInvoke("spawnBullet");
                activeReloading = false;
                for (int i = 0; i < towerList.Count; i++)
                {
                    if (towerList[i] != null)
                    {
                        currentTower = i;
                        InvokeRepeating("spawnBullet", 0, reloadTime);
                        activeReloading = true;
                    }
                    else
                        towerList.RemoveAt(i);
                }
            }
        }
        else
        {
            CancelInvoke("spawnBullet");
            activeReloading = false;
        }

    }
    void enemyShoot(){
        Vector3 targetDirection;
        targetDirection = towerList[0].transform.position - En_Bu_position;
        enemy_hover_bullet.GetComponent<Rigidbody>().AddForce(targetDirection * bulletSpeed);
        print("----------------bullet shoot");
        if (!activeReloading)
        {
            InvokeRepeating("spawnBullet", reloadTime, reloadTime);
            activeReloading = true;
        }
    }
    void spawnBullet()
    {
        Vector3 add_En_Bu_position = new Vector3(-0.3f, 0, 0.3f);
        En_Bu_position = gameObject.transform.position + add_En_Bu_position;
        Quaternion Bu_rotation = gameObject.transform.localRotation;
        enemy_hover_bullet = (GameObject)Instantiate(bullet_prefab, En_Bu_position, Bu_rotation);
    }
    
    public static void setBase(GameObject b)
    {
        baseObject = b;
    }
    // Update is called once per frame

    void hover()
    {
        height_current  = gameObject.transform.position.y;//enemy height
        height_error  = height_target - height_current;// buyongdong
        height_integral  += height_error * Time.deltaTime;//buyongdong
        height_derivative  = (height_error - height_error_pre) / Time.deltaTime;//buyongdong

        height_adjustment  = 2.0 * height_error + 0.0 * height_integral + 0.5 * height_derivative;//how to adjust?//调权重

        gameObject .GetComponent<Rigidbody>().AddForce(Vector3.up * (float)height_adjustment);//add force to enemy to let it hover
        height_error_pre  = height_error;
    }
}