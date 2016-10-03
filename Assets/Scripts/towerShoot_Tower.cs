using UnityEngine;
using System.Collections;

public class towerShoot_Tower : MonoBehaviour {
    public GameObject bullet;//bulletPrefab
    public GameObject enemy;//enemy in scene
    public Transform bulletSpawn;//bulletSpawn with Prefab
    public float towerRange;
    bool isShooting = false;

    // Use this for initialization
    void Start() {
    }
     
    // Update is called once per frame
    void spawnBullet()
    {
        Instantiate(bullet, bulletSpawn.position, bulletSpawn.localRotation);//create an empty GameObject as child of cannon, positioned at the face of cannon
    }

    void start_spawnBullet()
    {
        isShooting = true;
        InvokeRepeating("spawnBullet", 0, 0.5f);//shoot at intervals of how many seconds
    }

    void Update() {
        Vector3 enemyPosition = enemy.transform.position;
        Vector3 towerPosition = transform.position;
        Vector3 towardsEnemy = enemyPosition - towerPosition;
        float distance = Mathf.Sqrt(Mathf.Pow(enemyPosition.x - towerPosition.x, 2) + Mathf.Pow(enemyPosition.z - towerPosition.z, 2));
        print("Distance between tower and enemy is：" + distance);
        if (distance < towerRange){
            transform.GetChild(0).LookAt(enemyPosition);//notice the face of cannon when modeling
            if (!isShooting) start_spawnBullet();
        }
        else
        {
            CancelInvoke();
        }
    }
}
