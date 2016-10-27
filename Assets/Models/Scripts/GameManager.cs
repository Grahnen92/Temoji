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

    public GameObject groundObject;
    public GameObject targetObject;
    public GameObject playerObject;
    public GameObject enemyObject;
    public GameObject innerRuinObject;
    public GameObject outerRuinObject;
    public GameObject indestructObject;
    public GameObject treeObject;
    public GameObject gateObject;




    const int MAP_SIZE = 10;
    const int ENTRY_SIZE = 1;
    float base_radius = 5;
    // Use this for initialization
    void Start()
    {
        Vector3 spawn_pos_player = new Vector3(1f, 3f, 0f);

        NavigationScript.target_destination = initialBase(targetObject, 1);
        NavigationScript.spawn_destination = initialEntry(gateObject, NavigationScript.target_destination, base_radius, ENTRY_SIZE);
        generateMap();

        Instantiate(playerObject, spawn_pos_player, Quaternion.identity);

        print("Welcome to this level.");

        InvokeRepeating("spawnEnemy", 0, 5.0f);
    }

    void spawnEnemy()
    {
        Instantiate(enemyObject, NavigationScript.spawn_destination, Quaternion.identity);
        //Instantiate(enemyObject, NavigationScript.target_destination, Quaternion.identity);

    }

    // Update is called once per frame
    void Update()
    {

    }

    public Vector3 initialBase(GameObject BasePrefab, float BaseSize)
    {
        Vector3 position = new Vector3(Random.Range((MAP_SIZE / 2 - BaseSize / 2) * -1, (MAP_SIZE / 2 - BaseSize / 2)), BaseSize / 2, Random.Range((MAP_SIZE / 2 - BaseSize / 2) * -1, (MAP_SIZE / 2 - BaseSize / 2)));
        Instantiate(BasePrefab, position, Quaternion.identity);
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

        //Vector3 currPosition = new Vector3(Random.Range((MAP_SIZE / 2 - EntrySize / 2) * -1, (MAP_SIZE / 2 - EntrySize / 2)), EntrySize / 2, Random.Range((MAP_SIZE / 2 - EntrySize / 2) * -1, (MAP_SIZE / 2 - EntrySize / 2)));
        if ((gatePos - basePosition).magnitude > r)
        {
            Instantiate(EntryPrefab, gatePos, Quaternion.identity);


        }

        return gatePos;

    }

    void generateMap()
    {
        // Generate Base
        generateBase();

        // Generate Gate
        generateGate();

        // Generate Indestructables
        generateIndestructables();



        // Generate Collectables
        generateCollectibles();

        // Generate Environment

        generateEnvironment();

        // Generate Ground
        groundObject = GameObject.CreatePrimitive(PrimitiveType.Plane);
    }

    void generateBase()
    {

    }

    void generateGate()
    {

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
                    innerRuinObject.transform.localScale = new Vector3(.6f, .6f, .6f);
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
                    outerRuinObject.transform.localScale = new Vector3(.4f, .4f, .4f);
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
        for (int y = 0; y < N_CELLS; y++)
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
        for (int y = 0; y < 12; y++)
        {
            Vector3 position = new Vector3(); // game objects position
            position.x = (Random.value - 0.5f) * MAP_SIZE * 0.9f;
            position.z = (Random.value - 0.5f) * MAP_SIZE * 0.9f;
            position.y = 0f;

            Vector3 coll_sphere_position = new Vector3(); // Collision sphere position
            coll_sphere_position = position;
            coll_sphere_position.y += .4f;
            treeObject.transform.localScale = new Vector3(.4f, .4f, .4f);

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
                Instantiate(treeObject, position, Quaternion.identity);
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
