using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CityBuilder : MonoBehaviour {
  // Building prefab.
  public GameObject buildingPrefab;
	public GameObject roadStraightPrefab;
	public GameObject road4wayPrefab;
 
  // Number of buildings per axis.
  public int xCount = 10;
  public int yCount = 10;

  // Height range.
  public float minHeight = 0.5f;
  public float maxHeight = 2.0f;

  // Root game object to hold buildings.
  private GameObject cityRoot = null;

  public void GenerateCity () {
    if (cityRoot != null) {
      GameObject.Destroy(cityRoot);
    }
    cityRoot = new GameObject("City");

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
						roadTile.transform.localEulerAngles = new Vector3 (0f, 90f, 0f);
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
						position.y = height / 2f;
						GameObject building = 
							(GameObject) GameObject.Instantiate(buildingPrefab, position, Quaternion.identity, 
								cityRoot.transform);
						building.transform.localScale = new Vector3(.9f, height, .9f);
					}
				}


      }
    }
  }
}
