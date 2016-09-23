using UnityEngine;
using System.Collections;

public class AStarGrid : MonoBehaviour {

    private int gridSize = 10;
    int[,] grid = new int[10, 10];
	// Use this for initialization
	void Start () {
	    
        for(int i = 0; i < gridSize; i++)
        {
            for (int j = 0; j < gridSize; j++)
            {
                // Initialize gridcell
                print(grid[i,j]);


            }
        }


    }
	
	// Update is called once per frame
	void Update () {
	
	}
}
