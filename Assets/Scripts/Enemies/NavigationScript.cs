using UnityEngine;
using System.Collections;

public class NavigationScript : MonoBehaviour
{

    static public GameObject baseObject;

    public static Vector3 target_destination;
    public static Vector3 spawn_destination;

    public float speed_factor;
    private float attack_distance = 2.0f;
    const float MIN_VELOCITY = 0.01f;

    NavMeshAgent nav;
    NavMeshPath navPath;
    Vector3 direction;
    Rigidbody rb;

    private float body_rot_error, body_rot_integral, 
        body_rot_derivative, body_rot_adjustment, prev_body_rot_error;

    // Use this for initialization
    void Start()
    {
        GetComponent<NavMeshAgent>().SetDestination(target_destination);
        GetComponent<NavMeshAgent>().avoidancePriority = (int)Random.value * 100;
        GetComponent<NavMeshAgent>().updatePosition = false;
        GetComponent<NavMeshAgent>().updateRotation = false;

        nav = GetComponent<NavMeshAgent>();
        navPath = new NavMeshPath();
        rb = GetComponent<Rigidbody>();

    }

    public static void setBase(GameObject b)
    {
        baseObject = b;
    }

    // Update is called once per frame
    void Update()
    {
        float distance = (target_destination - transform.position).magnitude;

        nav.CalculatePath(target_destination, navPath);
        int i = 1;
        while (i < navPath.corners.Length)
        {
            if (Vector3.Distance(transform.position, navPath.corners[i]) > 0.5f)
            {
                direction = navPath.corners[i] - transform.position;
                break;
            }
            i++;
        }

        if (!GameManager.base_alive)
        {
            GetComponent<NavMeshAgent>().SetDestination(spawn_destination);

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

        body_rot_adjustment = 0.017f * body_rot_error + 0.02f * body_rot_integral + 0.01f * body_rot_derivative;
        prev_body_rot_error = body_rot_error;


        rb.AddRelativeTorque(Vector3.right * (float)body_rot_adjustment);


        //gameObject.GetComponent<Rigidbody>().AddForce(direction*100000.0f);

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
