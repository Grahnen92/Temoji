using UnityEngine;
using System.Collections;

public class BaseChargup : MonoBehaviour {

    public float CHARGE_TIME = 60;
    private float charge_time_interval;
    private float charge_timer;

	// Use this for initialization
	void Start () {
        charge_timer = 0.0f;
        charge_time_interval = CHARGE_TIME / 5.0f;
        var rs = GetComponentsInChildren<Renderer>();
        foreach (var r in rs)
        {
            Color tmp_color = r.material.color;
            tmp_color.a = 0.0f;
            r.material.color = tmp_color;
        }
            
    }
	
	// Update is called once per frame
	void Update () {
        if (charge_timer > CHARGE_TIME)
        {
            print("YOU WON THE GAME!");
            Application.Quit();
        }
        else
        {
            int current_charge_bar = Mathf.FloorToInt(charge_timer / charge_time_interval);
            GameObject current_charge_object = transform.GetChild(current_charge_bar).gameObject;

            var r = current_charge_object.GetComponent<Renderer>();
            Color tmp_color = r.material.color;
            tmp_color.a = (charge_timer - ((float)current_charge_bar) * charge_time_interval) / charge_time_interval;
            r.material.color = tmp_color;
            charge_timer += Time.deltaTime;
        }
            
	}
}
