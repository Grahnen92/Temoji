using UnityEngine;
using System.Collections;

public class towerShoot_Enemy : MonoBehaviour {
    float enemySpeed = 0.5f;
    
    // Use this for initialization
    void Start () {
	}

    void OnTriggerEnter(Collider coll)//use OnTrigger to avoid bouncing back
    {
        if (coll.gameObject.tag == "bullet")
            Destroy(coll.gameObject);
    }

    // Update is called once per frame
    void Update ()
    {
        if (transform.position.x < 10)
        {
            transform.Translate(transform.TransformVector(1, 0, 0) * enemySpeed * Time.deltaTime);
        }
	}

}
