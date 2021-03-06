﻿using UnityEngine;
using System.Collections;

public class NavigationRoll : MonoBehaviour
{

    private GameObject baseObject;

    private Vector3 target_destination;
    private Vector3 spawn_destination;

    public float speed_factor;
    private float attack_distance = 2.0f;
    const float MIN_VELOCITY = 0.01f;

    public GameObject explosion_prefab;

    NavMeshAgent nav;
    //GameObject roller;
    NavMeshPath navPath;
    Vector3 direction;
    Rigidbody rb;

    private float body_rot_error, body_rot_integral, 
        body_rot_derivative, body_rot_adjustment, prev_body_rot_error;


    void OnTriggerEnter(Collider col)
    {

    }

    void OnTriggerExit(Collider col)
    {
    }
    // Use this for initialization
    void Start()
    {

        //roller = transform.parent.GetChild(0).gameObject;
        nav = transform.parent.gameObject.GetComponent<NavMeshAgent>();

       
        transform.parent.gameObject.GetComponent<NavMeshAgent>().avoidancePriority = (int)Random.value * 100;
        transform.parent.gameObject.GetComponent<NavMeshAgent>().updatePosition = false;
        transform.parent.gameObject.GetComponent<NavMeshAgent>().updateRotation = false;

        navPath = new NavMeshPath();
        rb = GetComponent<Rigidbody>();

        explosion_prefab = Resources.Load("bot_explosion") as GameObject;
    }

    public void setBase(GameObject b, GameObject g)
    {
        baseObject = b;
        target_destination = baseObject.transform.position;
        spawn_destination = g.transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        transform.parent.gameObject.GetComponent<NavMeshAgent>().SetDestination(target_destination);
        float distance = (target_destination - transform.position).magnitude - 3.43f;
        transform.parent.gameObject.GetComponent<NavMeshAgent>().nextPosition = transform.position;

        //nav.CalculatePath(target_destination, navPath);
        //int i = 1;
        //while (i < navPath.corners.Length)
        //{
        //    if (Vector3.Distance(transform.position, navPath.corners[i]) > 0.5f)
        //    {
        //        direction = navPath.corners[i] - transform.position;
        //        break;
        //    }
        //    i++;
        //}
        direction = transform.parent.gameObject.GetComponent<NavMeshAgent>().steeringTarget - transform.position;
        direction.Normalize();

        if (!GameManager.base_alive)
        {
            transform.parent.gameObject.GetComponent<NavMeshAgent>().SetDestination(spawn_destination);

            if (distance <= attack_distance)
            {
                Destroy(gameObject); // They are home again
            }
        }

        // Distance from target

        if (distance <= attack_distance)
        {
            attack();
        }

        
    }

    void FixedUpdate()
    {

        Vector3 curr_forward = transform.forward;
        curr_forward.y = 0;
        curr_forward.Normalize();

        body_rot_error = -Vector3.Angle(direction, curr_forward);
        if (Vector3.Cross(direction, curr_forward).y < 0)
            body_rot_error = -body_rot_error;

        body_rot_integral = body_rot_integral + body_rot_error * Time.deltaTime;
        body_rot_derivative = (body_rot_error - prev_body_rot_error) / Time.deltaTime;

        body_rot_adjustment = 0.05f * body_rot_error + 0.0f * body_rot_integral + 0.01f * body_rot_derivative;
        prev_body_rot_error = body_rot_error;


        rb.AddRelativeTorque(Vector3.right * (float)body_rot_adjustment);

        //gameObject.GetComponent<Rigidbody>().AddForce(direction*100000.0f);

    }

    void attack()
    {

        Destroy(gameObject);
        // Attack target
        Base_Combat base_combat = baseObject.GetComponent<Base_Combat>();
       // base_combat.TakeDamage(10);
        GameObject explosion = Instantiate(explosion_prefab) as GameObject;
        explosion.transform.position = transform.position;
        explosion.transform.localScale = new Vector3(0.5f, 0.5f, 0.5f);


        GameObject tmpParent = transform.parent.gameObject;

        //outer child right
        GameObject tmpChildRight = tmpParent.transform.GetChild(1).GetChild(1).gameObject;
        tmpChildRight.transform.parent = null;
        tmpChildRight.AddComponent<SphereCollider>();
        tmpChildRight.AddComponent<Rigidbody>();
        tmpChildRight.layer = 22;
        tmpChildRight.AddComponent<DestroyTimer>().destructionTime = 4.0f;
        

        //outer child left
        GameObject tmpChildLeft = tmpParent.transform.GetChild(1).GetChild(0).gameObject;
        tmpChildLeft.transform.parent = null;
        tmpChildLeft.AddComponent<SphereCollider>();
        tmpChildLeft.AddComponent<Rigidbody>();
        tmpChildLeft.layer = 22;
        tmpChildLeft.AddComponent<DestroyTimer>().destructionTime = 4.0f;

        //outer child right
        tmpChildRight = tmpParent.transform.GetChild(0).GetChild(1).gameObject;
        tmpChildRight.transform.parent = null;
        tmpChildRight.AddComponent<SphereCollider>();
        tmpChildRight.AddComponent<Rigidbody>();
        tmpChildRight.layer = 22;
        tmpChildRight.AddComponent<DestroyTimer>().destructionTime = 4.0f;

        //outer child left
        tmpChildLeft = tmpParent.transform.GetChild(0).GetChild(0).gameObject;
        tmpChildLeft.transform.parent = null;
        tmpChildLeft.AddComponent<SphereCollider>();
        tmpChildLeft.AddComponent<Rigidbody>();
        tmpChildLeft.layer = 22;
        tmpChildLeft.AddComponent<DestroyTimer>().destructionTime = 4.0f;

    }

    void suicide()
    {
        Destroy(gameObject);
    }

    void stuck()
    {
        Destroy(gameObject);
    }
}
