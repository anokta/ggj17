using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class cityBuilder : MonoBehaviour {

    public int xCount = 20;
    public int yCount = 20;

	// Use this for initialization
	void Start () {
        createCity();
    }
	
	// Update is called once per frame
	void Update () {
		
	}

    void createCity()
    {
        for(int i = 0; i < xCount; i++)
        {
            for(int e = 0; e < yCount; e++)
            {
                GameObject Bldg = GameObject.CreatePrimitive(PrimitiveType.Cube);
                float heightRange = i / 30f + .5f;

                float height = Random.Range(.5f, heightRange * 6f);
                Bldg.transform.localScale = new Vector3(1f, height, 1f);
                Bldg.transform.position = new Vector3(2f * i, height / 2f, 2f * e);
            }
        }
    }
}
