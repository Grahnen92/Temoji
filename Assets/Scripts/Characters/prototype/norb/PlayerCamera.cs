using UnityEngine;
using System.Collections;

public class PlayerCamera : MonoBehaviour {
	private Vector3 offset;
	private Vector3 diff_vec;
	private float distance;
	private Rigidbody rb;

	private Vector2 camera_error;
	private Vector2 camera_prev_error = Vector2.zero;
	private Vector2 camera_error_integral = Vector2.zero;
	private Vector2 camera_error_derivative;
	private Vector2 camera_adjustment;
	private GameObject player;
    
    void Start () {
        player = GameObject.Find("final_prototype_head");

        rb = GetComponent<Rigidbody> ();
		offset = transform.position - player.transform.position;
	}

	void LateUpdate () {
		//transform.position = player.transform.position + offset;
		///*
		Vector3 diff_vec = (player.transform.position + offset) - transform.position;
		diff_vec.y = 0.0f;
		distance = diff_vec.magnitude;
		diff_vec.Normalize();
		//rb.AddForce (distance);
		rb.velocity = diff_vec*distance*distance;
		//*/

		/*
		Vector3 diff_vec = (player.transform.position + offset) - transform.position;
		diff_vec.y = 0.0f;
		distance = diff_vec.magnitude;
		Vector2 diff_vec_2 = new Vector2 (diff_vec.x, diff_vec.z);

		camera_error = -diff_vec_2;

		camera_error_integral = camera_error_integral + camera_error * Time.deltaTime;
		camera_error_derivative = (camera_error - camera_prev_error) / Time.deltaTime;

		camera_adjustment = 2.0f * camera_error + 3.0f * camera_error_integral + 0.0f * camera_error_derivative;
		camera_prev_error = camera_error;

		diff_vec_2 = Vector3.Scale (diff_vec_2, camera_adjustment);
		diff_vec.x = diff_vec_2.x; diff_vec.z = diff_vec_2.y;	
		rb.AddForce (diff_vec);
		//rb.velocity = -diff_vec*camera_adjustment;
		*/
	}
}
