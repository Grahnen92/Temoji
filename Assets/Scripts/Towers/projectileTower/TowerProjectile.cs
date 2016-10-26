using UnityEngine;
using System.Collections;
using System;


public class TowerProjectile : MonoBehaviour {

    private GameObject tower_base;

    private Rigidbody rb;
    private bool hovering = true;

    //hovering variables
    private double previous_hight_error = 0.0;
    private double hight_error;
    private double hight_integral = 0.0;
    private double hight_derivative;
    private double hight_adjustment;
    //private const double max_hight_adjustment = 1000.0;
    private double wanted_hight = 2.4;

    //hovering variables
    private double previous_x_error = 0.0;
    private double x_error;
    private double x_integral = 0.0;
    private double x_derivative;
    private double x_adjustment;
    //private const double max_hight_adjustment = 1000.0;
    private double wanted_x = 0.0;

    //hovering variables
    private double previous_y_error = 0.0;
    private double y_error;
    private double y_integral = 0.0;
    private double y_derivative;
    private double y_adjustment;
    //private const double max_hight_adjustment = 1000.0;
    private double wanted_y = 0.0;

    private GameObject explosion_prefab;
    private GameObject explosion;


    // Use this for initialization
    void Start () {
        rb = GetComponent<Rigidbody>();
        previous_hight_error = wanted_hight - 0.25;
        explosion_prefab = Resources.Load("Energy_explosion") as GameObject;
    }

    void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject != tower_base)
        {
            explosion = Instantiate(explosion_prefab) as GameObject;
            explosion.transform.position = transform.position;
            explosion.transform.localScale = new Vector3(0.5f, 0.5f, 0.5f);
            GameObject hit = collision.gameObject;

            Combat hit_health = hit.GetComponent<Combat>();

            if (hit_health)
            {
                hit_health.TakeDamage(10);
            }

            if (hovering)
                tower_base.GetComponent<ProjectileTower>().setLoaded(0);

            Destroy(gameObject);
        }
        
    }

    // Update is called once per frame
    void Update () {

       
    }

    void FixedUpdate()
    {
        if (hovering)
        {
            wanted_y = Mathf.Sin(Time.time*3.0f)*0.15;
            wanted_x = Mathf.Cos(Time.time * 3.0f) * 0.1;

            hight_error = wanted_hight - (transform.position.y - tower_base.transform.position.y);
            hight_integral = hight_integral + hight_error * Time.fixedDeltaTime;
            hight_derivative = (hight_error - previous_hight_error) / Time.fixedDeltaTime;
            hight_adjustment = 4.0 * hight_error + 2.0 * hight_integral + 2.0 * hight_derivative;
            //hight_adjustment = Math.Min(Math.Max(0.0, hight_adjustment), max_hight_adjustment);
            previous_hight_error = hight_error;

            rb.AddForce(Vector3.up * (float)hight_adjustment);
            if (Math.Abs(hight_error) < 0.1)
            {
                tower_base.GetComponent<ProjectileTower>().setLoaded(2);
            }

            x_error = wanted_x - (transform.position.x - tower_base.transform.position.x);
            x_integral = x_integral + x_error * Time.fixedDeltaTime;
            x_derivative = (x_error - previous_x_error) / Time.fixedDeltaTime;
            x_adjustment = 10.0 * x_error + 0.0 * x_integral + 1.0 * x_derivative;
            //x_adjustment = Math.Min(Math.Max(0.0, x_adjustment), max_x_adjustment);
            previous_x_error = x_error;

            rb.AddForce(Vector3.right * (float)x_adjustment);

            y_error = wanted_y - (transform.position.z - tower_base.transform.position.z);
            y_integral = y_integral + y_error * Time.fixedDeltaTime;
            y_derivative = (y_error - previous_y_error) / Time.fixedDeltaTime;
            y_adjustment = 10.0 * y_error + 0.0 * y_integral + 1.0 * y_derivative;
            //y_adjustment = Math.Min(Math.Max(0.0, y_adjustment), max_y_adjustment);
            previous_y_error = y_error;

            rb.AddForce(Vector3.forward * (float)y_adjustment);

        }
    }

    public void setTowerBase(GameObject _tower_base)
    {
        tower_base = _tower_base;
    }
    public void setHovering(bool _should_hover)
    {
        hovering = _should_hover;
    }
}
