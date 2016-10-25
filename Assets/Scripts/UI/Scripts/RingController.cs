using UnityEngine;
using System.Collections;

public class RingController : MonoBehaviour {

    public GameObject bullet;

    GameObject backOuterRing;
    GameObject backInnerRing;


    // Use this for initialization
    void Start () {
        backOuterRing = transform.Find("Back Ring").Find("Outer Plate Ring").gameObject;
        backInnerRing = transform.Find("Back Ring").Find("Inner Plate Ring").gameObject;
    }

	// Update is called once per frame
	void Update () {
        if (Input.GetButton("Fire1"))
        {
            backOuterRing.GetComponent<MakePlateRings>().rotationSpeed = 3;
            backInnerRing.GetComponent<MakePlateRings>().rotationSpeed = 3;
            Instantiate(bullet);
        }
        else
        {
            backOuterRing.GetComponent<MakePlateRings>().rotationSpeed = 1;
            backInnerRing.GetComponent<MakePlateRings>().rotationSpeed = 1;
        }

        var x = Input.GetAxis("Horizontal") * Time.deltaTime * 150.0f;
        var z = Input.GetAxis("Vertical") * Time.deltaTime * 3.0f;

        transform.Rotate(0, x, 0);
        transform.Translate(0, 0, z);
    }

    void fireCmd()
    {
        //Random.insideUnitCircle;
    }
}
