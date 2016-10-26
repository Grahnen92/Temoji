using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SlowingTower: MonoBehaviour
{
    private List<GameObject> Entities = new List<GameObject>();
    private List<float> originalDrag = new List<float>();

    private Shader refractive;
    

    // Use this for initialization
    void Start()
    {
        refractive = GetComponent<Shader>();
       // refractive.
    }

    void OnTriggerEnter(Collider col)
    {
        
        Entities.Add(col.gameObject);
        originalDrag.Add(col.gameObject.GetComponent<Rigidbody>().drag);
        col.gameObject.GetComponent<Rigidbody>().drag = col.gameObject.GetComponent<Rigidbody>().drag  + 10;
        print("hej");
    }

    void OnTriggerExit(Collider col)
    {
        int tmpIndex = Entities.IndexOf(col.gameObject);
        if(tmpIndex > 0)
        {
            col.gameObject.GetComponent<Rigidbody>().drag = originalDrag[tmpIndex];
            Entities.RemoveAt(tmpIndex);
            originalDrag.RemoveAt(tmpIndex);
            print("hejdå");
        }
        
        
    }

    // Update is called once per frame
    void Update()
    {

    }
}
