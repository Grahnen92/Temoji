using UnityEngine;
using System.Collections;

public class monsterFindPath : MonoBehaviour {

    public NavMeshAgent monster;
    public Transform target;
	// Use this for initialization
	void Start () {
        monster = GetComponent<NavMeshAgent>();
	}
	
	// Update is called once per frame
	void Update () {
        monster.SetDestination(target.position);
	}
}
