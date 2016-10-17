using UnityEngine;
using System.Collections;

public class damaging_Enemy : MonoBehaviour {
    float enemySpeed = 0.5f;
    
    void Start () {	}

    void Update ()
    {
        if (transform.position.x < 10)
        {
            transform.Translate(transform.TransformVector(1, 0, 0) * enemySpeed * Time.deltaTime);
            ////test die
            //if (transform.position.x > -3)
            //    Destroy(gameObject);
        }
	}

}
