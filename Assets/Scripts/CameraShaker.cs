// ----------------------------------------------------------------------
//   soundquake - Ludum Dare 32 Compo Entry
//
//     Copyright 2015 Alper Gungormusler. All rights reserved.
//
// -----------------------------------------------------------------------

using UnityEngine;
using System.Collections;

public class CameraShaker : MonoBehaviour {

  public static float intensifier = 1.0f;

  public EarthquakeController controller;

  public float shakeMagnitude = 0.5f;
  public float shakeSpeed = 4.0f;

  Quaternion initialRotation;

  void Awake () {
    initialRotation = transform.rotation;
  }

  void Update () {
    Quaternion targetRotation = initialRotation;
    if (controller.Radius > 0.0f) {
      float rotationAngle = 
        intensifier * controller.Radius * Random.Range(-shakeMagnitude, shakeMagnitude);
      targetRotation *= Quaternion.AngleAxis(rotationAngle, Vector3.forward);
    }
    transform.rotation = 
      Quaternion.Slerp(transform.rotation, targetRotation, shakeSpeed * Time.deltaTime);
  }


}