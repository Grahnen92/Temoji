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
    private bool base_alive;
    const float MIN_VELOCITY = 0.01f;

    private GameObject explosion_prefab;
    private GameObject explosion;

    // Use this for initialization
    void Start()
    {
        base_alive = true;
        GetComponent<NavMeshAgent>().SetDestination(target_destination);
        botVelocity = new Vector3(-.01f, 0, 0);
        GetComponent<NavMeshAgent>().speed = speed_factor*(Random.value + .5f);
        GetComponent<NavMeshAgent>().avoidancePriority = (int)Random.value * 100;
        GetComponent<NavMeshAgent>().autoBraking = false;
        GetComponent<NavMeshAgent>().autoRepath = true;
        GetComponent<NavMeshAgent>().baseOffset = 0.0f;
        GetComponent<NavMeshAgent>().stoppingDistance = -1.0f;

        explosion_prefab = Resources.Load("bot_explosion") as GameObject;
    }

    public static void setBase(GameObject b)
    {
        baseObject = b;
    }

    // Update is called once per frame
    void Update()
    {
        // Distance from target
        float distance = (target_destination - transform.position).magnitude;

        if (GetComponent<NavMeshAgent>().remainingDistance <= attack_distance)
        {
            attack();
        }


        // TODO - fix check to see when to destroy itself
        if(GetComponent<Rigidbody>().velocity.sqrMagnitude < MIN_VELOCITY* MIN_VELOCITY)
        {
            //suicide();
        }

        if (!GameManager.base_alive)
        {
            GetComponent<NavMeshAgent>().SetDestination(spawn_destination);

            if(GetComponent<NavMeshAgent>().remainingDistance <= attack_distance)
            {
                Destroy(gameObject); // They are home again
            }
        }


    }

    void attack()
    {
        // Attack target
        print("Attack target!");
        Destroy(gameObject);
        Base_Combat base_combat = baseObject.GetComponent<Base_Combat>();
       // base_combat.TakeDamage(10);
        explosion = Instantiate(explosion_prefab) as GameObject;
        explosion.transform.position = transform.position;
        explosion.transform.localScale = new Vector3(0.5f, 0.5f, 0.5f);


    }

    void suicide()
    {
        print("Commit Suicide!");
        Destroy(gameObject);
    }
}
