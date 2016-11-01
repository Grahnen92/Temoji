using UnityEngine;
using System.Collections;

public class RUIN
{
    public const float INNER_CHANCE = 0.02f;
    public const float INNER_CHANCE_INCREMENT = 0.3f;

    public const float OUTER_CHANCE = 0.05f;
    public const float OUTER_CHANCE_INCREMENT = 0.9f;

    public const int N_MAX = 20; // Max amount of inner ruins on map

    // IDs for ruin grid
    public const int EMPTY = 0;
    public const int INNER = 1;
    public const int OUTER = 2;
    public static int count;
}

public class GameManager : MonoBehaviour
{
    public GameObject mainCamera;
    public static GameObject groundObject;
    public GameObject targetObject;
    public GameObject playerObject;
    public GameObject enemyObject;
    public GameObject innerRuinObject;
    public GameObject outerRuinObject;
    public GameObject indestructObject;
    public GameObject rock1Object;
    public GameObject rock2Object;
    public GameObject treeObject;
    public GameObject gateObject;
    public ParticleSystem airParticles;
    public static bool base_alive = false;

    private GameObject thePlayer;
    private ParticleSystem theAirParticles;

    private int vertX;
    private int vertY;



    const float MAP_SIZE = 100.0f;
    const int ENTRY_SIZE = 1;
    float base_radius = 5;
    // Use this for initialization
    void Start()
    {
        mainCamera = GameObject.Find("Main Camera");
        theAirParticles = airParticles.GetComponent<ParticleSystem>();
        generateMap();

        spawnPlayer();


        print("Welcome to this level.");

        InvokeRepeating("spawnEnemy", 0, 10.0f);
    }

    void spawnPlayer()
    {

        Vector3 spawn_pos_player = NavigationRoll.target_destination + new Vector3(3.0f, 3f, 0f);

        thePlayer = (GameObject)Instantiate(playerObject, spawn_pos_player, Quaternion.identity);
        mainCamera.GetComponent<PlayerCamera>().setCameraTarget(thePlayer);

    }

    void spawnEnemy()
    {
        Quaternion rotation = Quaternion.identity;
        rotation.SetLookRotation(NavigationRoll.spawn_destination.normalized);

        GameObject newEnemy = (GameObject)Instantiate(enemyObject, NavigationRoll.spawn_destination + new Vector3(0,1,0), rotation);
    }

    // Update is called once per frame
    void Update()
    {
        if (!base_alive)
        {
            CancelInvoke();
        }
        theAirParticles.transform.position = GameObject.Find("final_prototype_neckjoint").transform.position + new Vector3(0,2,0);

    }

    public Vector3 initialBase(GameObject BasePrefab, float BaseSize)
    {

        float allowed_spawn_area = 0.8f; // 80%

        float x = Random.Range((MAP_SIZE / 2 - BaseSize / 2) * -1, (MAP_SIZE / 2 - BaseSize / 2)) * allowed_spawn_area;
        float z = Random.Range((MAP_SIZE / 2 - BaseSize / 2) * -1, (MAP_SIZE / 2 - BaseSize / 2)) * allowed_spawn_area;
        Vector3 position = new Vector3(x, BaseSize / 2, z);
        GameObject hej = (GameObject)Instantiate(BasePrefab, position, Quaternion.identity);
        base_alive = true;
        NavigationRoll.setBase(hej);
        return position;
    }

