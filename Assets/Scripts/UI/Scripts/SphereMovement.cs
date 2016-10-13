using UnityEngine;
using System.Collections;

public class SphereMovement : MonoBehaviour
{

    // Use this for initialization
    void Start()
    {

    }

    Vector3 direction = Vector3.left * 0.3f;
    float time = 0f;

    // Update is called once per frame
    void Update()
    {
        transform.Translate(direction);
        time += Time.deltaTime;
        if (time > 1.0f)
        {
            time = 0;
            direction = -direction;
        }
    }
}
