using UnityEngine;
using System.Collections;

public class NavigationScript : MonoBehaviour
{

    public static Vector3 target_destination;
    public static Vector3 spawn_destination;
    public float speed_factor;
    static public GameObject baseObject;
    private Vector3 botVelocity;
    private float attack_distance = 1.0f;
    const float MIN_VELOCITY = 0.01f;

    // Use this for initialization
    void Start()
    {
        botVelocity = new Vector3(-.01f, 0, 0);
        GetComponent<NavMeshAgent>().speed = speed_factor*(Random.value + .5f);
        GetComponent<NavMeshAgent>().avoidancePriority = (int)Random.value * 100;
    }

    public static void setBase(GameObject b)
    {
        baseObject = b;
    }

    // Update is called once per frame
    void Update()
    {
        GetComponent<NavMeshAgent>().SetDestination(target_destination);
        // Distance from target
        float distance = (target_destination - transform.position).magnitude;

        if (distance < attack_distance)
        {
            attack();
        }


        // TODO - fix check to see when to destroy itself
        if(GetComponent<Rigidbody>().velocity.sqrMagnitude < MIN_VELOCITY* MIN_VELOCITY)
        {
            //suicide();
        }
    }

    void attack()
    {
        // Attack target
        print("Attack target!");
        Destroy(gameObject);
        Base_Combat base_combat = baseObject.GetComponent<Base_Combat>();
        base_combat.TakeDamage(10);
    }

    void suicide()
    {
        print("Commit Suicide!");
        Destroy(gameObject);
    }
}