    public Vector3 initialEntry(GameObject EntryPrefab, Vector3 basePosition, float r, float EntrySize)
    {
        // Choose x or z
        Vector3 gatePos = new Vector3();
        bool first = true;
        int safe_counter = 0;
        while ((gatePos - basePosition).magnitude < r || first || safe_counter > 1000)
        {
            safe_counter++;
            first = false;
            float rand = Random.value;
            if (rand < .5f)
            {
                float rand_inner = Random.value;
                int sign = 1;
                if (rand_inner < .5f) { sign = -1; };

                gatePos.x = sign * (MAP_SIZE / 2 - EntrySize / 2);
                gatePos.z = (Random.value - .5f) * MAP_SIZE;
            }
            else
            {
                float rand_inner = Random.value;
                int sign = 1;
                if (rand_inner < .5f) sign = -1;
                gatePos.z = sign * (MAP_SIZE / 2 - EntrySize / 2);
                gatePos.x = (Random.value - .5f) * MAP_SIZE;
            }

            gatePos.y = EntrySize / 2;
        }

        if(safe_counter > 1000)
        {
            print("Something wrong :(");
        }

        if ((gatePos - basePosition).magnitude > r)
        {
            // rotate gate if placed on the x-axis
            Quaternion rotation = Quaternion.identity;
            if (Mathf.Abs(gatePos.z) > Mathf.Abs(gatePos.x))
            {
                rotation.SetLookRotation(new Vector3(1, 0, 0));
            }

            Vector3 offset = new Vector3(0.0f, 1.8f, 0.0f);
            Instantiate(EntryPrefab, gatePos + offset, rotation);
        }

        return gatePos;

    }

    void generateMap()
    {

        // Generate Base
        NavigationRoll.target_destination = initialBase(targetObject, 1);

        // Generate Gate
        NavigationRoll.spawn_destination = initialEntry(gateObject, NavigationRoll.target_destination, base_radius, ENTRY_SIZE);
        
        // Generate Collectables
        generateCollectibles();


    }


    /*------------------------------------------------
    In this function collectables objects are generated
    These objects can be destroyed
    ------------------------------------------------   */
    void generateCollectibles()
    {
        SurfaceCreator mountains = GameObject.Find("Mountains").GetComponent<SurfaceCreator>();
        int resolution = mountains.resolution;
        int sqrdResolution = resolution * resolution;

        int minSpread = 10;
        int maxSpread = 30;

        for (vertX = 0; vertX < resolution; vertX += Random.Range(minSpread, maxSpread))
        {

            for (vertY = 0; vertY < resolution; vertY += Random.Range(minSpread, maxSpread))
            {
                int v = vertX + vertY * resolution;
                Vector3 position = mountains.getVertex(v) * 150;
                position.y += 10;
                Vector3 normal = mountains.getNormal(v);

                if(position.y > 0)
                {
                    Quaternion rotation = Quaternion.identity;
                    GameObject collectible;

                    int chancePercentage = Random.Range(0, 100);

                    if(chancePercentage < 40){
                        int maxRotate = 8;
                        //rotation *= Quaternion.Euler(Random.Range(-maxRotate, maxRotate), Random.Range(0, 360), Random.Range(-maxRotate, maxRotate));

                        collectible = (GameObject)Instantiate(rock1Object, position, Quaternion.identity);
                        
                        collectible.transform.rotation = Quaternion.FromToRotation(collectible.transform.up, normal) * collectible.transform.rotation;
                        collectible.transform.Rotate(Random.Range(-maxRotate, maxRotate), Random.Range(0, 360), Random.Range(-maxRotate, maxRotate));

                        float scale = Random.Range(0.8f, 1.2f);
                        collectible.transform.localScale *= scale;
                    }
                    else
                    {
                        int maxRotate = 8;
                        rotation *= Quaternion.Euler(Random.Range(-maxRotate, maxRotate), Random.Range(0, 360), Random.Range(-maxRotate, maxRotate));

                        collectible = (GameObject)Instantiate(treeObject, position, rotation);

                        float scale = Random.Range(0.8f, 1.2f);
                        collectible.transform.localScale *= scale;
                    }
                }

                    
    
                
            }

        }
        //Instantiate(treeObject, position, Quaternion.identity);
        //Quaternion rotation = Quaternion.identity;
        //rotation.y = Random.value * 6.3f;
        //Instantiate(rock1Object, position, rotation);
        
    }

}
