using UnityEngine;
using System.Collections;

public class Health : MonoBehaviour {

    private float health = 100.0f;
    private Rigidbody rb;
    private float last_velocity;

    // Use this for initialization
    void Start () {
        rb = GetComponent<Rigidbody>();
	}

    void setHealth(float f)
    {
        health = f;
    }

    void Update()
    {
        last_velocity = rb.velocity.sqrMagnitude;

        if (health <= 0.0f)
        {
            print("I died.");
            Destroy(gameObject);
        }
    }
}
