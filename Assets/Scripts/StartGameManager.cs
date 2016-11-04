using UnityEngine;
using System.Collections;


public class StartGameManager : MonoBehaviour
{
    public GameObject mainCamera;

    public GameObject playerPrefab;
    private GameObject player;
    
    // Use this for initialization
    void Start()
    {
        mainCamera = GameObject.Find("Main Camera");
        spawnPlayer();

    }

    void spawnPlayer()
    {

        Vector3 spawn_pos_player = new Vector3(0.0f, 3f, 0f);

        player = (GameObject)Instantiate(playerPrefab, spawn_pos_player, Quaternion.identity);
        if(player.name == "rb_character_prototype(Clone)" || player.name == "ball_prototype(Clone)")
            mainCamera.GetComponent<PlayerCamera>().setCameraTarget(player.transform.GetChild(0).gameObject); // TODO: Fix ugly fix
        else
            mainCamera.GetComponent<PlayerCamera>().setCameraTarget(player);


    }
    

    // Update is called once per frame
    void Update()
    { 
    

    }

}
