using UnityEngine;
using System.Collections;

public class PlayerCamera : MonoBehaviour {
	public GameObject player;
	private Vector3 offset;
	private Vector3 diff_vec;
	private float distance;
	private Rigidbody rb;

	void Start () {
		rb = GetComponent<Rigidbody> ();
		offset = transform.position - player.transform.position;
	}

	void LateUpdate () {
		//transform.position = player.transform.position + offset;
		Vector3 diff_vec = (player.transform.position + offset) - transform.position;
		diff_vec.y = 0.0f;
		distance = diff_vec.magnitude;
		diff_vec.Normalize();
		//rb.AddForce (distance);
		rb.velocity = diff_vec*distance*distance;
	}
}
