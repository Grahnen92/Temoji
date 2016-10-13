using UnityEngine;
using System.Collections;

public class towerShoot_Bullet : MonoBehaviour {
    void Start () { }

    void OnCollisionEnter(Collision coll)//bullet hits something, bullet disappears
    {
        print(coll.gameObject);
        Destroy(gameObject);
    }
}
