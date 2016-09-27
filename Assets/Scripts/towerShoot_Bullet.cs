using UnityEngine;
using System.Collections;

public class towerShoot_Bullet : MonoBehaviour {
    public float bulletSpeed;
    public GameObject tower;
	// Use this for initialization
	void Start () {
        print("Tower shooting begins");
        Rigidbody rb = GetComponent<Rigidbody>();
        Vector3 direction = tower.transform.TransformVector(-1, 0, 0);
        print(direction);
        rb.AddForce(direction * bulletSpeed * 100);
    }
	
	// Update is called once per frame
	void Update () {
        if (transform.position.y < 0.2) {
            Destroy(this.gameObject);
        }
	}
}
