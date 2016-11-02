using UnityEngine;
using System.Collections;

public class ArmProjectile : MonoBehaviour {

	// Use this for initialization
    private GameObject explosion_prefab;
    private GameObject explosion;
    public AudioClip rockHit;

    void Start () {
        explosion_prefab = Resources.Load("Energy_explosion") as GameObject;
    }
	void OnCollisionEnter(Collision collision)
	{
        explosion = Instantiate(explosion_prefab) as GameObject;
        explosion.transform.position = transform.position;
        explosion.transform.localScale = new Vector3(0.5f, 0.5f, 0.5f);
        GameObject hit = collision.gameObject;

        Combat DamageHit = hit.GetComponent<Combat>();

        if (DamageHit)
            DamageHit.TakeDamage(10);

        if(hit.layer == 21 && hit.transform.parent != null)
        {
            if(hit.tag == "Stone")
            {
                GameObject tmpMaterialPart = hit.transform.parent.gameObject;
                tmpMaterialPart.transform.parent.gameObject.GetComponent<AudioSource>().Play();

                tmpMaterialPart.AddComponent<Rigidbody>().mass = 4.0f;
                tmpMaterialPart.AddComponent<DestroyTimer>().destructionTime = 30.0f;
                tmpMaterialPart.layer = 12;
                //for (int i = 0; i < tmpMaterialPart.transform.childCount; i++)
                //{
                //    tmpMaterialPart.transform.GetChild(i).gameObject.layer = 12;
                //}
           
                tmpMaterialPart.transform.GetChild(0).gameObject.layer = 12;
               
                GameObject tmpTopPart = hit.transform.parent.parent.GetChild(0).gameObject;
                tmpMaterialPart.transform.parent = null;
                if (tmpTopPart.layer != 12)
                //if (tmpTopPart.transform.parent.childCount < 2 && tmpT    §opPart.ToString() == "shard1")
                {
                    tmpTopPart.AddComponent<Rigidbody>().mass = 4.0f; ;
                    tmpTopPart.AddComponent<DestroyTimer>().destructionTime = 30.0f;
                    tmpTopPart.layer = 12;
                    //for (int i = 0; i < tmpMaterialPart.transform.childCount; i++)
                    //{
                    //    tmpMaterialPart.transform.GetChild(i).gameObject.layer = 12;
                    //}
                    tmpTopPart.transform.GetChild(0).gameObject.layer = 12;
                }
                
            }
            else if(hit.tag == "Wood" && hit.transform.parent != null)
            {
                GameObject tmpMaterialParent = hit.transform.parent.parent.gameObject;
                tmpMaterialParent.GetComponent<AudioSource>().Play();
                //for (int i = 0; i < tmpMaterialParent.transform.childCount; i++)
                while (tmpMaterialParent.transform.childCount > 0)
                {
                    GameObject tmpMaterialPart = tmpMaterialParent.transform.GetChild(0).gameObject;
                    Rigidbody tmpRB = tmpMaterialPart.AddComponent<Rigidbody>();
                    tmpRB.drag = 1f;
                    tmpRB.angularDrag = 1f;
                    tmpMaterialPart.AddComponent<DestroyTimer>().destructionTime = 30.0f;
                    tmpMaterialPart.layer = 12;
                    //for (int i = 0; i < tmpMaterialPart.transform.childCount; i++)
                    //{
                    //    tmpMaterialPart.transform.GetChild(i).gameObject.layer = 12;
                    //}
                    tmpMaterialPart.transform.GetChild(0).gameObject.layer = 12;
                    tmpMaterialPart.transform.parent = null;
                }
                Destroy(tmpMaterialParent, 2.0f);
            }
            
        }

            Destroy(gameObject);
    }

    // Update is called once per frame
    void Update () {
        GetComponent<Rigidbody>().AddForce(-transform.up * 50.0f);
	}
}
