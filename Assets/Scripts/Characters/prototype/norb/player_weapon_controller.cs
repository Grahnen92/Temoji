using UnityEngine;
using System.Collections;

public class player_weapon_controller : MonoBehaviour 
{
	private GameObject owner;
	private Vector3 offset;
	private Rigidbody rb;
	private Vector3 wanted_pos;
	private Vector3 current_pos;
	// Use this for initialization
	void Start () 
	{
		owner = GameObject.Find("Player");
		rb = GetComponent<Rigidbody> ();
		offset = transform.position - owner.transform.position;
		current_pos = new Vector3 (0.0f, 0.0f, 1.0f);
	}
	
	// Update is called once per frame
	void FixedUpdate () 
	{
		Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
		RaycastHit hit;
		Physics.Raycast (ray, out hit, 100);
		Vector3 tmp_vec = hit.point - owner.transform.position;
		tmp_vec.y = 0;
		//tmp_vec.Normalize();
		Vector3 weapon_pos = owner.transform.position + tmp_vec;

		//Planar movement
		//float swingH = Input.GetAxis ("HorizontalRight");
		//float swingV = Input.GetAxis ("VerticalRight");
		//Vector3 weapon_pos = new Vector3 (swingH, 0, swingV);


		//Vector3 weapon_pos = new Vector3 (1.0f, 0, 1.0f);
		if (weapon_pos.magnitude > 0.001f) 
		{
			current_pos = weapon_pos;
		}
		//transform.position = (owner.transform.position + current_pos);
		transform.position = weapon_pos;

	}
}
