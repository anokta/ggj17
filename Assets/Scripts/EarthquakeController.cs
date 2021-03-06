﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EarthquakeController : MonoBehaviour {
  public ParticleSystem particles;

  public Collider ground;

  // Fracture fragments prefab.
  public GameObject fragmentsPrefab;

  // Layer mask for destructable objects.
  public LayerMask destructableMask;

  // Upwards modifier to lift objects via explosion.
  public float upwardsModifier = 2.0f;

  public float forcePower = 10.0f;

  public float minStableRadius = 5.0f;

  public float minStableSpeed = 4.0f;

  public float maxRadius = 20.0f;

  public float waveAmplitude = 0.75f;

  public float waveFrequency = 2.0f;

  public float waveSpeed = 1.0f;

  public Vector3 Position {
    get { return currentPosition; }
  }

  private Vector3 currentPosition;

  public float Radius {
    get { return currentRadius; }
  }

  private float currentRadius;

  private float targetRadius;

  private bool propagatingWave;

  private float wavePhasor;

  private Vector3 targetWavePosition;

  void Awake () {
    currentPosition = Vector3.zero;
    currentRadius = 0.0f;
    targetRadius = 0.0f;

    propagatingWave = false;
    wavePhasor = 0.0f;
    targetWavePosition = Vector3.zero;
  }

  void Update () {
    if (targetRadius > 0.0f) {
      // Update radius.
      currentRadius = Mathf.Lerp(currentRadius, targetRadius, minStableSpeed * Time.deltaTime);
      AddForce(currentPosition, forcePower, currentRadius, ForceMode.Force);
      targetRadius = minStableRadius;
    }
    if (propagatingWave) {
      // Propagate quake wave.
      targetWavePosition.y = currentPosition.y;
      float distance = Vector3.Distance(currentPosition, targetWavePosition);
      if (distance < 0.1f) {
        propagatingWave = false;
        targetRadius = 0.0f;
      } else {
        float intensity = Mathf.Min(0.75f * maxRadius / minStableRadius, 0.25f * distance);
        currentPosition = 
          Vector3.Lerp(currentPosition, targetWavePosition, waveSpeed * Time.deltaTime);
        currentPosition.y = waveAmplitude * intensity * Mathf.Sin(2.0f * Mathf.PI * wavePhasor);
        targetRadius = minStableRadius * intensity;

        wavePhasor += waveFrequency * Time.deltaTime;
        if (wavePhasor >= 1.0f) {
          wavePhasor -= 1.0f;
        }
      }
    }
    Vector3 particlePosition = currentPosition;
    particlePosition.y *= 0.1f;
    particles.transform.position = particlePosition;
    ParticleSystem.MainModule main = particles.main;
    main.startSizeMultiplier = 0.5f * currentRadius;
    main.startSpeedMultiplier = 0.5f * currentRadius;
//    ParticleSystem.EmissionModule emission = particles.emission;
  }

  public void StartEarthquake (Vector3 position) {
    currentPosition = position;
    currentRadius = 0.0f;
    targetRadius = minStableRadius;
    propagatingWave = false;
  }

  public void EndEarthquake () {
    currentRadius = 0.0f;
    targetRadius = 0.0f;
    propagatingWave = false;
  }

  public void Intensify (Vector3 position, float percent) {
    currentPosition = position;
    targetRadius = Mathf.Min(maxRadius, minStableRadius + percent * (maxRadius - minStableRadius));
  }

  public void SendWave (Vector3 origin, Vector3 direction) {
    propagatingWave = true;
    wavePhasor = 0.0f;
    currentPosition = origin;
    // Set a high target position to get the furthest point in |direction|.
    targetWavePosition = ground.bounds.ClosestPoint(origin + 50.0f * direction);
  }

  // Sends an explosive force within |radius| from the origin |position|.
  private void AddForce (Vector3 position, float power, float radius, ForceMode mode) {
    Collider[] colliders = Physics.OverlapSphere(position, radius, destructableMask.value);
    foreach (var collider in colliders) {
      Rigidbody body = collider.GetComponent<Rigidbody>();
      if (body != null) {
        body.AddExplosionForce(power, position, radius, upwardsModifier, mode);
      }      
    }
  }
}
