using UnityEngine;
using System.Collections;

public class lookat_ball : MonoBehaviour {

	private GameObject flail_frame;

	// Use this for initialization
	void Start () {
		flail_frame = GameObject.Find("flail_frame");
	}
	
	// Update is called once per frame
	void Update () {
		transform.LookAt (flail_frame.transform.position);
		transform.Rotate (180, 0, 0);
	}
}
