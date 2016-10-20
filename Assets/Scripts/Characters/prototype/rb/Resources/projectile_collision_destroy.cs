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
        GameObject hit = collision.gameObject;

        NavigationScript hit_player = hit.GetComponent<NavigationScript>();
        Base_Combat hit_base = hit.GetComponent<Base_Combat>();
        Collectable_Combat hit_collectable = hit.GetComponent<Collectable_Combat>();

        if (hit_player)
        {
            print("hitplayer!=null");
            Enemy1_Combat enemy1_combat = hit.GetComponent<Enemy1_Combat>();
            enemy1_combat.TakeDamage(10);


        }

        if (hit_base)
        {
            print("hit base!");
            Base_Combat base_combat = hit.GetComponent<Base_Combat>();
            base_combat.TakeDamage(10);
        }

        if (hit_collectable)
        {
            print("hit collectable!");
            Collectable_Combat col_combat = hit.GetComponent<Collectable_Combat>();
            col_combat.TakeDamage(10);
        }

        Destroy(gameObject);
    }

    // Update is called once per frame
    void Update () {
            
	}
}
