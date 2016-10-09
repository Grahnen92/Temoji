using UnityEngine;
using System.Collections;

public class towerShoot_Bullet : MonoBehaviour {
    public GameObject tower;//towerPrefab
    float bulletSpeed = 100;

    // Use this for initialization
    void Start () {
        print("Tower shooting begins");//
        Rigidbody rb = GetComponent<Rigidbody>();
        Vector3 direction = tower.transform.GetChild(0).TransformVector(-1, 0, 0);//cannon direction
        print(direction);
        rb.AddForce(direction * bulletSpeed * 100);
    }

    void OnCollisionEnter(Collision coll)
    {
        //if (coll.gameObject.tag == "enemy" || coll.gameObject.name == "Ground Quad")
        if (coll.gameObject.tag == "enemy")//how to avoid bouncing back???
            Destroy(this.gameObject);
    }
    
    // Update is called once per frame
    //void Update () {
    //    if (transform.position.y < 0.2)
    //    {
    //        Destroy(this.gameObject);
    //    }
    //}
}
