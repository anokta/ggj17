using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour {
  public CityBuilder city;

  public FracturePool fracturePool;

  void Start () {
    city.GenerateCity();
    fracturePool.GenerateFragments();
  }

  void Update () {
#if UNITY_EDITOR
    if (Input.GetKeyDown(KeyCode.R)) {
      // Debug reset.
      city.GenerateCity();
    }
#endif
  }
}