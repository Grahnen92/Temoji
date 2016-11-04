using UnityEngine;
using System.Collections;
using System;

public class ball_player_controller : MonoBehaviour {

	//Avatar parts
	private Rigidbody rb_head;
	private GameObject ball_prototype;
	private GameObject flail_frame;

	private GameObject scoop_prefab;
	private GameObject scoop;

	private GameObject flail_prefab;
	private GameObject flail;

	private GameObject current_tool;

	private Vector3 flail_to_body;
	private Vector3 flail_to_body_normalized;

	private Vector3 mouse_to_body;

	private bool released = false;
	private bool locked = false;
	private bool docked = true;

	public float max_speed;
	private float curr_speed;
	private Vector3 planar_velocity;

	private double prev_body_rot_error = 0.0;
	private double body_rot_error;
	private double body_rot_integral = 0.0;
	private double body_rot_derivative;
	private double body_rot_adjustment;

	private LayerMask default_mask = 1;

	void Start()
	{
		rb_head = GetComponent<Rigidbody> ();
		flail_frame = GameObject.Find("flail_frame");
		ball_prototype = GameObject.Find("main_frame");

		scoop_prefab = Resources.Load ("scoop_prefab") as GameObject;

		flail_prefab = Resources.Load ("flail_prefab") as GameObject;
		flail = Instantiate (flail_prefab) as GameObject;
		flail.transform.position = flail_frame.transform.position;
		flail.transform.parent = flail_frame.transform;
		docked = true;

	}


	void OnCollisionEnter(Collision collision)
	{
		if (!released) {
			if (collision.transform.gameObject == flail) {
				if(locked)
					flail.GetComponent<Rigidbody> ().constraints = RigidbodyConstraints.None;
				
				Destroy(flail.GetComponent<SpringJoint> ());
				Destroy(flail.GetComponent<Rigidbody> ());

				flail.transform.position = flail_frame.transform.position;
				flail.transform.parent = flail_frame.transform;
				docked = true;
			}
		}
	}
	/*
	void OnCollisionStay(Collision collision)
	{
		isColliding = true;
	}
	void OnCollisionExit(Collision collision)
	{
		isColliding = false;
	}
	*/

	void Update(){
		//shoot ball =========================================================================
		//shoot

		//deploy scoop
		if (Input.GetButtonDown ("OpenWings")) {
			Vector3 tmp_pos = flail.transform.position;
			Destroy (flail);
			flail = Instantiate (scoop_prefab) as GameObject;
			flail.transform.position = tmp_pos;
			flail.transform.parent = ball_prototype.transform;
			flail.AddComponent<Rigidbody> ();
			flail.GetComponent<Rigidbody> ().constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;
			flail.AddComponent<SpringJoint> ();
			flail.GetComponent<SpringJoint> ().autoConfigureConnectedAnchor = false;
			flail.GetComponent<SpringJoint> ().connectedBody = flail_frame.GetComponent<Rigidbody>();
			flail.GetComponent<SpringJoint> ().anchor = Vector3.zero;
			flail.GetComponent<SpringJoint> ().connectedAnchor = Vector3.zero;
			flail.GetComponent<SpringJoint> ().spring = 100;
			flail.GetComponent<SpringJoint> ().maxDistance = flail_to_body.magnitude;
		}

		//shoot and lock
		if(docked && !locked){
			if (Input.GetButtonDown ("Fire1")) {
				released = true;
				docked = false;

				flail.AddComponent<Rigidbody> ();
				SpringJoint tmpJoint = flail.AddComponent<SpringJoint> ();

				flail.transform.parent = ball_prototype.transform;
				flail.transform.position = flail_frame.transform.position + mouse_to_body*1.05f;
				tmpJoint.autoConfigureConnectedAnchor = false;
				tmpJoint.connectedBody = flail_frame.GetComponent<Rigidbody> ();
				tmpJoint.anchor = Vector3.zero;
				tmpJoint.connectedAnchor = Vector3.zero;
				tmpJoint.maxDistance = 10;
				tmpJoint.spring = 100;
				flail.GetComponent<Rigidbody> ().AddForce ((mouse_to_body) * 1000);
			} 
		}
			
		//pull
		if (released) {

			if (Input.GetButtonDown ("Fire2")) {
				
				flail.GetComponent<SpringJoint> ().maxDistance = 0;
				if (locked) {
					flail.GetComponent<SpringJoint> ().spring = 50;
				} else {
					flail.GetComponent<SpringJoint> ().spring = 20;
				}
				released = false;
			}

			if (Input.GetButtonDown ("Jump")) {
				locked = true;
				flail.GetComponent<Rigidbody> ().constraints = RigidbodyConstraints.FreezeAll;
				flail.GetComponent<SpringJoint> ().maxDistance = Mathf.Min(flail_to_body.magnitude, 10);
			} 
		}
		//lock
		if (Input.GetButtonUp ("Jump")) {
			if(!docked)
				flail.GetComponent<Rigidbody> ().constraints = RigidbodyConstraints.None;
			locked = false;
		}
		//sprint
		if (Input.GetButton ("Fire3"))
			curr_speed = max_speed * 2.0f;
		else
			curr_speed = max_speed;

		//Jump ==============================================================================
		//if(Input.GetButton("Jump"))
		//if (Input.GetButtonDown ("Jump") ) 
		//{
		//	rb_head.AddForce (Vector3.up * Time.deltaTime * 10000);
		//}
	}

