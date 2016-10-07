using UnityEngine;
using System.Collections;

public class projectile_collision_destroy : MonoBehaviour {

	// Use this for initialization
    private GameObject explosion_prefab;
    private GameObject explosion;
    void Start () {
        explosion_prefab = Resources.Load("Energy_explosion") as GameObject;
    }
	void OnCollisionEnter(Collision collision)
	{
        explosion = Instantiate(explosion_prefab) as GameObject;
        explosion.transform.position = transform.position;
        explosion.transform.localScale = new Vector3(0.5f, 0.5f, 0.5f);
        Destroy (gameObject);
    }
	
	// Update is called once per frame
	void Update () {
            
	}
}
