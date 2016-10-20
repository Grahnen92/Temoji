using UnityEngine;
using System.Collections;


/* *
 * Class to have object point at other objects 
 * Arrow will rotate around the Y-axis of the character
 * Configurable fields:
 * 1) arrowPosition: Indicates where the arrow will be, relative to the character
 * 2) pointer: The model that will represent the pointer
 * 3) target: The target that the arrow will point to
 *      - Configurable during runtime through setTarget() method
 * */

public class TargetPointer : MonoBehaviour {

    // The position where the arrow is
    public Transform arrowPosition;

    // The model used for the arrow
    public GameObject pointer;

    // Target for arrow to point to
    public GameObject target;

	// Initialize the pointer arrow
	void Start () {
        pointer = Instantiate(pointer);
        pointer.transform.parent = this.transform;
        pointer.transform.position = arrowPosition.position;
    }
	
	// Update is called once per frame
	void Update () {
        // Rotate to point to target if there is a target
        if (target != null)
        {
            transform.LookAt(target.transform);
        }
	}

    // Change the target which the arrow will point at
    public void setTarget(GameObject newTarget)
    {
        target = newTarget;
    }
}
