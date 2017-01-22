using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CityBuilder : MonoBehaviour {
  public AudioManager audioManager;

  // Building prefab.
  public GameObject buildingPrefab;
  public GameObject roadStraightPrefab;
  public GameObject road4wayPrefab;
  public Material[] buildingMaterials = new Material[0];
 
  // Number of buildings per axis.
  public int xCount = 10;
  public int yCount = 10;

  // Height range.
  public float minHeight = 0.5f;
  public float maxHeight = 2.0f;

  public float maxBuildingSpeed = 2.0f;

  public FracturePool fracturePool;

  // Root game object to hold buildings.
  private GameObject cityRoot = null;

  private Rigidbody[] buildings = null;

  private float speedThreshold;

  void Awake () {
    buildings = new Rigidbody[xCount * yCount];
    speedThreshold = Mathf.Pow(maxBuildingSpeed, 2.0f);
  }

  void Update () {
    for (int i = 0; i < buildings.Length; ++i) {
      if (buildings[i] != null && buildings[i].velocity.sqrMagnitude > speedThreshold) {
        audioManager.DestroyBuildingSfx();
        int numFractures = Mathf.CeilToInt(buildings[i].transform.localScale.y);
        for (int f = 0; f < numFractures; ++f) {
          FractureController fracture = fracturePool.GetNextFracture();
          fracture.transform.position = buildings[i].transform.position;
          fracture.gameObject.SetActive(true);
          GameObject.Destroy(buildings[i].gameObject);
        }
      }
    }
  }

  public void GenerateCity () {
    if (cityRoot != null) {
      GameObject.Destroy(cityRoot);
    }
    cityRoot = new GameObject("City");

    int index = 0;
    for (int x = 0; x < xCount; ++x) {
      for (int y = 0; y < yCount; ++y) {
        Vector3 position = new Vector3(x - xCount, .05f, y - yCount);

        //Every fourth tile is a road
        if (x % 4 == 0) {
          //Every third tile is a cross street, so make it a 4 way;
          if (y % 3 == 0) {
            GameObject roadTile = 
              (GameObject) GameObject.Instantiate(road4wayPrefab, position, Quaternion.identity, 
                                                  cityRoot.transform);

          } else {
            //Since it's not a cross street, make it a standard street;
            GameObject roadTile = 
              (GameObject) GameObject.Instantiate(roadStraightPrefab, position, Quaternion.identity, 
                                                  cityRoot.transform);
            roadTile.transform.localEulerAngles = new Vector3(0f, 90f, 0f);
          }
        } else {
          if (y % 3 == 0) {
            //In this case, it's a street
            GameObject roadTile = 
              (GameObject) GameObject.Instantiate(roadStraightPrefab, position, Quaternion.identity, 
                                                  cityRoot.transform);
          } else {
            //Not a street, and not a building. Let's build!
            float height = Random.Range(minHeight, maxHeight);
            float width = Random.Range(.7f, .9f);
            float length = Random.Range(.7f, .9f);

            position.y = height / 2f;
            GameObject building = 
              (GameObject) GameObject.Instantiate(buildingPrefab, position, Quaternion.identity, 
                                                  cityRoot.transform);
            building.transform.localScale = new Vector3(width, length, height);
            building.transform.localEulerAngles = new Vector3(-90f, 0f, 0f);
            building.GetComponent<Renderer>().material = 
              buildingMaterials[Random.Range(0, buildingMaterials.Length)];
            buildings[index++] = building.GetComponent<Rigidbody>();
          }
        }
      }
    }
  }
}
