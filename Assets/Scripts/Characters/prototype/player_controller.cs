using UnityEngine;
using System.Collections;
using System;

public class player_controller : MonoBehaviour {

	private Rigidbody rb;

	private bool isColliding;

	public GameObject certain_weapon;
	private GameObject current_weapon;

	public float max_speed;
	private float curr_speed;
	private Vector3 planar_velocity;

	private double previous_hight_error = 0.0;
	private double hight_error;
	private double hight_integral = 0.0;
	private double hight_derivative;
	private double hight_adjustment;
	private const double max_hight_adjustment = 1000.0;
	private const double wanted_hight = 4;

	void Start()
	{
		rb = GetComponent<Rigidbody> ();
	}
	void OnCollisionEnter(Collision collision)
	{
		isColliding = true;
	}
	void OnCollisionStay(Collision collision)
	{
		isColliding = true;
	}
	void OnCollisionExit(Collision collision)
	{
		isColliding = false;
	}
	void FixedUpdate()
	{

		// Hover function ===================================================================

		RaycastHit hit;
		if (Physics.Raycast (rb.transform.position, Vector3.down, out hit, 100.0f)) {
			hight_error = wanted_hight - hit.distance;
			hight_integral = hight_integral + hight_error * Time.deltaTime;
			hight_derivative = (hight_error - previous_hight_error) / Time.deltaTime;
			hight_adjustment = 100.0 * hight_error + 10.0 * hight_integral + 50.0 * hight_derivative;
			previous_hight_error = hight_error;
			hight_adjustment = Math.Min (Math.Max (0.0, hight_adjustment), max_hight_adjustment);
			//if(hight_adjustment > 0.0 && hight_adjustment < max_hight_adjustment)
			rb.AddForce (Vector3.up * (float)hight_adjustment);
			//print (rb.transform.position.y);
			//print (hit.distance);
			//print (hight_error);

				
			print (hight_adjustment);
		} else {
			previous_hight_error = previous_hight_error;
			hight_integral = 0.0;
		}

		//===================================================================================

		planar_velocity = rb.velocity; planar_velocity.y = 0.0f;
	
		//Planar movement
		float moveH = Input.GetAxis ("Horizontal");
		float moveV = Input.GetAxis ("Vertical");

		if (Input.GetButton ("Fire3"))
			curr_speed = max_speed * 2.0f;
		else
			curr_speed = max_speed;

		if (Input.GetButtonDown ("Fire1")) 
		{
			current_weapon = Instantiate (certain_weapon);
		}

		if(Input.GetButtonUp("Fire1"))
		{
			Destroy (current_weapon);
		}

		Vector3 wanted_velocity = new Vector3(moveH, 0, moveV) * curr_speed;
		Vector3 velocity_diff = wanted_velocity - planar_velocity;

		float diff_magnitude = velocity_diff.magnitude;
		velocity_diff.Normalize ();
		///*

		//if (isColliding) 
		if(true)
		{
			if (planar_velocity.magnitude < curr_speed) {
				rb.AddForce (velocity_diff * diff_magnitude * diff_magnitude * Time.deltaTime * 60);
			}

			//Jump
			//if(Input.GetButton("Jump"))
			if (Input.GetButtonDown ("Jump") ) 
			{
				print("yo");
				rb.AddForce(0.0f, 100000.0f * Time.deltaTime, 0.0f);
			}

			//drag force that slows the player down in the x and z dirrs
			rb.velocity = rb.velocity - planar_velocity * 0.1f; 
		}
	}
}
