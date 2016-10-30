using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public class NavigationHover : MonoBehaviour
{

    static public GameObject baseObject;
    public GameObject bulletPrefab;
    List<GameObject> towerlist = new List<GameObject>();
    public GameObject attactTrigger;


    public static Vector3 target_destination;
    public static Vector3 spawn_destination;

    public float speed_factor;
//    const float MIN_VELOCITY = 0.01f;
    bool activeShooting = false;//Must have this flag, otherwise "spawnBullet" will be invoked many times, actually increasing frequency!!!
    GameObject enemy_hover_bullet;
    Vector3 En_Bu_position;
    public float bulletSpeed;
    int currentTower = 0;

    public GameObject explosion_prefab;

    NavMeshAgent nav;
    NavMeshPath navPath;
    Vector3 direction;
    Rigidbody rb;

    double height_target = 8.1d;//飘多高
    double height_current ;
    double height_error ;
    double height_error_pre = 0.0;
    double height_integral = 0.0;
    double height_derivative ;
    double height_adjustment ;


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

        

        explosion_prefab = Resources.Load("bot_explosion") as GameObject;
    }
        void enemyShoot(){
            Vector3 targetDirection;
            targetDirection = target_destination - En_Bu_position;
            enemy_hover_bullet.GetComponent<Rigidbody>().AddForce(targetDirection * bulletSpeed);

            print("----------------bullet shoot");
    }
        void spawnBullet()
    {
        Vector3 add_En_Bu_position = new Vector3(-0.3f, 0, 0.3f);
        En_Bu_position = gameObject.transform.position + add_En_Bu_position;
        Quaternion Bu_rotation = gameObject.transform.localRotation;
        enemy_hover_bullet = (GameObject)Instantiate(bulletPrefab, En_Bu_position, Bu_rotation);
        enemy_hover_bullet.SetActive(false);
    }

    void OnTriggerEnter(Collider col)
    {
        towerlist.Add(col.gameObject);
        if (currentTower < 0)
            currentTower = towerlist.Count - 1;


    }

    void OnTriggerExit(Collider col)
    {
        int tmpIndex = towerlist.IndexOf(col.gameObject);
        towerlist.RemoveAt(tmpIndex);
        if (tmpIndex == currentTower)
        {
            if (towerlist.Count > 0)
            {

                for (int i = 0; i < towerlist.Count; i++)
                {
                    if (towerlist[i] == null)
                    {
                        towerlist.RemoveAt(i);
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
    // Update is called once per frame
    void Update()
    {
        //float distance = (target_destination - transform.position).magnitude - 3.43f;
        GetComponent<NavMeshAgent>().nextPosition = transform.position;
        nav.SetDestination(target_destination);

        direction = GetComponent<NavMeshAgent>().steeringTarget - transform.position;
        direction.Normalize();

        GetComponent<Rigidbody>().AddForce(direction*1.0f);

        //gameObject.transform.LookAt(target_destination);

        
        print("basealive: " + GameManager.base_alive);
        if (!GameManager.base_alive)
        {
            GetComponent<NavMeshAgent>().SetDestination(spawn_destination);

            if (activeShooting==true)
            {
                Destroy(gameObject); // They are home again
            }
        }


        hover();
        // Distance from target

 /*       if (distance <= attack_distance)
        {
            attack();
        }
        */

    }

    

    void attack()
    {
        // Attack target
        print("Attack target!");

        Vector3 add_En_Bu_position = new Vector3 (-0.3f,0,0.3f);
        Vector3 En_Bu_position = gameObject.transform.position + add_En_Bu_position;
        Quaternion Bu_rotation = gameObject.transform.localRotation;
        GameObject enemy_hover_bullet = (GameObject)Instantiate(bulletPrefab, En_Bu_position, Bu_rotation);
        float bulletSpeed = 30.0f;

        Vector3 targetDirection;
        targetDirection = target_destination - En_Bu_position;
        enemy_hover_bullet.GetComponent<Rigidbody>().AddForce(targetDirection * bulletSpeed);

        print("----------------bullet shoot");

        //Destroy(gameObject);
        //Base_Combat base_combat = baseObject.GetComponent<Base_Combat>();
        // base_combat.TakeDamage(10);
        //GameObject explosion = Instantiate(explosion_prefab) as GameObject;
        //explosion.transform.position = transform.position;
        //explosion.transform.localScale = new Vector3(0.5f, 0.5f, 0.5f);



    }

    void spawnbullet()
    {

    }

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
