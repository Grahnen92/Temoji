using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ProjectileTower : MonoBehaviour {

    //projectile
    private GameObject projectile_prefab;
    private GameObject current_projectile;
   // private GameObject[] loaded_projectiles;
   // private int projectile_nr = 3;
    private int loaded = 0;


    int currentEnemy = -1;
    private List<GameObject> enemies = new List<GameObject>();

    List<GameObject> bulletList = new List<GameObject>();
    private int maxNrOfBullets = 4;
    //height hovering
    private Vector3 TARGET_POSITION;
    Vector3[] pos_target;// = new Vector3[nrOfBullets] { 2.4, 1.5, 1, 0.6 };
    Vector3[] pos_current;// = new Vector3[4];
    Vector3[] pos_error;// = new Vector3[4];
    Vector3[] pos_error_pre;// = new Vector3[4] { 0.0, 0.0, 0.0, 0.0 };
    Vector3[] pos_integral;// = new Vector3[4] { 0.0, 0.0, 0.0, 0.0 };
    Vector3[] pos_derivative;// = new Vector3[4];
    Vector3[] pos_adjustment;// = new Vector3[4];
    //Vector3[][] pid_constants;
    Vector3[,] pid_constants;
     
    // Use this for initialization
    void Start () {

        pos_target = new Vector3[maxNrOfBullets];
        pos_current = new Vector3[maxNrOfBullets];
        pos_error = new Vector3[maxNrOfBullets];
        pos_error_pre = new Vector3[maxNrOfBullets];
        pos_integral = new Vector3[maxNrOfBullets];
        pos_derivative = new Vector3[maxNrOfBullets];
        pos_adjustment = new Vector3[maxNrOfBullets];

        Vector3 TARGET_POSITION = new Vector3(0.0f, 2.4f, 0.0f);
        pid_constants = new Vector3[maxNrOfBullets, 3];

        for (int i = 0; i < maxNrOfBullets; i++)
        {
            pos_target[i] = TARGET_POSITION + transform.position;
            pos_error_pre[i] = TARGET_POSITION - Vector3.up*0.25f;
            pos_integral[i] = Vector3.zero;

            //x
            pid_constants[i, 0] = new Vector3(5.0f, 0.0f, 1.0f);
            //height
            pid_constants[i, 1] = new Vector3(10.0f, 0.0f, 2.0f);
            //z
            pid_constants[i, 2] = new Vector3(5.0f, 0.0f, 1.0f);
        }
            
        //loaded_projectiles = n2Zew GameObject[projectile_nr];
        //initiating the projectile prefab of the character
        projectile_prefab = Resources.Load("tower_projectile") as GameObject;
        InvokeRepeating("spawnProjectile", 0, 2);
    }

    void OnTriggerEnter(Collider col)
    {
        enemies.Add(col.gameObject);
        if(currentEnemy < 0)
            currentEnemy = enemies.Count -1;


    }

    void OnTriggerExit(Collider col)
    {
        int tmpIndex = enemies.IndexOf(col.gameObject);
        enemies.RemoveAt(tmpIndex);
        if (tmpIndex == currentEnemy)
        {
            if(enemies.Count > 0)
            {
                
                for (int i = 0; i < enemies.Count; i++)
                {
                    if (enemies[i] == null)
                    {
                        enemies.RemoveAt(i);
                    }
                    else
                    {
                        currentEnemy = i;
                        break;
                    }
                }
            }
            else
            {
                currentEnemy = -1;
            }
        }
    }

    // Update is called once per frame
    void Update () {
        print(enemies.Count);
        if(bulletList[0] == null)
        {

        }
        //check if tower is loaded or if a new ball should be spawned
        //if (loaded == 0)
        if (false)
        {
            current_projectile = Instantiate(projectile_prefab);
            current_projectile.transform.position = transform.position + Vector3.up * 0.3f;
            loaded = 1;
            current_projectile.GetComponent<TowerProjectile>().setTowerBase(gameObject);
        }
       // else if(loaded == 2)
        else if (false)
        {
            //tower is loaded. Check if there is an enemy to shoot at
            if (currentEnemy != -1 && enemies[currentEnemy] != null)
            {
                current_projectile.GetComponent<TowerProjectile>().setHovering(false);
                Vector3 shootVec = (enemies[currentEnemy].transform.position + enemies[currentEnemy].GetComponent<Rigidbody>().velocity) - current_projectile.transform.position;
                current_projectile.GetComponent<Rigidbody>().AddForce(shootVec.normalized * 1000.0f);
                loaded = 0;
            }
            else
            {
                for (int i = 0; i < enemies.Count; i++)
                {
                    if (enemies[i] == null)
                    {
                        enemies.RemoveAt(i);
                    }
                    else
                    {
                        currentEnemy = i;
                        break;
                    }
                }
            }
           
        }

    }

    void FixedUpdate()
    {
        hoverBalls();
    }

    void hoverBalls()
    {
        for(int i = 0; i < bulletList.Count; i++)
        {
            pos_target[i].x  = transform.position.x + Mathf.Sin((Time.time) + i)*Mathf.Cos((Time.time) + i+ transform.position.x) *0.3f;
            pos_target[i].y = transform.position.y + Mathf.Sin((Time.time) + i)*Mathf.Sin((Time.time) + i + transform.position.x) * 0.3f + 2.4f;
            pos_target[i].z = transform.position.z + Mathf.Cos((Time.time) + i + transform.position.x) * 0.3f;
            //pos_error[i] = pos_target[i] - (bulletList[i].transform.position - transform.position);
            pos_error[i] = pos_target[i] - bulletList[i].transform.position;
            pos_integral[i] = pos_integral[i] + pos_error[i] * Time.fixedDeltaTime;
            pos_derivative[i] = (pos_error[i] - pos_error_pre[i]) / Time.fixedDeltaTime;

            Vector3 tmpVec = new Vector3(pos_error[i].x, pos_integral[i].x, pos_derivative[i].x);
            pos_adjustment[i].x = Vector3.Dot(tmpVec, pid_constants[i, 0]);
            tmpVec = new Vector3(pos_error[i].y, pos_integral[i].y, pos_derivative[i].y);
            pos_adjustment[i].y = Vector3.Dot(tmpVec, pid_constants[i, 1]);
            tmpVec = new Vector3(pos_error[i].z, pos_integral[i].z, pos_derivative[i].z);
            pos_adjustment[i].z = Vector3.Dot(tmpVec, pid_constants[i, 2]);

            //hight_adjustment = Math.Min(Math.Max(0.0, hight_adjustment), max_hight_adjustment);
            pos_error_pre[i] = pos_error[i];
            bulletList[i].GetComponent<Rigidbody>().AddForce(pos_adjustment[i]);
        }
    }

    void spawnProjectile()
    {
        GameObject tmpBullet = Instantiate(projectile_prefab, transform.position, transform.localRotation) as GameObject;
        tmpBullet.GetComponent<Rigidbody>().AddForce(Vector3.right * 10);
        bulletList.Add(tmpBullet);//create an empty GameObject as a child of cannon
        if (bulletList.Count == maxNrOfBullets)
            CancelInvoke("spawnProjectile");
    }

    void shoot()
    {
        //bulletList
    }

    public void setLoaded(int load_state)
    {
        loaded = load_state;
    }
}
