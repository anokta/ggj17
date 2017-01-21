using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CityBuilder : MonoBehaviour {
  // Building prefab.
  public GameObject buildingPrefab;
 
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
        float height = Random.Range(minHeight, maxHeight);
        Vector3 position = new Vector3(2.0f * x - xCount, 0.5f * height, 2.0f * y - yCount);

        GameObject building = 
          GameObject.Instantiate(buildingPrefab, position, Quaternion.identity, cityRoot.transform);
        building.transform.localScale = new Vector3(1.0f, height, 1.0f);
      }
    }
  }
}
