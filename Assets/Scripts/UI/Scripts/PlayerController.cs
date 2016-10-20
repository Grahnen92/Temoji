using UnityEngine;
using System.Collections;

public class PlayerController : MonoBehaviour {

    public GameObject target1;
    public GameObject target2;

    int currTarget = 0;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
        var x = Input.GetAxis("Horizontal") * Time.deltaTime * 150.0f;
        var z = Input.GetAxis("Vertical") * Time.deltaTime * 3.0f;

        transform.Rotate(0, x, 0);
        transform.Translate(0, 0, z);

        // Switching targets for Target Pointer
        if (Input.GetKeyDown(KeyCode.Space))
        {
            // Getting TargetPointer script in children to change target
            GameObject tp = transform.FindChild("TargetPointer").gameObject;
            TargetPointer p = tp.GetComponent<TargetPointer>();
            if (currTarget == 0)
            {
                currTarget = 1;
                p.setTarget(target2);
            }
            else
            {
                currTarget = 0;
                p.setTarget(target1);
            }
        }
    }
}
