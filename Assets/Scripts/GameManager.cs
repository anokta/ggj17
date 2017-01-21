using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour {

  public CityBuilder city;

  void Start () {
    city.GenerateCity();
  }
	
  void Update () {
    if (Input.GetKeyDown(KeyCode.R)) {
      // Debug reset.
      city.GenerateCity();
    }
  }
}
