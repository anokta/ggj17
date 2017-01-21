using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EarthquakeDebug : MonoBehaviour {

  public EarthquakeController controller;

  void Update () {
    transform.position = controller.Position;
    transform.localScale = 0.5f * Vector3.one * controller.Radius;
  }
}
