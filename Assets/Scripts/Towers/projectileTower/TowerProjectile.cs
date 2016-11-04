using UnityEngine;
using System.Collections;
using System;


public class TowerProjectile : MonoBehaviour {

    private GameObject tower_base;

    private Rigidbody rb;
    private bool hovering = true;

    private GameObject explosion_prefab;
    private GameObject explosion;
    private bool armed = false;


    // Use this for initialization
    void Start () {
        
        rb = GetComponent<Rigidbody>();
        explosion_prefab = Resources.Load("Energy_explosion") as GameObject;
    }

    void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject != tower_base && armed)
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

            Destroy(gameObject);
        }
        
    }

    // Update is called once per frame
    void Update () {
       
    }

    public void setTowerBase(GameObject _tower_base)
    {
        tower_base = _tower_base;
    }
    public void setHovering(bool _should_hover)
    {
        hovering = _should_hover;
    }
    public void setArmed(bool _armed)
    {
        armed = _armed;
        gameObject.layer = 20;
    }
}
