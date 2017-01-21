using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CubeFracture : MonoBehaviour {
    public GameObject cubeFragments;
	// Use this for initialization

	// Update is called once per frame
	void Update () {
        if (Input.GetKeyDown("space"))
        {
            Instantiate(cubeFragments, transform.position, transform.rotation);
            Destroy(gameObject);
        }
	}
}
