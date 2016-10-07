using UnityEngine;
using System.Collections;

public class explosion_destroy_timer : MonoBehaviour
{

    // Use this for initialization
    private float timer;
    void Start()
    {
        timer = 0.0f;
    }

    // Update is called once per frame
    void Update()
    {
        timer += Time.deltaTime;

        if (timer > 2.5f)
        {
            Destroy(gameObject);
        }

    }
}