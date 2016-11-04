using UnityEngine;
using System.Collections;
using System.Collections.Generic;



public class SlowingTower: MonoBehaviour
{

    public LayerMask mask = (1 << 10) | (1 << 13) | (1 << 14) | (1 << 18);
    public float radius = 5.0f;

    private float slowAmount = 10;


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
            tmpRB.angularDrag += slowAmount;
        }
        var pRB = col.gameObject.GetComponentInParent<Rigidbody>();
        //foreach (var pRB in pRBs)
        //{
        //ugly solution to neckjoint triggering a trigger collision while aiming
        if (pRB == tmpRB || pRB.gameObject.layer == 8)
            return;

        pRB.drag += slowAmount;
        pRB.angularDrag += slowAmount;
       // }
        //var cRBs = GetComponentsInChildren<Rigidbody>();
        //foreach (var cRB in cRBs)
        //{
        //    cRB.drag += slowAmount;
        //}
    }

    void OnTriggerExit(Collider col)
    {
        Rigidbody tmpRB = col.gameObject.GetComponent<Rigidbody>();
        if (tmpRB)
        {
            tmpRB.drag -= slowAmount;
            tmpRB.angularDrag -= slowAmount;
        }
        var pRB = col.gameObject.GetComponentInParent<Rigidbody>();
        //foreach (var pRB in pRBs)
        // {
        //ugly solution to neckjoint triggering a trigger collision while aiming
        if (pRB == tmpRB || pRB.gameObject.layer == 8)
            return;
        pRB.drag -= slowAmount;
        pRB.angularDrag -= slowAmount;
       // }
        //var cRBs = GetComponentsInChildren<Rigidbody>();
       // foreach (var cRB in cRBs)
        //{
         //   cRB.drag -= slowAmount;
        //}
    }

    public void Death()
    {
        var cols = Physics.OverlapSphere(transform.position, radius, mask);
        var rigidbodies = new List<Rigidbody>();
        foreach (var col in cols)
        {
            Rigidbody tmpRB = col.gameObject.GetComponent<Rigidbody>();
            if (tmpRB != null && !rigidbodies.Contains(tmpRB)) {
                rigidbodies.Add(tmpRB);
            }
            if(col.transform.parent != null)
            {
                Rigidbody pRB = col.transform.parent.gameObject.GetComponent<Rigidbody>();
                                                                //ugly solution to neckjoint triggering a trigger collision while aiming
                if (pRB != null && !rigidbodies.Contains(pRB) && pRB.gameObject.layer != 8)
                {
                    rigidbodies.Add(pRB);
                }
            }
            
        }
        foreach (var rb in rigidbodies)
        {
            rb.drag -= slowAmount;
            rb.angularDrag -= slowAmount;
        }
    }

    // Update is called once per frame
    void Update()
    {

    }
}
