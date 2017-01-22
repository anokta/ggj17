using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapMaker : MonoBehaviour {

	public Texture2D thisMap;
	public GameObject buildingPrefab;
	public GameObject roadPrefab;
	public Color roadColor;
	public Color goodBldg;
	public Color evilBldg;
	public Color neutralBldg;


	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	void readPrefab (Texture2D map){
		int width = map.width;
		int height = map.height;

		for (int x = 0; x < width; x++) {
			for (int y = 0; y < height; y++) {
				//switch (map.GetPixel (x, y)) {
				//}
			}
		}
	}

	void createRoad (int x, int z){
		//GameObject road = GameObject.Instantiate (roadPrefab, new Vector3 (x, 0f, z)) as GameObject;
	}
}
