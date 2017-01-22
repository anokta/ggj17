using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CityBuilder : MonoBehaviour {
  public AudioManager audioManager;

  // Building prefab.
	public int whichLevel = 1;

  public GameObject buildingPrefab;
	public GameObject roadStraightPrefab;
	public GameObject road4wayPrefab;
	public GameObject specialBldg;
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
  // Handle multiple tomorrow
  private Rigidbody specialBuildingRigidbody = null;

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
    if (specialBuildingRigidbody != null && specialBuildingRigidbody.velocity.sqrMagnitude > speedThreshold)
    {
      audioManager.DestroyBuildingSfx();
      int numFractures = Mathf.CeilToInt(12);
      for (int f = 0; f < numFractures; ++f)
      {
        FractureController fracture = fracturePool.GetSpecialFracture();
        fracture.transform.position = specialBuildingRigidbody.transform.position;
        fracture.gameObject.SetActive(true);
        GameObject.Destroy(specialBuildingRigidbody.gameObject);
      }
    }
  }

  public void GenerateCity () {
    if (cityRoot != null) {
      GameObject.Destroy(cityRoot);
    }
    cityRoot = new GameObject("City");

		Vector2[] specialPoints = new Vector2[whichLevel];
		for (var i = 0; i < whichLevel; i++) {

			int specialX = 4;
			float specialX2 = 4;
			int specialY = 3;
			float specialY2 = 3;

			while (specialX % 4 == 0) {
				specialX = Random.Range (0, xCount);
				if ((specialX + 1) % 4 == 0) {
					specialX2 = specialX - .5f;
				} else {
					specialX2 = specialX + .5f;
				}
			}
			while (specialY % 3 == 0) {
				specialY = Random.Range (0, yCount);
				if ((specialY + 1) % 3 == 0) {
					specialY2 = specialY - .5f;
				} else {
					specialY2 = specialY + .5f;
				}
			}

			var specialPosition = new Vector3 (specialX2 - xCount, 4f, specialY2 - yCount);
      
		  GameObject building2 = 
			  (GameObject)GameObject.Instantiate (specialBldg, specialPosition, Quaternion.identity, 
				  cityRoot.transform);
      specialBuildingRigidbody = building2.GetComponent<Rigidbody>();
		  building2.transform.localScale = new Vector3 (1.9f, 1.9f, 8f);
		  building2.transform.localEulerAngles = new Vector3 (-90f, 0f, 0f);
			specialPoints[i] = new Vector2 (specialX2, specialY2);
		}

    int index = 0;
		for (int x = 0; x < xCount; ++x) {
			for (int y = 0; y < yCount; ++y) {
				bool moveForward = true;
				for (int i = 0; i < specialPoints.Length; i++) {
					if ((x == specialPoints[i].x + .5f || x == specialPoints[i].x - .5f) && (y == specialPoints[i].y + .5f || y == specialPoints[i].y - .5f)) {
						moveForward = false;
					}
				}


				if(moveForward) {
					Vector3 position = new Vector3 (x - xCount, .05f, y - yCount);

					//Every fourth tile is a road
					if (x % 4 == 0) {
						//Every third tile is a cross street, so make it a 4 way;
						if (y % 3 == 0) {
							GameObject roadTile = 
								(GameObject)GameObject.Instantiate (road4wayPrefab, position, Quaternion.identity, 
									cityRoot.transform);

						} else {
							//Since it's not a cross street, make it a standard street;
							GameObject roadTile = 
								(GameObject)GameObject.Instantiate (roadStraightPrefab, position, Quaternion.identity, 
									cityRoot.transform);
							roadTile.transform.localEulerAngles = new Vector3 (0f, 90f, 0f);
						}
					} else {
						if (y % 3 == 0) {
							//In this case, it's a street
							GameObject roadTile = 
								(GameObject)GameObject.Instantiate (roadStraightPrefab, position, Quaternion.identity, 
									cityRoot.transform);
						} else {
							//Not a street, and not a building. Let's build!
							float height = Random.Range (minHeight, maxHeight);
							float width = Random.Range (.7f, .9f);
							float length = Random.Range (.7f, .9f);

							position.y = height / 2f;
							GameObject building = 
								(GameObject)GameObject.Instantiate (buildingPrefab, position, Quaternion.identity, 
									cityRoot.transform);
							building.transform.localScale = new Vector3 (width, length, height);
							building.transform.localEulerAngles = new Vector3 (-90f, 0f, 0f);
							building.GetComponent<Renderer> ().material = 
              buildingMaterials [Random.Range (0, buildingMaterials.Length)];
							buildings [index++] = building.GetComponent<Rigidbody> ();
						}
					}
				}
			}
    }
  }
}
