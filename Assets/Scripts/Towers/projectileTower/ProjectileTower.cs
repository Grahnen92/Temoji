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
    

    // Use this for initialization
    void Start () {
        //loaded_projectiles = new GameObject[projectile_nr];
        //initiating the projectile prefab of the character
        projectile_prefab = Resources.Load("tower_projectile") as GameObject;
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
        //check if tower is loaded or if a new ball should be spawned
        if (loaded == 0)
        {
            current_projectile = Instantiate(projectile_prefab);
            current_projectile.transform.position = transform.position + Vector3.up * 0.3f;
            loaded = 1;
            current_projectile.GetComponent<TowerProjectile>().setTowerBase(gameObject);
        }
        else if(loaded == 2)
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

    public void setLoaded(int load_state)
    {
        loaded = load_state;
    }
}
