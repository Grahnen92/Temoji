﻿using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class startTostory : MonoBehaviour {
	// Use this for initialization
	void Start () {
        
        GameObject btnObj = GameObject.Find("story");
        Button but = btnObj.GetComponent<Button>();
        but.onClick.AddListener(delegate ()
        {
            this.EnterNextScene(btnObj);
        }
            );
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    //void Awake()
    //{
    //    .Get(but).onClick += EnterNextScene;
    //}
    public void EnterNextScene(GameObject go)
    {
       // Debug.Log("Enter next scene");
        Application.LoadLevel("story_screen");
    }
}
