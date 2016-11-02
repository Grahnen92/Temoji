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
    public GameObject groundPrefab;
    public GameObject BasePrefab;
    private GameObject Base;

    public GameObject playerPrefab;
    private GameObject player;
    public GameObject enemy1Prefab;
    public GameObject enemy2Prefab;
    public GameObject rock1Prefab;
    public GameObject treePrefab;
    public GameObject gatePrefab;
    private GameObject Gate;
    public ParticleSystem airParticles;
    public static bool base_alive = false;

    
    private ParticleSystem theAirParticles;

    private int vertX;
    private int vertY;

    private int groupSpawnInterval = 40;
    private int enemiesPerGroup = 5;
    private int enemyGroupCounter = 0;

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

        InvokeRepeating("spawnEnemyGroup", 0, 40.0f);
        
    }

    void spawnPlayer()
    {

        Vector3 spawn_pos_player = Base.transform.position + new Vector3(5.0f, 3f, 0f);

        player = (GameObject)Instantiate(playerPrefab, spawn_pos_player, Quaternion.identity);
        if(player.name == "rb_character_prototype(Clone)")
            mainCamera.GetComponent<PlayerCamera>().setCameraTarget(player.transform.GetChild(0).gameObject); // TODO: Fix ugly fix
        else
            mainCamera.GetComponent<PlayerCamera>().setCameraTarget(player);
    }

    void spawnEnemyGroup()
    {
            InvokeRepeating("spawnEnemy", 0, 1.0f);
    }

    void spawnEnemy()
    {
        Quaternion rotation = Quaternion.identity;

        float tmpChance = Random.Range(0, 100);
        GameObject newEnemy;


        if (tmpChance >40)
        {
            newEnemy = (GameObject)Instantiate(enemy1Prefab, Gate.transform.position + new Vector3(0, 0, 0), rotation);
            newEnemy.GetComponentInChildren<NavigationRoll>().setBase(Base, Gate);
        }
        else
        {
            newEnemy = (GameObject)Instantiate(enemy2Prefab, Gate.transform.position + new Vector3(0, 0, 0), rotation);
            newEnemy.GetComponentInChildren<NavigationHover>().setBase(Base, Gate);
        }

        enemyGroupCounter++;
        if (enemyGroupCounter >= enemiesPerGroup)
        {
            CancelInvoke("spawnEnemy");
            enemyGroupCounter = 0;
        }
        

    }

    // Update is called once per frame
    void Update()
    {
        if (!base_alive)
        {
            CancelInvoke();
        }
        theAirParticles.transform.position = player.transform.position + new Vector3(0,2,0);

    }

    private void initialBase(float BaseSize)
    {

        float allowed_spawn_area = 0.8f; // 80%

        float x = Random.Range((MAP_SIZE / 2 - BaseSize / 2) * -1, (MAP_SIZE / 2 - BaseSize / 2)) * allowed_spawn_area;
        float z = Random.Range((MAP_SIZE / 2 - BaseSize / 2) * -1, (MAP_SIZE / 2 - BaseSize / 2)) * allowed_spawn_area;
        Vector3 BasePosition = new Vector3(x, BaseSize, z);
        Base = (GameObject)Instantiate(BasePrefab, BasePosition, Quaternion.identity);
        base_alive = true;
        //NavigationRoll.setBase(hej);
        //NavigationHover.setBase(hej);

        print("target_destination in initial" + BasePosition);
    }

    private void initialEntry(float r, float EntrySize)
    {
        // Choose x or z
        Vector3 gatePos = new Vector3();
        bool first = true;
        int safe_counter = 0;
        while ((gatePos - Base.transform.position).magnitude < r || first || safe_counter > 1000)
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
            return;
            print("Something wrong :(");
        }

        if ((gatePos - Base.transform.position).magnitude > r)
        {
            // rotate gate if placed on the x-axis
            Quaternion rotation = Quaternion.identity;
            if (Mathf.Abs(gatePos.z) > Mathf.Abs(gatePos.x))
            {
                rotation.SetLookRotation(new Vector3(1, 0, 0));
            }

            Vector3 offset = new Vector3(0.0f, 1.8f, 0.0f);
            Gate = Instantiate(gatePrefab, gatePos + offset, rotation) as GameObject;
        }

    }

    void generateMap()
    {

        // Generate Base
        initialBase(1);

        // Generate Gate
        initialEntry( base_radius, ENTRY_SIZE);
        
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

                        collectible = (GameObject)Instantiate(rock1Prefab, position, Quaternion.identity);
                        
                        collectible.transform.rotation = Quaternion.FromToRotation(collectible.transform.up, normal) * collectible.transform.rotation;
                        collectible.transform.Rotate(Random.Range(-maxRotate, maxRotate), Random.Range(0, 360), Random.Range(-maxRotate, maxRotate));

                        float scale = Random.Range(0.8f, 1.2f);
                        collectible.transform.localScale *= scale;
                    }
                    else
                    {
                        int maxRotate = 8;
                        rotation *= Quaternion.Euler(Random.Range(-maxRotate, maxRotate), Random.Range(0, 360), Random.Range(-maxRotate, maxRotate));

                        collectible = (GameObject)Instantiate(treePrefab, position, rotation);

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
