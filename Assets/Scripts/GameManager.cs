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


    const float MAP_SIZE = 100.0f;
    const int ENTRY_SIZE = 1;
    float base_radius = 5;
    // Use this for initialization
    void Start()
    {

        theAirParticles = airParticles.GetComponent<ParticleSystem>();
        generateMap();

        spawnPlayer();


        print("Welcome to this level.");

        InvokeRepeating("spawnEnemy", 0, 10.0f);
    }

    void spawnPlayer()
    {

        Vector3 spawn_pos_player = NavigationRoll.target_destination + new Vector3(1.5f, 3f, 0f);

        thePlayer = (GameObject)Instantiate(playerObject, spawn_pos_player, Quaternion.identity);

    }

    void spawnEnemy()
    {
        Instantiate(enemyObject, NavigationRoll.spawn_destination + new Vector3(0,1,0), Quaternion.identity);
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
        
        // Generate Indestructables
        ///generateIndestructables();

        // Generate Collectables
        generateCollectibles();

        // Generate Environment
        //generateEnvironment();

        // Scale Ground
        //groundObject.transform.localScale = new Vector3(MAP_SIZE/10.0f, MAP_SIZE/10.0f, MAP_SIZE/10.0f);
    }


    /*------------------------------------------------
    In this function indestructable objects are generated
     ------------------------------------------------   */
    void generateIndestructables()
    {

        // Ruin grid
        const int N_CELLS = 10;
        const float CELL_SIZE = MAP_SIZE / N_CELLS;
        int[,] ruinGrid = new int[N_CELLS, N_CELLS];
        for (int x_cell = 0; x_cell < N_CELLS; x_cell++)
        {
            for (int z_cell = 0; z_cell < N_CELLS; z_cell++)
            {
                // Generate peice of inner ruin
                float build_chance = Random.value;

                if (x_cell > 0 && x_cell < N_CELLS - 1 && z_cell > 0 && z_cell < N_CELLS - 1)
                {
                    if (ruinGrid[x_cell - 1, z_cell] == RUIN.INNER)
                    {
                        build_chance += RUIN.INNER_CHANCE_INCREMENT;
                    }
                    if (ruinGrid[x_cell + 1, z_cell] == RUIN.INNER)
                    {
                        build_chance += RUIN.INNER_CHANCE_INCREMENT;
                    }
                    if (ruinGrid[x_cell, z_cell - 1] == RUIN.INNER)
                    {
                        build_chance += RUIN.INNER_CHANCE_INCREMENT;
                    }
                    if (ruinGrid[x_cell, z_cell + 1] == RUIN.INNER)
                    {
                        build_chance += RUIN.INNER_CHANCE_INCREMENT;
                    }
                }

                if (build_chance > (1.0 - RUIN.INNER_CHANCE) && RUIN.count < RUIN.N_MAX)
                {
                    Vector3 position = new Vector3();
                    position.x = (x_cell + 0.5f) * CELL_SIZE - MAP_SIZE / 2;
                    position.y = 0;
                    position.z = (z_cell + 0.5f) * CELL_SIZE - MAP_SIZE / 2;
                    innerRuinObject.transform.localScale = new Vector3(60f, 60f, 60f);
                    Instantiate(innerRuinObject, position, Quaternion.identity);
                    ruinGrid[x_cell, z_cell] = RUIN.INNER;
                    RUIN.count++;
                }
                else
                {
                    ruinGrid[x_cell, z_cell] = RUIN.EMPTY;
                }


            }
        }


        for (int x_cell = 0; x_cell < N_CELLS; x_cell++)
        {
            for (int z_cell = 0; z_cell < N_CELLS; z_cell++)
            {
                // Generate peice of inner ruin
                float build_chance = Random.value;

                if (x_cell > 0 && x_cell < N_CELLS - 1 && z_cell > 0 && z_cell < N_CELLS - 1)
                {
                    if (ruinGrid[x_cell - 1, z_cell] == RUIN.INNER)
                    {
                        build_chance += RUIN.OUTER_CHANCE_INCREMENT;
                    }
                    if (ruinGrid[x_cell + 1, z_cell] == RUIN.INNER)
                    {
                        build_chance += RUIN.OUTER_CHANCE_INCREMENT;
                    }
                    if (ruinGrid[x_cell, z_cell - 1] == RUIN.INNER)
                    {
                        build_chance += RUIN.OUTER_CHANCE_INCREMENT;
                    }
                    if (ruinGrid[x_cell, z_cell + 1] == RUIN.INNER)
                    {
                        build_chance += RUIN.OUTER_CHANCE_INCREMENT;
                    }
                }

                if (build_chance > (1.0 - RUIN.OUTER_CHANCE) && RUIN.count < RUIN.N_MAX && ruinGrid[x_cell, z_cell] == RUIN.EMPTY)
                {
                    Vector3 position = new Vector3();
                    position.x = (x_cell + 0.5f) * CELL_SIZE - MAP_SIZE / 2;
                    position.y = 0;
                    position.z = (z_cell + 0.5f) * CELL_SIZE - MAP_SIZE / 2;
                    outerRuinObject.transform.localScale = new Vector3(60f, 60f, 60f);
                    Instantiate(outerRuinObject, position, Quaternion.identity);
                    ruinGrid[x_cell, z_cell] = RUIN.OUTER;
                    RUIN.count++;
                }
                else
                {
                    ruinGrid[x_cell, z_cell] = RUIN.EMPTY;
                }


            }
        }

        // Random Indestructables
        //for (int y = 0; y < N_CELLS; y++)
        {
            Vector3 position = new Vector3(); // game objects position
            position.x = (Random.value - 0.5f) * MAP_SIZE * 0.9f;
            position.z = (Random.value - 0.5f) * MAP_SIZE * 0.9f;
            position.y = 0f;

            Vector3 coll_sphere_position = new Vector3(); // Collision sphere position
            coll_sphere_position = position;
            coll_sphere_position.y += .4f;
            indestructObject.transform.localScale = new Vector3(.4f, .4f, .4f);

            Collider[] hitColliders = Physics.OverlapSphere(coll_sphere_position, 0.30f);
            int i = 0;
            bool build = true;
            while (i < hitColliders.Length) // Check collision
            {
                i++;
                build = false;
            }

            if (build) // Build if no collide
            {
                Instantiate(indestructObject, position, Quaternion.identity);
            }
        }
    }

    /*------------------------------------------------
    In this function collectables objects are generated
    These objects can be destroyed
    ------------------------------------------------   */
    void generateCollectibles()
    {

        // Trees
        for (int y = 0; y < 10; y++)
        {
            Vector3 position = targetObject.transform.position; // game objects position
            while ((targetObject.transform.position - position).magnitude < 2.0f)
            {
                position.x = (Random.value - 0.5f) * MAP_SIZE * 0.9f;
                position.z = (Random.value - 0.5f) * MAP_SIZE * 0.9f;
                position.y = 0f;
            }

            Vector3 coll_sphere_position = new Vector3(); // Collision sphere position
            coll_sphere_position = position;


            coll_sphere_position.y += .4f;
            treeObject.transform.localScale = new Vector3(30f, 40f, 30f);

            Collider[] hitColliders = Physics.OverlapSphere(coll_sphere_position, 0.35f);
            int i = 0;
            bool build = true;
            while (i < hitColliders.Length) // Check collision
            {
                i++;
                build = false;
            }

            if (build) // Build if no collide
            {
                Instantiate(treeObject, position, Quaternion.identity);
            }
        }

        // Rocks
        for (int y = 0; y < 20; y++)
        {
            Vector3 position = targetObject.transform.position; // game objects position
            while ((targetObject.transform.position - position).magnitude < 2.0f)
            {
                position.x = (Random.value - 0.5f) * MAP_SIZE * 0.9f;
                position.z = (Random.value - 0.5f) * MAP_SIZE * 0.9f;
                position.y = 0f;
            }


            Vector3 coll_sphere_position = new Vector3(); // Collision sphere position
            coll_sphere_position = position;


            coll_sphere_position.y += .4f;
            rock1Object.transform.localScale = new Vector3(10f, 10f, 10f);
            Collider[] hitColliders = Physics.OverlapSphere(coll_sphere_position, 0.35f);
            int i = 0;
            bool build = true;
            while (i < hitColliders.Length) // Check collision
            {
                i++;
                build = false;
            }

            if (build) // Build if no collide
            {
                Quaternion rotation = Quaternion.identity;
                rotation.y = Random.value * 6.3f;
                Instantiate(rock1Object, position, rotation);
            }
        }
    }


    /*------------------------------------------------
    In this function environment objects are generated
    Enemies and players can move through these objects
    ------------------------------------------------   */
    void generateEnvironment()
    {

    }
}
