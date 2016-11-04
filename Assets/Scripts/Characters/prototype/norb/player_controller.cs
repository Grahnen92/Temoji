using UnityEngine;
using System.Collections;
using System;

public class player_controller : MonoBehaviour {

	//Avatar parts
	private Rigidbody rb_head;
	private GameObject neck;
	private GameObject lwing;
	private GameObject rwing;
	//private Rigidbody rb_rwing;
	private GameObject fwing;
	private GameObject bwing;

	public GameObject certain_weapon;
	private GameObject current_weapon;

	//movement properties
	public float max_speed;
	private float curr_speed;
	private Vector3 planar_velocity;

	//hovering variables
	private double previous_hight_error = 0.0;
	private double hight_error;
	private double hight_integral = 0.0;
	private double hight_derivative;
	private double hight_adjustment;
	private const double max_hight_adjustment = 1000.0;
	private const double wanted_hight = 4.1;

	//rotational variables
	private float current_mouse_angle;

	private double prev_head_rot_error = 0.0;
	private double head_rot_error;
	private double head_rot_integral = 0.0;
	private double head_rot_derivative;
	private double head_rot_adjustment;

	private double prev_body_rot_error = 0.0;
	private double body_rot_error;
	private double body_rot_integral = 0.0;
	private double body_rot_derivative;
	private double body_rot_adjustment;

	//projectile
	private GameObject wing_projectile_prefab;
	private LayerMask default_mask = 1;

	void Start()
	{
		rb_head = GetComponent<Rigidbody> ();
		neck = GameObject.Find("final_prototype_neckjoint");
		lwing = GameObject.Find("final_prototype_lwing");
		rwing = GameObject.Find("final_prototype_rwing");
		//rb_rwing = rwing.GetComponent<Rigidbody> ();
		fwing = GameObject.Find("final_prototype_fwing");
		bwing = GameObject.Find("final_prototype_bwing");



		wing_projectile_prefab = Resources.Load ("final_prototype_wing_projectile") as GameObject;
	}
	/*
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
	*/
	void FixedUpdate()
	{

		//Turn function =====================================================================
		//mouse
		Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
		RaycastHit mouse_hit;
		Physics.Raycast (ray, out mouse_hit, 100, default_mask);
		Vector3 curr_mouse = mouse_hit.point - transform.position;
		curr_mouse.y = 0;
		curr_mouse.Normalize ();

		//head
		Vector3 curr_forward = transform.up;
		curr_forward.y = 0;
		curr_forward.Normalize ();

		head_rot_error = -Vector3.Angle (curr_mouse, curr_forward);
		if (Vector3.Cross (curr_mouse, curr_forward).y < 0)
			head_rot_error = -head_rot_error;
		
		head_rot_integral = head_rot_integral + head_rot_error * Time.deltaTime;
		head_rot_derivative = (head_rot_error - prev_head_rot_error) / Time.deltaTime;

		head_rot_adjustment = 0.1 * head_rot_error + 0.0 * head_rot_integral + 0.05 * head_rot_derivative;
		prev_head_rot_error = head_rot_error;
		rb_head.AddRelativeTorque(Vector3.forward * (float)head_rot_adjustment);

		//Body
		curr_forward = neck.transform.forward;
		curr_forward.y = 0;
		curr_forward.Normalize ();

		body_rot_error = -Vector3.Angle (curr_mouse, curr_forward);
		if (Vector3.Cross (curr_mouse, curr_forward).y < 0)
			body_rot_error = -body_rot_error;

		body_rot_integral = body_rot_integral + body_rot_error * Time.deltaTime;
		body_rot_derivative = (body_rot_error - prev_body_rot_error) / Time.deltaTime;

		body_rot_adjustment = 100.0 * body_rot_error + 0.0 * body_rot_integral + 1.0 * body_rot_derivative;
		prev_body_rot_error = body_rot_error;
		neck.GetComponent<Rigidbody>().AddRelativeTorque(Vector3.up * (float)body_rot_adjustment);

		// Hover function ===================================================================

		RaycastHit hit;
		if (Physics.Raycast (rb_head.transform.position, Vector3.down, out hit, 100.0f, default_mask)) {
			hight_error = wanted_hight - hit.distance;
			hight_integral = hight_integral + hight_error * Time.deltaTime;
			hight_derivative = (hight_error - previous_hight_error) / Time.deltaTime;

			hight_adjustment = 100.0 * hight_error + 0.0 * hight_integral + 50.0 * hight_derivative;
			hight_adjustment = Math.Min (Math.Max (0.0, hight_adjustment), max_hight_adjustment);

			previous_hight_error = hight_error;


			rb_head.AddForce (Vector3.up * (float)hight_adjustment);

		} else {
			previous_hight_error = previous_hight_error;
			hight_integral = 0.0;
		}

		//===================================================================================

		planar_velocity = rb_head.velocity; planar_velocity.y = 0.0f;
	
		//Planar movement
		float moveH = Input.GetAxis ("Horizontal");
		float moveV = Input.GetAxis ("Vertical");

		if (Input.GetButton ("Fire3"))
			curr_speed = max_speed * 2.0f;
		else
			curr_speed = max_speed;

		if (Input.GetButtonDown ("Fire2")) {
			rwing.transform.localEulerAngles = new Vector3(-10, 85, -92);
			rwing.transform.localPosition = new Vector3 (1.0f, -0.331f, -0.4f);

			//current_weapon = Instantiate (certain_weapon);
		} else if (Input.GetButton("Fire2"))  {
			if (Input.GetButtonDown ("Fire1")) {
				GameObject wing_projectile = Instantiate (wing_projectile_prefab) as GameObject;
				wing_projectile.transform.position = rwing.transform.position;
				wing_projectile.transform.rotation = rwing.transform.rotation;
				Vector3 tmp_vec = neck.transform.forward; tmp_vec.y = 0;
				wing_projectile.GetComponent<Rigidbody> ().AddForce (tmp_vec * 7000);
			}
		}else if(Input.GetButtonUp("Fire2")){
			rwing.transform.localEulerAngles =  new Vector3(0, 90, 0);
			rwing.transform.localPosition = new Vector3(0.66f, -0.131f, 0.0f);
		}

		Vector3 wanted_velocity = new Vector3(moveH, 0, moveV) * curr_speed;
		Vector3 velocity_diff = wanted_velocity - planar_velocity;

		float diff_magnitude = velocity_diff.magnitude;
		velocity_diff.Normalize ();


		if (planar_velocity.magnitude < curr_speed) {
			rb_head.AddForce (velocity_diff * diff_magnitude * diff_magnitude * Time.deltaTime * 60);
		}

		//Jump
		//if(Input.GetButton("Jump"))
		if (Input.GetButtonDown ("Jump") ) 
		{
			rb_head.AddForce(0.0f, 100000.0f * Time.deltaTime, 0.0f);
		}

		//drag force that slows the player down in the x and z dirrs
		rb_head.velocity = rb_head.velocity - planar_velocity * 0.1f; 
	}

	void LateUpdate()
	{
		Vector3 tmpAng;
		if (Input.GetButton ("Fire2")) {
			tmpAng = rwing.transform.eulerAngles;
			tmpAng.z = -90;
			rwing.transform.eulerAngles = tmpAng;
		}
		tmpAng = transform.eulerAngles;
		tmpAng.x = -90;
		tmpAng.y = 0;
		//transform.eulerAngles = tmpAng;
	}
}
