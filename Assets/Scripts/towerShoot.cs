using UnityEngine;
using System.Collections;

public class towerShoot : MonoBehaviour {
    public float speed = 5.0f;
    public GameObject player;
	// Use this for initialization
	void Start () {
        print("Tower shooting begins");
        Rigidbody rb = GetComponent<Rigidbody>();
        Vector3 direction = player.transform.TransformVector(-1, 0, 0);
        rb.AddForce(direction * speed * 100);
    }
	
	// Update is called once per frame
	void Update () {
        if (transform.position.y<-5) {
            Destroy(this.gameObject);
        }
	}
}