	void FixedUpdate()
	{
		flail_to_body = flail.transform.position - flail_frame.transform.position;
		flail_to_body_normalized = flail_to_body.normalized;
		//Turn function =====================================================================
		//mouse
		Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
		RaycastHit mouse_hit;
		Physics.Raycast (ray, out mouse_hit, 100, default_mask);
		mouse_to_body = mouse_hit.point - transform.position;
		mouse_to_body.y = 0;
		mouse_to_body.Normalize ();

		if (released || locked) {
			flail_frame.transform.LookAt (flail.transform.position);
			flail_frame.transform.Rotate (-90, 0, 0);
		}


		if (!released && !locked) {
			body_rot_error = -Vector3.Angle (mouse_to_body, flail_to_body_normalized);
			if (Vector3.Cross (mouse_to_body, flail_to_body_normalized).y < 0)
				body_rot_error = -body_rot_error;

			body_rot_integral = body_rot_integral + body_rot_error * Time.deltaTime;
			body_rot_derivative = (body_rot_error - prev_body_rot_error) / Time.deltaTime;

			body_rot_adjustment = 0.02 * body_rot_error + 0.0 * body_rot_integral + 0.01 * body_rot_derivative;
			prev_body_rot_error = body_rot_error;
			//flail.GetComponent<Rigidbody>().AddForce(Vector3.Cross(Vector3.up,flail_to_body_normalized) * (float)body_rot_adjustment);

		}

			
		//Planar movement ===================================================================
		planar_velocity = rb_head.velocity; planar_velocity.y = 0.0f;

		float moveH = Input.GetAxis ("Horizontal");
		float moveV = Input.GetAxis ("Vertical");

		Vector3 wanted_velocity = new Vector3(moveH, 0, moveV) * curr_speed;
		Vector3 velocity_diff = wanted_velocity - planar_velocity;

		float diff_magnitude = velocity_diff.magnitude;
		velocity_diff.Normalize ();

		if (planar_velocity.magnitude < curr_speed) {
			rb_head.AddForce (velocity_diff * diff_magnitude * diff_magnitude * Time.deltaTime * 60);
		}

		//drag force that slows the player down in the x and z dirrs
		rb_head.velocity = rb_head.velocity - planar_velocity * 0.1f;

	}

	void LateUpdate()
	{
		
	}
}
