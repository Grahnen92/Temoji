using UnityEngine;
using System.Collections;

public class destroy_timer : MonoBehaviour {

	// Use this for initialization
	private float timer;
    private GameObject explosion_prefab;
    private GameObject explosion;
    private bool start_timer = false;
    void Start () {
		timer = 0.0f;
        explosion_prefab = Resources.Load("Explosion") as GameObject;
    }
	void OnCollisionEnter(Collision collision)
	{
        explosion = Instantiate(explosion_prefab) as GameObject;
        explosion.transform.position = transform.position;
        explosion.transform.localScale = new Vector3(0.5f, 0.5f, 0.5f);
        //Destroy (gameObject);
    }
	
	// Update is called once per frame
	void Update () {
        if(start_timer)
		    timer += Time.deltaTime;

        if (timer > 5.0f) {
            Destroy(explosion);
            Destroy(explosion_prefab);
            Destroy(gameObject);
        }
            
	}
}
