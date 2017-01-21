using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExplosionTest : MonoBehaviour {

  public EarthquakeController earthquake;

  public LayerMask groundMask;

  // Force power in kg*m/s^2.
  public float forcePower = 10.0f;

  // Force radius in meters.
  public float forceRadius = 10.0f;

  // Impulse power in kg*m/s^2.
  public float impulsePower = 5.0f;

  // Impulse radius in meters.
  public float impulseRadius = 2.5f;

  private readonly float maxRaycastDistance = 100.0f;

  public float sineAmplitude = 1.0f;
  public float sineFrequency = 0.5f;
  private float phasor = 0.0f;

  public float targetRadius;
  public float expandPeriod;
  private bool expanding; 

  void Update () {
    RaycastHit hit;
    bool success = Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit, 
                                   maxRaycastDistance, groundMask.value);
    if (!success) {
      return;
    }

    Vector3 scale = Vector3.zero;
    Vector3 position = hit.point;
    if (Input.GetMouseButtonDown(0)) {
      transform.position = position;
      // Send an impulse on click.
      earthquake.AddForce(position, impulsePower, impulseRadius, ForceMode.Impulse);
      scale = Vector3.one * impulseRadius;
    } else if (Input.GetMouseButtonDown(1)) {
      phasor = 0.0f;
    } else if (Input.GetMouseButton(1)) {
      scale = Vector3.one * forceRadius;
      position.y = sineAmplitude * Mathf.Sin(2.0f * Mathf.PI * phasor);
      phasor += sineFrequency * Time.deltaTime;
      if (phasor >= 1.0f) {
        phasor -= 1.0f; 
      }

      transform.position = position;
      earthquake.AddForce(position, forcePower, forceRadius, ForceMode.Force);
    } else if (Input.GetMouseButtonDown(2)) {
      transform.position = position;
      expanding = true;
    }
    if (expanding) {
      phasor += Time.deltaTime / expandPeriod;
      float currentRadius = targetRadius * phasor;
      scale = Vector3.one * currentRadius;
      earthquake.AddForce(position, forcePower, currentRadius, ForceMode.Force);
      if (phasor >= 1.0f) {
        phasor = 0.0f;
        expanding = false;
      }
    }

    transform.localScale = 0.5f * scale;
  }
}
