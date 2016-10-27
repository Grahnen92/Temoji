using UnityEngine;
using System.Collections;
using UnityEngine.Networking;

public class Combat : NetworkBehaviour {

    public int maxHealth = 10;

    //[SyncVar]
    protected int health;

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

    public virtual void TakeDamage(int amount) { }
}
