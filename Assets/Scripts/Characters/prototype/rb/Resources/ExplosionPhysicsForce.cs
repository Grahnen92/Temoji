using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UnityStandardAssets.Effects
{
    public class ExplosionPhysicsForce : MonoBehaviour
    {
        public float explosionForce = 4;
        public LayerMask mask = (1 << 10) | ( 1 << 13) | ( 1 << 14 );

        public float maxDamage = 10.0f;
        private float x;


        private IEnumerator Start()
        {
            x = maxDamage * 0.2f;
            // wait one frame because some explosions instantiate debris which should then
            // be pushed by physics force
            yield return null;

            float multiplier = GetComponent<ParticleSystemMultiplier>().multiplier;

            float r = 10*multiplier;
            var cols = Physics.OverlapSphere(transform.position, r, mask);
            var rigidbodies = new List<Rigidbody>();
            foreach (var col in cols)
            {
                if (col.attachedRigidbody != null && !rigidbodies.Contains(col.attachedRigidbody))
                {
                    rigidbodies.Add(col.attachedRigidbody);
                }
            }
            foreach (var rb in rigidbodies)
            {
                rb.AddExplosionForce(explosionForce*multiplier, transform.position, r, 1*multiplier, ForceMode.Impulse);
                var dist = Vector3.Distance(transform.position, rb.transform.position);

                

                float dmgScaled = Mathf.Max((dist / r), 0.2f);
                dmgScaled = dmgScaled * dmgScaled;

               if(rb.gameObject.tag == "Player")
                {
                    print("============");
                    print("dist = " + dist);
                    print("radius = " + r);
                    print("dmg = " + (3.0f / dmgScaled));
                    print("tag = " + rb.gameObject.tag);
                    print("============");
                }

               // rb.gameObject.GetComponent<Combat>().TakeDamage((int)(3.0f/ dmgScaled));
                Combat tmpCombatScript = rb.gameObject.GetComponent<Combat>();
               
                if (tmpCombatScript)
                {
                    tmpCombatScript.TakeDamage((int)(x / dmgScaled));
                }
              
                //rb.gameObject.GetComponent<Combat>().TakeDamage(50);

            }
        }
    }
}
