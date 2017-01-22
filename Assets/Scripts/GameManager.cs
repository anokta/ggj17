﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour {

  public CityBuilder city;

  void Start () {
    city.GenerateCity();
    FragmentPool.current.GenerateFragments();
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