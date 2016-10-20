using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;


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

    private Vector3 prev_pos_error;
    private Vector3 pos_error;
    private Vector3 pos_integral;
    private Vector3 pos_derivative;
    private Vector3 wanted_pos;
    private Vector3 pos_adjustment;
    private Vector3 max_pos_adjustment;

    void Start () {
        player = GameObject.Find("final_prototype_head");

        rb = GetComponent<Rigidbody> ();
        offset = new Vector3(0, 8, -5);
        transform.position = player.transform.position + offset;
    }

	void LateUpdate () {

        wanted_pos = player.transform.position + offset;
        pos_error = wanted_pos - transform.position;

        pos_integral = pos_integral + pos_error * Time.deltaTime;
        pos_derivative = (pos_error - prev_pos_error) / Time.deltaTime;

        pos_adjustment = 1.0f * pos_error + 0.0f * pos_integral + 5.0f * pos_derivative;

        prev_pos_error = pos_error;

        rb.AddForce(pos_adjustment);

    }
}
