using UnityEngine;
using System.Collections;
using System;

public class RingController : MonoBehaviour {

    public GameObject bullet;
    public float displacement;
    public float rotateShoulders;

    GameObject backOuterRing;
    GameObject backInnerRing;
    GameObject rightRing;
    GameObject rightShoulder;
    GameObject leftShoulder;
    GameObject bodyRing1;
    GameObject bodyRing2;

    Rigidbody head;

    float bulletRadius;
    bool buildMode = false;
    Quaternion originalRight;
    Quaternion originalLeft;

    // Use this for initialization
    void Start () {
        backOuterRing = transform.Find("Outer Plate Ring").gameObject;
        backInnerRing = transform.Find("Inner Plate Ring").gameObject;
        rightShoulder = transform.Find("Right Shoulder").gameObject;
        leftShoulder = transform.Find("Left Shoulder").gameObject;
        bulletRadius = backInnerRing.GetComponent<MakePlateRings>().radius * 0.9f;
        bodyRing1 = transform.Find("Body").Find("Ball Rings").Find("Body Ring 1").gameObject;
        bodyRing2 = transform.Find("Body").Find("Ball Rings").Find("Body Ring 2").gameObject;
        originalRight = rightShoulder.transform.localRotation;
        originalLeft = leftShoulder.transform.localRotation;
        head = GetComponent<Rigidbody>();
    }


    GameObject shot;
    Vector2 randPt;

    // Update is called once per frame
    void Update () {
        if (Input.GetButton("Fire1"))
        {
            fireCmd();
        }
        else
        {
            // Resume normal rotation speed
            backOuterRing.GetComponent<MakePlateRings>().rotationSpeed = 1;
            backInnerRing.GetComponent<MakePlateRings>().rotationSpeed = 1;
        }

        if (Input.GetKeyDown(KeyCode.F))
        {
            buildToggle();

        }

        // Movement
        //var x = Input.GetAxis("Horizontal") * Time.deltaTime * 150.0f;
        //var z = Input.GetAxis("Vertical") * Time.deltaTime * 3.0f;

        //transform.Rotate(0, x, 0);
        //transform.Translate(0, 0, z);

        //bodyRing1.transform.Rotate(z*50, z*100, z*200);
        //bodyRing2.transform.Rotate(z*200, z*50, z*100);

        animate();
    }


    double hight_error;
    double wanted_hight = 2.4;
    double hight_integral = 0.0;
    double hight_derivative;
    double previous_hight_error = 0.0;
    double hight_adjustment = 1000.0;
    double max_hight_adjustment;

    float curr_speed = 10f;
    Vector3 planar_velocity;

    void FixedUpdate()
    {
        // === Turn functions =====================================================================
        //mouse ray tracing =====================================================================
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit mouse_hit;
        Physics.Raycast(ray, out mouse_hit, 100, mouse_mask);
        curr_mouse_hit = mouse_hit.point;
        curr_mouse_dir = mouse_hit.point - transform.position;
        curr_mouse_dir_noy = curr_mouse_dir;
        curr_mouse_dir_noy.y = 0;
        curr_mouse_dir_noy.Normalize();

        //Hovering ============================================================================
        RaycastHit hit;
        if (Physics.Raycast(transform.position, Vector3.down, out hit, 100.0f, 1))
        {
            hight_error = wanted_hight - hit.distance;
            hight_integral = hight_integral + hight_error * Time.deltaTime;
            hight_derivative = (hight_error - previous_hight_error) / Time.deltaTime;

            hight_adjustment = 100.0 * hight_error + 0.0 * hight_integral + 50.0 * hight_derivative;
            hight_adjustment = Math.Min(Math.Max(0.0, hight_adjustment), max_hight_adjustment);

            previous_hight_error = hight_error;

            head.AddForce(Vector3.up * (float)hight_adjustment);

        }
        else {
            previous_hight_error = 0.0;
            hight_integral = 0.0;
        }

        //Movement ======================================================================
        planar_velocity = head.velocity; planar_velocity.y = 0.0f;

        float moveH = Input.GetAxis("Horizontal");
        float moveV = Input.GetAxis("Vertical");


        Vector3 wanted_velocity = new Vector3(moveH, 0, moveV).normalized * curr_speed;
        Vector3 velocity_diff = wanted_velocity - planar_velocity;

        float diff_magnitude = velocity_diff.magnitude;
        velocity_diff.Normalize();

        if (planar_velocity.magnitude < curr_speed)
        {
            head.AddForce(velocity_diff * diff_magnitude * diff_magnitude * Time.deltaTime * 60);
        }
    }

    void fireCmd()
    {
        randPt = UnityEngine.Random.insideUnitCircle;
        // Speed up rings
        backOuterRing.GetComponent<MakePlateRings>().rotationSpeed = 3;
        backInnerRing.GetComponent<MakePlateRings>().rotationSpeed = 3;
        // Shoot bullet at random position inside back inner ring
        shot = Instantiate(bullet);
        shot.transform.position = backInnerRing.transform.position + new Vector3(randPt.x, randPt.y, 0) * bulletRadius;
        shot.transform.forward = backInnerRing.transform.forward;
        shot.transform.LookAt(transform.Find("Target"));
    }

    float time = 0f;
    float sin;

    float handTime = 0f;
    float handSin;

    void animate()
    {
        if (time > Mathf.PI*2)
            time = 0f;
        else
            time += Time.deltaTime;

        if (handTime > Mathf.PI * 2)
            handTime = 0f;
        else
            handTime += Time.deltaTime;

        sin = Mathf.Sin(time)*displacement;
        handSin = Mathf.Sin(handTime) * rotateShoulders;

        transform.Translate(0, sin, 0);
        if (!buildMode)
        {
            //rightShoulder.transform.Rotate(handSin, handSin, 0);
            //leftShoulder.transform.Rotate(-handSin, -handSin, 0);
            rightShoulder.transform.Rotate(0, 0, handSin);
            leftShoulder.transform.Rotate(0, 0, -handSin);
        }

        bodyRing1.transform.Rotate(0, 1f, 1f);
        bodyRing2.transform.Rotate(1f, 0, 1f);
    }

    void buildToggle() {
        if (!buildMode)
        {
            buildMode = true;
            rightShoulder.transform.localRotation = originalRight;
            rightShoulder.transform.Rotate(-30, -30, 0);
            leftShoulder.transform.localRotation = originalLeft;
            leftShoulder.transform.Rotate(-30, 30, 0);
        }
        else
        {
            buildMode = false;
            rightShoulder.transform.localRotation = originalRight;
            leftShoulder.transform.localRotation = originalLeft;
            handTime = 0f;
        }
    }
}
