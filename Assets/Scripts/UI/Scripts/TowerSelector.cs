using UnityEngine;
using System.Collections;
using System.Collections.Generic;


/* *
 * Class for showing the tower holograms in build mode
 * To use:
 *  - Add the HologramSpawn prefab to the character object to let it spawn holograms at the position
 *    that HologramSpawn is at
 *    
 * Configurable fields:
 * 1) Setting the tower models to be used in the hologram (Preferably just the model, without functionality)
 *      - Just add more variables to set more models
 *      - Also remember to add it to the list in Start() function
 *      - And remember to add a key to select it in the Update() function
 * 2) Setting the keys that will trigger the build mode, and also selection of towers
 * 3) Transparency level 
 *      - NOTE: The material for the model must support transparency
 *      - Alternatively, can just register models that are already transparent, and set this transparency to 1.0
 * */

public class TowerSelector : MonoBehaviour {

    // Transform that indicates where hologram will spawn
    public Transform spawnPt;

    // Just the model alone, without any additional components. 
    // Add additional fields as needed
    public GameObject model1;
    public GameObject model2;
    public GameObject model3;

    // Key to press that will show the holograms
    public KeyCode startBuildKey = KeyCode.F;

    // Key to press for selecting towers
    public KeyCode tower1Key = KeyCode.Alpha1;
    public KeyCode tower2Key = KeyCode.Alpha2;
    public KeyCode tower3key = KeyCode.Alpha3;

    // Set transparency of models NOTE: Material must support transparency
    public float transparency = 0.5f;

    // Lists for storing all models and instantiated towers
    List<GameObject> modelList = new List<GameObject>();
    List<GameObject> modInstances = new List<GameObject>();

    // True when showing hologram
    bool showing = false;

    // Index of currently selected tower
    // This attribute is public to let other classes know which tower is currently selected.
    public int selectedTower = 0; 

	void Start () {
        // Add additional lines here if adding additional models
        modelList.Add(model1);
        modelList.Add(model2);
        modelList.Add(model3);

        foreach (GameObject go in modelList)
        {
            GameObject modInst = Instantiate(go);
            modInstances.Add(modInst);
            modInst.transform.parent = this.transform;
            modInst.transform.position = spawnPt.position;

            // Making object invisible and transparent
            var renderers = modInst.GetComponentsInChildren<Renderer>();
            foreach (var r in renderers)
            {
                r.enabled = false;

                // Making the object transparent NOTE: Material for model must support transparency
                Color c = r.material.color;
                c.a = transparency;
                r.material.color = c;
            }
        }
    }
	
	void Update () {
        if (Input.GetKeyDown(startBuildKey))
        {
            if (!showing)
                showHolograms();
            else
                hideHolograms();
        }

        // Checking for keypresses indicating a switch of model
        if (showing)
        {
            if (Input.GetKeyDown(tower1Key))
            {
                switchModel(0);
            }
            else if (Input.GetKeyDown(tower2Key))
            {
                switchModel(1);
            }
            else if (Input.GetKeyDown(tower3key))
            {
                switchModel(2);
            }
        }
	}

    // Make the models visible
    void showHolograms()
    {
        showing = true;
        showModel(selectedTower);
    }

    // Make the models invisible
    void hideHolograms()
    {
        showing = false;
        hideModel(selectedTower);
    }

    // Switch which model is visible
    void switchModel(int toSwitch)
    {
        hideModel(selectedTower);
        showModel(toSwitch);

        selectedTower = toSwitch;
    }

    // Show a model
    void showModel(int model)
    {
        var renderers = modInstances[model].GetComponentsInChildren<Renderer>();
        foreach (var r in renderers)
        {
            r.enabled = true;
        }
    }

    // Hide a model
    void hideModel(int model)
    {
        var renderers = modInstances[selectedTower].GetComponentsInChildren<Renderer>();
        foreach (var r in renderers)
        {
            r.enabled = false;
        }
    }
}
