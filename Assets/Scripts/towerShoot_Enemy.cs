using UnityEngine;
using System.Collections;

public class towerShoot_Enemy : MonoBehaviour {
    public float enemySpeed;
    // Use this for initialization
    void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
        if (transform.position.x < -1) {
            transform.Translate(transform.TransformVector(1, 0, 0) * enemySpeed * Time.deltaTime);
        }
        
	}
}
