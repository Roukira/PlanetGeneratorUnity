using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainGame : MonoBehaviour {

    public GameObject planetGameObject;
    public Planet planet;
    public UIManager uiManager;
    public CameraScript mainCamera;
    public MaterialManager materialManager;

	// Use this for initialization
	void Start () {

        planetGameObject = new GameObject("Planet");
        planet = planetGameObject.AddComponent<Planet>();
        planet.Initialize();
        uiManager.currentPlanet = planet;
        mainCamera.target = planetGameObject;
        uiManager.LoadSettings();
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
