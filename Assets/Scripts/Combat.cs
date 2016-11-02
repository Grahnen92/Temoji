using UnityEngine;
using System.Collections;
using UnityEngine.Networking;

public class Combat : NetworkBehaviour {

    public float maxHealth = 10;

    //[SyncVar]
    protected float health;

    public Combat()
    {
        health = maxHealth;
    }

    // Use this for initialization
    void Start () {
        health = maxHealth;
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    public virtual void TakeDamage(float amount) { }
}
