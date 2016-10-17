using UnityEngine;
using System.Collections;
using System;

public class TowerBuilder : MonoBehaviour {

	public GameObject tower_prefab;
	private float buildTimer;
	private float buildTime;

    // Use this for initialization
    void Start () {
		buildTimer = 0;
		buildTime = 5;
        tower_prefab = Resources.Load("Tower") as GameObject;
	}
	
	// Update is called once per frame
	void Update () {

		buildTimer += Time.deltaTime;

		if (buildTimer > buildTime) {
            GameObject tower = Instantiate(tower_prefab);
            tower.transform.position = transform.position;
			Destroy(gameObject);
		}
	}
}
