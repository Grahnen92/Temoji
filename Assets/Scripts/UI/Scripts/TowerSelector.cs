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
    private const int nrOfModels = 3;
    public GameObject model0;
    public GameObject model1;
    public string tower_resource1 = "Towers\\ProjectileTower\\projectile_tower_base";
    public GameObject model2;
    public string tower_resource2 = "Towers\\SlowingTower\\slowing_tower_base";
    public GameObject uiRing;

    // Key to press that will show the holograms
    public KeyCode startBuildKey = KeyCode.F;

    // Key to press for selecting towers
    public KeyCode tower1Key = KeyCode.Alpha1;
    public KeyCode tower2Key = KeyCode.Alpha2;
    public KeyCode tower3key = KeyCode.Alpha3;

    // Set transparency of models NOTE: Material must support transparency
    public float transparency = 0.3f;

    // Lists for storing all models and instantiated towers
    List<GameObject> modelList = new List<GameObject>();
    List<GameObject> modInstances = new List<GameObject>();
    private int[] activeModels = new int[nrOfModels];
    GameObject uiRingInstance;

    // True when showing hologram
    bool showing = false;

    // Index of currently selected tower
    // This attribute is public to let other classes know which tower is currently selected.
    public int selectedTower = 0;

    //Materials
    private List<GameObject> wood = new List<GameObject>();
    private List<GameObject> stone = new List<GameObject>();
    private List<GameObject> energy = new List<GameObject>();

    public GameObject tower_builder;
    private Vector3 tmp_pos;

    void OnTriggerEnter(Collider col)
    {
        if (col.tag == "Stone")
        {
            stone.Add(col.gameObject);

            //var renderers = modInstances[0].GetComponentsInChildren<Renderer>();
            //Color tmp_color;
            //foreach (var r in renderers)
            //{
            //    tmp_color = r.material.color;
            //    tmp_color.a = transparency;
            //}
            print("hi stone");
        }
        else if (col.tag == "Wood")
        {
            wood.Add(col.gameObject);
            print("hi wood");
        }
        else
        {
            energy.Add(col.gameObject);
        }

        checkActiveTowersAfterAdd();


    }
    void OnTriggerExit(Collider col)
    {
        if (col.tag == "Stone")
        {
            stone.Remove(col.gameObject);
            print("by stone");
        }
        else if (col.tag == "Wood")
        {
            wood.Remove(col.gameObject);
            print("by Wood");
        }
        else
        {
            energy.Remove(col.gameObject);
        }

        checkActiveTowersAfterRemove();
    }

    void Start () {
        // Add additional lines here if adding additional models

        for (int i = 0; i < nrOfModels; i++)
        {
            activeModels[i] = 0;
        }

        uiRingInstance =  Instantiate(uiRing);
        uiRingInstance.transform.parent = transform;
        uiRingInstance.transform.position = spawnPt.position;
        uiRingInstance.transform.localScale = new Vector3(7.8f, 7.8f, 7.8f);
        Renderer tmp_renderer = uiRingInstance.GetComponent<Renderer>();
        tmp_renderer.enabled = false;
        Color tmp_color = tmp_renderer.material.color;
        tmp_color.a = transparency;
        uiRingInstance.GetComponent<Renderer>().material.color = tmp_color;


        modelList.Add(model0);
        modelList.Add(model1);
        modelList.Add(model2);

        for(int i = 0; i < modelList.Count; i++)
        {
            GameObject modInst = Instantiate(modelList[i]);
            modInstances.Add(modInst);
            modInst.transform.parent = this.transform;
            Vector3 tmpVec = new Vector3(Mathf.Cos((2.0f*Mathf.PI / modelList.Count) * i), 0.0f, Mathf.Sin((2.0f * Mathf.PI / modelList.Count) * i)) * 2;
            modInst.transform.position = spawnPt.position + tmpVec;

            print(tmpVec);
            print((Mathf.PI / modelList.Count) * i);

            // Making object invisible and transparent
            var renderers = modInst.GetComponentsInChildren<Renderer>();
            foreach (var r in renderers)
            {
                r.enabled = false;

                // Making the object transparent NOTE: Material for model must support transparency
                Color c = r.material.color;
                //c.a = transparency;
                c.a = 0.0f;
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
        else if (Input.GetKeyUp(startBuildKey))
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
                // switchModel(0);
                if (activeModels[1] > 0)
                {
                    GameObject tmpGO = wood[0];
                    //tmp_pos = tmpGO.transform.position;
                    wood.RemoveAt(0);
                    Destroy(tmpGO);

                    tmpGO = stone[0];
                    //tmp_pos += tmpGO.transform.position;
                    stone.RemoveAt(0);
                    Destroy(tmpGO);
                    print("starting to build tower");
                    GameObject towerBuilderInstance = Instantiate(tower_builder) as GameObject;
                    //tmp_pos.y = 2.0f;
                    //tmp_pos = tmp_pos / 2.0f;
                    towerBuilderInstance.transform.position = transform.position + Vector3.up;
                    towerBuilderInstance.GetComponent<TowerBuilder>().loadTower(tower_resource1);

                    checkActiveTowersAfterRemove();

                }
            }
            else if (Input.GetKeyDown(tower2Key))
            {
                // switchModel(0);
                if (activeModels[2] > 0)
                {

                    GameObject tmpGO = stone[0];
                    //tmp_pos += tmpGO.transform.position;
                    stone.RemoveAt(0);
                    Destroy(tmpGO);
                    print("starting to build tower");
                    GameObject towerBuilderInstance = Instantiate(tower_builder) as GameObject;
                    //tmp_pos.y = 2.0f;
                    //tmp_pos = tmp_pos / 2.0f;
                    towerBuilderInstance.transform.position = transform.position + Vector3.up;
                    towerBuilderInstance.GetComponent<TowerBuilder>().loadTower(tower_resource2);

                    checkActiveTowersAfterRemove();

                }
            }
            else if (Input.GetKeyDown(tower3key))
            {
               // switchModel(2);
            }
        }
	}

    void LateUpdate()
    {
        Vector3 tmpAng;
        tmpAng = transform.localEulerAngles;
        //tmpAng.x = 0;
        //tmpAng.y = -90;
        //tmpAng.z = 90;
        transform.localEulerAngles = tmpAng;
    }

    void checkActiveTowersAfterAdd()
    {
        print("hi check add");
        if (wood.Count > 0 && stone.Count > 0)
        {
            if (activeModels[1] == 0)
            {
                showModel(1);
                activeModels[1] = 1;
            }

        }

        if (energy.Count > 0 && stone.Count > 0)
        {
            if (activeModels[0] == 0)
            {
                showModel(0);
                activeModels[0] = 1;
            }

        }
        if (stone.Count > 0)
        {
            if (activeModels[2] == 0)
            {
                showModel(2);
                activeModels[2] = 1;
            }

        }
    }

    void checkActiveTowersAfterRemove()
    {
        print("hi check remove");
        if (wood.Count < 1 || stone.Count < 1)
        {
            if (activeModels[1] == 1)
            {
                hideModel(1);
                activeModels[1] = 0;
            }

        }
        if (energy.Count < 1 || stone.Count < 1)
        {
            if (activeModels[0] == 1)
            {
                hideModel(0);
                activeModels[0] = 0;
            }

        }
        if (stone.Count < 1)
        {
            if (activeModels[2] == 1)
            {
                hideModel(2);
                activeModels[2] = 0;
            }

        }
    }

    // Make the models visible
    void showHolograms()
    {
        uiRingInstance.GetComponent<Renderer>().enabled = true;
        showing = true;
        showModels();
 
    }

    // Make the models invisible
    void hideHolograms()
    {
        uiRingInstance.GetComponent<Renderer>().enabled = false;
        showing = false;
        hideModels();
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
        //var renderers = modInstances[model].GetComponentsInChildren<Renderer>();
        //foreach (var r in renderers)
        //{
        //    r.enabled = true;
        //}
        print("show model" + model);
        var renderers = modInstances[model].GetComponentsInChildren<Renderer>();
        Color tmp_color;
        foreach (var r in renderers)
        {
            tmp_color = r.material.color;
            tmp_color.a = transparency;
            r.material.color = tmp_color;
        }
    }
    void showModels()
    {
        
        foreach (var mod in modInstances)
        {
            var renderers = mod.GetComponentsInChildren<Renderer>();
            foreach (var r in renderers)
            {
                r.enabled = true;
            }
        }
        
    }

    // Hide a model
    void hideModel(int model)
    {
        //var renderers = modInstances[selectedTower].GetComponentsInChildren<Renderer>();
        //foreach (var r in renderers)
        //{
        //    r.enabled = false;
        //}
        var renderers = modInstances[model].GetComponentsInChildren<Renderer>();
        Color tmp_color;
        foreach (var r in renderers)
        {
            tmp_color = r.material.color;
            tmp_color.a = 0;
            r.material.color = tmp_color;
        }
    }

    void hideModels()
    {
        foreach (var mod in modInstances)
        {
            var renderers = mod.GetComponentsInChildren<Renderer>();
            foreach (var r in renderers)
            {
                r.enabled = false;
            }
        }
    }
}
