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
    private List<GameObject> enemyList = new List<GameObject>();

    List<GameObject> bulletList = new List<GameObject>();
    private int maxNrOfBullets = 4;
    private bool activeReloading = false;
    private float reloadTime = 1.5f;
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
        InvokeRepeating("spawnProjectile", 0, reloadTime);
        activeReloading = true;
    }

    void OnTriggerEnter(Collider col)
    {
        enemyList.Add(col.gameObject);
        if(currentEnemy < 0)
            currentEnemy = enemyList.Count -1;


    }

    void OnTriggerExit(Collider col)
    {
        int tmpIndex = enemyList.IndexOf(col.gameObject);
        enemyList.RemoveAt(tmpIndex);
        if (tmpIndex == currentEnemy)
        {
            if(enemyList.Count > 0)
            {
                
                for (int i = 0; i < enemyList.Count; i++)
                {
                    if (enemyList[i] == null)
                    {
                        enemyList.RemoveAt(i);
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

        //check if there is an enemy to attack
        if(currentEnemy != -1)
        {
            //check if the current enemy has died
            if(enemyList[currentEnemy] != null)
            {
                //check if there are bullets to shoot
                if (bulletList.Count > 0 && bulletList[0].transform.position.y > transform.position.y + TARGET_POSITION.y*0.75f )
                {
                    shootProjectile();
                }
            }
            else //current enemy is dead, search enemy list for a new target and enemies from the list if they are dead
            {
                enemyList.RemoveAt(currentEnemy);
                currentEnemy = -1;
                for(int i = 0; i < enemyList.Count; i++)
                {
                    if (enemyList[i] != null)
                        currentEnemy = i;
                    else
                        enemyList.RemoveAt(i);
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
        {
            CancelInvoke("spawnProjectile");
            activeReloading = false;
        }
           
    }

    void shootProjectile()
    {
        Vector3 shootVec = enemyList[currentEnemy].transform.position - bulletList[0].transform.position + enemyList[currentEnemy].GetComponent<Rigidbody>().velocity;
        bulletList[0].GetComponent<Rigidbody>().AddForce(shootVec * 100.0f);
        bulletList[0].GetComponent<TowerProjectile>().setArmed(true);
        bulletList.RemoveAt(0);
        if (!activeReloading)
        {
            InvokeRepeating("spawnProjectile", reloadTime, reloadTime);
            activeReloading = true;
        }
    }

    public void setLoaded(int load_state)
    {
        loaded = load_state;
    }
}
