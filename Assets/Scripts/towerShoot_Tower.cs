using UnityEngine;
using System.Collections;

public class towerShoot_Tower : MonoBehaviour {
    // Use this for initialization
    void Start() {

    }
    public GameObject bullet;
    public GameObject enemy;
    public float towerRange;
    // Update is called once per frame
    void Update() {
        //        if (Input.GetButtonDown("Fire1"))
        //        {
        //            Instantiate(bullet, transform.position, transform.localRotation);
        //        }
        Vector3 enemyPosition = enemy.transform.position;
        Vector3 towerPosition = transform.position;
        Vector3 towardsEnemy = enemyPosition - towerPosition;
        float distance = Mathf.Sqrt(Mathf.Pow(enemyPosition.x - towerPosition.x, 2) + Mathf.Pow(enemyPosition.z - towerPosition.z, 2));
//        print("Distance between tower and enemy is：" + distance);
        if (distance < towerRange){
            transform.LookAt(enemyPosition);
            if (Input.GetButtonDown("Fire1"))
                {
                    Instantiate(bullet, transform.position, transform.localRotation);
                }
        }
    }
}
