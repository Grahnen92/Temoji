using UnityEngine;
using System.Collections;

public class towerShootInput : MonoBehaviour
{
    // Use this for initialization
    void Start()
    {

    }
    public Transform bullet;
    // Update is called once per frame
    void Update()
    {
        if (Input.GetButtonDown("Fire1"))
        {
            Instantiate(bullet, transform.position, transform.localRotation);
        }
    }
}
