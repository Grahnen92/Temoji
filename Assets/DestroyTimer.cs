using UnityEngine;
using System.Collections;

public class DestroyTimer : MonoBehaviour {

    // Use this for initialization
    private float timer;
    public float destructionTime = 1.0f;
    void Start() {
        timer = 0.0f;
    }
    // Update is called once per frame
    void Update()
    {
        timer += Time.deltaTime;

        if (timer > destructionTime)
        {
            Destroy(gameObject);
        }

    }

    public void setDestructionTime(float _time)
    {
        destructionTime = _time;
    }
}
