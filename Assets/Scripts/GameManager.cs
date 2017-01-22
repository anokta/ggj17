using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour {
  public IntroMessage message;

  public CityBuilder city;

  public AudioManager audioManager;
  public EarthquakeController controller;

  public FracturePool fracturePool;

  public float timeMultiplier = 15.0f;

  public static int level = 1;

  public static bool playing = false;

  public static float remainingTime = 0.0f;

  void Start () {
    ResetGame();
  }

  public void ResetGame () {
    controller.EndEarthquake();
    audioManager.EndEarthquakeSfx();

    city.GenerateCity(level);
    fracturePool.GenerateFragments();    
    float levelThing = GameManager.level == 1 ? 0.0f : 1.0f / (float) level;
    remainingTime = Mathf.Round((1.0f + levelThing) * timeMultiplier);
    CameraShaker.intensifier = 1.0f;
  }

  void Update () {
    if (playing) {
      remainingTime -= Time.deltaTime;
      if (remainingTime <= 0.0f) {
        message.EndLevel(false);
      }
    }
#if UNITY_EDITOR
    if (Input.GetKeyDown(KeyCode.R)) {
      // Debug reset.
      ResetGame();
    }
#endif
  }
}