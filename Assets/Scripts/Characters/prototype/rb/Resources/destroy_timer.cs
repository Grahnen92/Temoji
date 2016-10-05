using UnityEngine;
using System.Collections;

public class destroy_timer : MonoBehaviour {

	// Use this for initialization
	private float timer;
	void Start () {
		timer = 0.0f;
	}
	//void OnCollisionEnter(Collision collision)
	//{
	//	Destroy (gameObject);
	//}
	
	// Update is called once per frame
	void Update () {
		timer += Time.deltaTime;
		if (timer > 5.0f)
			Destroy (gameObject);
	}
}
