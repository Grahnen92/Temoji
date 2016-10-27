using UnityEngine;
using System.Collections;

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
        var x = Input.GetAxis("Horizontal") * Time.deltaTime * 150.0f;
        var z = Input.GetAxis("Vertical") * Time.deltaTime * 3.0f;

        transform.Rotate(0, x, 0);
        transform.Translate(0, 0, z);

        bodyRing1.transform.Rotate(z*50, z*100, z*200);
        bodyRing2.transform.Rotate(z*200, z*50, z*100);

        animate();
    }

    void fireCmd()
    {
        randPt = Random.insideUnitCircle;
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
