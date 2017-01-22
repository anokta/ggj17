using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EarthquakeDebug : MonoBehaviour {

  public EarthquakeController controller;

  void Update () {
    Vector3 position = controller.Position;
    position.y *= 0.1f;
    transform.position = position;
    transform.localScale = 0.5f * Vector3.one * controller.Radius;
  }
}
