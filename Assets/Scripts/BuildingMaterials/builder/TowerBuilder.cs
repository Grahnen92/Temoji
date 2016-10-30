using UnityEngine;
using System.Collections;
using System;

public class TowerBuilder : MonoBehaviour {

	public GameObject tower_prefab;
    private ParticleSystem particle_system;
	private float buildTimer;
	private float buildTime;
    private float finalSpeed;

    // Use this for initialization
    void Start () {
		buildTimer = 0;
		buildTime = 2;
       
        particle_system = gameObject.GetComponentInChildren<ParticleSystem>();
        Color tmp_color = GetComponent<Renderer>().material.color;
        tmp_color.a = 0.2f;
        GetComponent<Renderer>().material.color = tmp_color;

    }

    public void loadTower(String _tower_resource)
    {
        //tower_prefab = Resources.Load("Towers\\ProjectileTower\\projectile_tower_base") as GameObject;
        tower_prefab = Resources.Load(_tower_resource) as GameObject;

    }
	
	// Update is called once per frame
	void Update () {

        RaycastHit hit;
        if (Physics.SphereCast(transform.position, 0.05f, Vector3.down, out hit, 100.0f, 1))
        //if (Physics.Raycast(transform.position, Vector3.down, out hit, 100.0f, 1))
        {

            finalSpeed = (hit.distance / 0.3f) + 1;
        }

        buildTimer += Time.deltaTime;
        particle_system.startSpeed = (buildTimer/buildTime) * finalSpeed;


        if (buildTimer > buildTime) {
            GameObject tower = Instantiate(tower_prefab);
            tower.transform.position = hit.point;
            Destroy(gameObject);
        }
	}
}
