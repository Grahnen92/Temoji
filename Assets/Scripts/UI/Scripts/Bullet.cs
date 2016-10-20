using UnityEngine;
using System.Collections;

public class Bullet : MonoBehaviour {

	// Use this for initialization
	void Start () {
        GetComponent<Rigidbody>().velocity = transform.forward * 10;
	}
	
	// Update is called once per frame
	void Update () {
        Destroy(this.gameObject, 2);
	}
}
