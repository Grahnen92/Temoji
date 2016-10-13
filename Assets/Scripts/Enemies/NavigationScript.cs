using UnityEngine;
using System.Collections;

public class NavigationScript : MonoBehaviour
{

    public static Vector3 target_destination;
    public static Vector3 spawn_destination;
    public GameObject bot;
    private Vector3 botVelocity;
    public float attack_distance = 1.0f;

    // Use this for initialization
    void Start()
    {
        botVelocity = new Vector3(-.01f, 0, 0);
        GetComponent<NavMeshAgent>().speed = (Random.value + .5f);
        GetComponent<NavMeshAgent>().avoidancePriority = (int)Random.value * 100;
    }

    // Update is called once per frame
    void Update()
    {


        GetComponent<NavMeshAgent>().SetDestination(target_destination);
        // Distance from target
        float distance = (target_destination - transform.position).magnitude;
        if (distance < attack_distance)
        {
            // Attack target
            print("Attack target!");
            Destroy(bot);
            GetComponent<NavMeshAgent>().speed = 0;
        }
    }
}
