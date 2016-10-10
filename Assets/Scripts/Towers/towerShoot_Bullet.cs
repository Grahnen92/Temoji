using UnityEngine;
using System.Collections;

public class towerShoot_Bullet : MonoBehaviour {
    public GameObject tower;//towerPrefab
    float bulletSpeed = 100;

    // Use this for initialization
    void Start ()
    {
        Rigidbody rb = GetComponent<Rigidbody>();
        Vector3 direction = tower.transform.GetChild(0).TransformVector(-1, 0, 0);//cannon direction
        print(direction);
        rb.AddForce(direction * bulletSpeed * 100);
    }

    //Update is called once per frame
    void Update()
    {
        if (transform.position.y < 0.5 || transform.position.y > 1)
            Destroy(this.gameObject);
    }
}
