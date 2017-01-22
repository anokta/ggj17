using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CityBuilder : MonoBehaviour {
  public AudioManager audioManager;

  // Building prefab.
  public GameObject buildingPrefab;
  public GameObject roadStraightPrefab;
  public GameObject road4wayPrefab;
  public GameObject specialBuildingPrefab;
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
  private Rigidbody[] specialBuildings = null;

  private float speedThreshold;

  private int numBuildings;
  private int destroyCount;

  void Awake () {
    buildings = new Rigidbody[xCount * yCount / 2];
    speedThreshold = Mathf.Pow(maxBuildingSpeed, 2.0f);
  }

  void Update () {
    for (int i = 0; i < buildings.Length; ++i) {
      if (buildings[i] != null && buildings[i].velocity.sqrMagnitude > speedThreshold) {
        DestroyBuilding(buildings[i], false);
        ++destroyCount;
      }
    }
    if (specialBuildings != null) {
      for (int i = 0; i < specialBuildings.Length; ++i) {
        if (specialBuildings[i] != null &&
            specialBuildings[i].velocity.sqrMagnitude > 4.0f * speedThreshold) {
          DestroyBuilding(specialBuildings[i], true);

          // LOSE GAME STATE for special building.
          GuiDebug.debugText = "YOU LOST in Level " + GameManager.level + " - Precious got hurt.";
          GameManager.playing = false;
          GameManager.level = 1;
        }
     
      }
    }
    if (GameManager.playing && destroyCount == numBuildings) {
      // WIN GAME STATE for destroying all.
      GuiDebug.debugText = "YOU WON Level " + GameManager.level + " - Destroyed all the things.";
      GameManager.playing = false;
      ++GameManager.level;
    }
  }

  private void DestroyBuilding (Rigidbody building, bool special) {
    audioManager.DestroyBuildingSfx();
    int numFractures = special ? 12 : Mathf.CeilToInt(building.transform.localScale.y);
    for (int i = 0; i < numFractures; ++i) {
      FractureController fracture = 
        special ? fracturePool.GenerateSpecialFracture() : fracturePool.GetNextFracture();
      fracture.transform.position = building.transform.position;
      fracture.gameObject.SetActive(true);
      GameObject.Destroy(building.gameObject);
    }
  }

  public void GenerateCity (int level) {
    // For game win state.
    destroyCount = 0;

    if (cityRoot != null) {
      GameObject.Destroy(cityRoot);
    }
    cityRoot = new GameObject("City");

    specialBuildings = new Rigidbody[level];
    Vector2[] specialPoints = new Vector2[level];
    for (var i = 0; i < level; i++) {

      int specialX = 4;
      float specialX2 = 4;
      int specialY = 3;
      float specialY2 = 3;

      while (specialX % 4 == 0) {
        specialX = Random.Range(0, xCount);
        if ((specialX + 1) % 4 == 0) {
          specialX2 = specialX - .5f;
        } else {
          specialX2 = specialX + .5f;
        }
      }
      while (specialY % 3 == 0) {
        specialY = Random.Range(0, yCount);
        if ((specialY + 1) % 3 == 0) {
          specialY2 = specialY - .5f;
        } else {
          specialY2 = specialY + .5f;
        }
      }

      var specialPosition = new Vector3(specialX2 - 0.5f * xCount, 4f, specialY2 - 0.5f * yCount);
      
      GameObject specialBuilding = 
        (GameObject) GameObject.Instantiate(specialBuildingPrefab, specialPosition, 
                                            Quaternion.identity, cityRoot.transform);
      specialBuilding.transform.localScale = new Vector3(1.9f, 1.9f, 8f);
      specialBuilding.transform.localEulerAngles = new Vector3(-90f, 0f, 0f);

      specialBuildings[i] = specialBuilding.GetComponent<Rigidbody>();
      specialPoints[i] = new Vector2(specialX2, specialY2);
    }

    int index = 0;
    for (int x = 0; x < xCount; ++x) {
      for (int y = 0; y < yCount; ++y) {
        bool moveForward = true;
        for (int i = 0; i < specialPoints.Length; i++) {
          if ((x == specialPoints[i].x + .5f || x == specialPoints[i].x - .5f) &&
              (y == specialPoints[i].y + .5f || y == specialPoints[i].y - .5f)) {
            moveForward = false;
          }
        }

        if (moveForward) {
          Vector3 position = new Vector3(x - 0.5f * xCount, 0.05f, y - 0.5f * yCount);

          //Every fourth tile is a road
          if (x % 4 == 0) {
            //Every third tile is a cross street, so make it a 4 way;
            if (y % 3 == 0) {
              GameObject.Instantiate(road4wayPrefab, position, Quaternion.identity, 
                                     cityRoot.transform);

            } else {
              //Since it's not a cross street, make it a standard street;
              GameObject roadTile = 
                (GameObject) GameObject.Instantiate(roadStraightPrefab, position, 
                                                    Quaternion.identity, cityRoot.transform);
              roadTile.transform.localEulerAngles = new Vector3(0f, 90f, 0f);
            }
          } else {
            if (y % 3 == 0) {
              //In this case, it's a street
               GameObject.Instantiate(roadStraightPrefab, position, Quaternion.identity, 
                                      cityRoot.transform);
            } else {
              //Not a street, and not a building. Let's build!
              float height = Random.Range(minHeight, maxHeight);
              float width = Random.Range(.7f, .9f);
              float length = Random.Range(.7f, .9f);

              position.y = 0.5f * height;
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
    numBuildings = index;
  }
}
