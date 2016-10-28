using UnityEngine;
using System.Collections;

public class MaterialPart : MonoBehaviour {

    void OnCollisionEnter(Collision col)
    {
        if(col.gameObject.layer != 12)
        {
            gameObject.GetComponent<Rigidbody>().isKinematic = false;
            gameObject.GetComponent<Rigidbody>().useGravity = true;
            gameObject.layer = 12;
            for (int i = 0; i < gameObject.transform.childCount; i++)
            {
                gameObject.transform.GetChild(i).gameObject.layer = 12;
            }
            Destroy(gameObject.GetComponent<MaterialPart>());
        }
    }

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
