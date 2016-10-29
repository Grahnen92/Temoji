using UnityEngine;
using System.Collections;
using System.Collections.Generic;



public class SlowingTower: MonoBehaviour
{

    public LayerMask mask = (1 << 10) | (1 << 13) | (1 << 14) | (1 << 18);
    public float radius = 5.0f;

    private float slowAmount = 100;


    //private List<GameObject> Entities = new List<GameObject>();
    //private List<float> originalDrag = new List<float>();
    // Use this for initialization
    void Start()
    {
        transform.GetChild(0).transform.localScale = new Vector3(radius * 2.0f, radius * 2.0f, radius * 2.0f);
    }

    void OnTriggerEnter(Collider col)
    {
        Rigidbody tmpRB = col.gameObject.GetComponent<Rigidbody>();
        if (tmpRB)
        {
            tmpRB.drag += slowAmount;
        }
        var pRBs = GetComponentsInParent<Rigidbody>();
        foreach (var pRB in pRBs)
        {
            pRB.drag += slowAmount;
        }
        var cRBs = GetComponentsInChildren<Rigidbody>();
        foreach (var cRB in cRBs)
        {
            cRB.drag += slowAmount;
        }

        print("hej!");
    }

    void OnTriggerExit(Collider col)
    {
        Rigidbody tmpRB = col.gameObject.GetComponent<Rigidbody>();
        if (tmpRB)
        {
            tmpRB.drag -= slowAmount;
        }
        var pRBs = GetComponentsInParent<Rigidbody>();
        foreach (var pRB in pRBs)
        {
            pRB.drag -= slowAmount;
        }
        var cRBs = GetComponentsInChildren<Rigidbody>();
        foreach (var cRB in cRBs)
        {
            cRB.drag -= slowAmount;
        }
        print("hejdå!");
    }

    public void Death()
    {
        var cols = Physics.OverlapSphere(transform.position, radius, mask);
        var rigidbodies = new List<Rigidbody>();
        foreach (var col in cols)
        {
            if (col.attachedRigidbody != null && !rigidbodies.Contains(col.attachedRigidbody))
            {
                rigidbodies.Add(col.attachedRigidbody);
            }
        }
    }

    // Update is called once per frame
    void Update()
    {

    }
}
