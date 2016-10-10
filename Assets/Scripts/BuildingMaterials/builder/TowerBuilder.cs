using UnityEngine;
using System.Collections;
using System;

public class TowerBuilder : MonoBehaviour {

	public GameObject tower;
	private float buildTimer;
	private float buildTime;

	// Use this for initialization
	void Start () {
		buildTimer = 0;
		buildTime = 5;
	}
	
	// Update is called once per frame
	void Update () {

		buildTimer += Time.deltaTime;

		if (buildTimer > buildTime) {
			//instantiate tower
			Destroy(gameObject);
		}
	}
}
