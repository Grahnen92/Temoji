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
	}
}
