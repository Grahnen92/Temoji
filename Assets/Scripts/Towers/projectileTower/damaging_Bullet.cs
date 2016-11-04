using UnityEngine;
using System.Collections;

public class damaging_Bullet : MonoBehaviour {
    void Start () { }

    void OnCollisionEnter(Collision coll)//bullet hits something, bullet disappears
    {
        Destroy(gameObject);
    }
}
