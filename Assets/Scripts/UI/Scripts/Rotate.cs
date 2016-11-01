using UnityEngine;
using System.Collections;

public class Rotate : MonoBehaviour {

    public Vector3 angle;
    public bool rotate = true;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
        if(rotate)
        transform.Rotate(angle);
    }
}
