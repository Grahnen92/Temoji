using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System;

public class RingController : NetworkBehaviour {

    public GameObject bullet;
    public float displacement;
    public float rotateShoulders;

    public LayerMask mouse_mask = (1 << 1) | (1 << 10) | (1 << 13) | (1 << 14) | (1 << 18) | (1 << 21);

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

        transform.position = NavigationRoll.target_destination + new Vector3(1.5f, 1f, 0f);
    }


    GameObject shot;
    Vector2 randPt;

    // Update is called once per frame
    void Update () {
        if (!isLocalPlayer)
            return;

        if (Input.GetButton("Fire1"))
        {
            CmdFire();
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

    Vector3 curr_mouse_dir;
    Vector3 curr_mouse_dir_noy;
    Vector3 curr_mouse_hit;

    double prev_rot_error = 0.0;
    double rot_error;
    double rot_integral = 0.0;
    double rot_derivative;
    double rot_adjustment;

    double hight_error;
    double wanted_hight = 1.0;
    double hight_integral = 0.0;
    double hight_derivative;
    double previous_hight_error = 0.0;
    double hight_adjustment;
    double max_hight_adjustment = 1000.0;

    float curr_speed = 10f;
    Vector3 planar_velocity;

    void FixedUpdate()
    {
        if (!isLocalPlayer)
        {
            Debug.Log("Not local");
            return;
        }
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

        //head turning =====================================================================
        Vector3 curr_forward = transform.forward;
        curr_forward.y = 0;
        curr_forward.Normalize();

        rot_error = -Vector3.Angle(curr_mouse_dir_noy, curr_forward);
        if (Vector3.Cross(curr_mouse_dir_noy, curr_forward).y < 0)
            rot_error = -rot_error;

        rot_integral = rot_integral + rot_error * Time.deltaTime;
        rot_derivative = (rot_error - prev_rot_error) / Time.deltaTime;

        rot_adjustment = 2.025 * rot_error + 0.0 * rot_integral + 0.25 * rot_derivative;
        prev_rot_error = rot_error;
        head.AddRelativeTorque(Vector3.forward * (float)rot_adjustment);
        //head.AddRelativeTorque(Vector3.forward * 100);
        //Debug.Log(rot_adjustment);

        //transform.LookAt(curr_mouse_dir_noy + Vector3.up * 0.24f);

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
            Debug.Log(hight_adjustment);
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

        //var x = Input.GetAxis("Horizontal") * Time.deltaTime * 3.0f;
        //var z = Input.GetAxis("Vertical") * Time.deltaTime * 3.0f;

        //transform.Translate(x, 0, z);

        bodyRing1.transform.Rotate(moveV * 25 + moveH * 25, moveV * 50 + moveH * 50, moveV * 100 + moveH * 100);
        bodyRing2.transform.Rotate(moveV * 100 + moveH * 100, moveH * 25 + moveV * 25, moveV * 50 + moveH * 50);
    }

    [Command]
    void CmdFire()
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

        NetworkServer.Spawn(shot);
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
