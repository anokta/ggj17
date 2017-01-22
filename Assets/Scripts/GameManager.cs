using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour {
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
    remainingTime = Mathf.Round((1.0f + 1.0f / (float) level) * timeMultiplier);
    CameraShaker.intensifier = 1.0f;
    playing = true;
  }

  void Update () {
    if (playing) {
      remainingTime -= Time.deltaTime;
      if (remainingTime <= 0.0f) {
        // LOSE GAME STATE for timeup.
        GuiDebug.debugText = "YOU LOST in Level " + GameManager.level + " - Time's up.";
        GameManager.playing = false;
        playing = false;
      } else {
        GuiDebug.debugText = "Remaining time: " + remainingTime.ToString("F1");
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