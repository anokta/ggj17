using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class IntroMessage : MonoBehaviour {

  public GameManager gameManager;

  public Text timeText, scoreText;


  public Button startButton;
  private static Text timeTextStatic;
  private static Text scoreTextStatic;
  public Canvas timer;

  void Awake () {
    timeTextStatic = timeText;
    scoreTextStatic = scoreText;
  }

  public void AddTextToCanvas () {
    timeTextStatic.text = "You are on level" + GameManager.level + "\n-Save the YELLOW buildings\n\n-You have " + Mathf.Round(GameManager.remainingTime) + " seconds";
        
  }

  public static void AddBowlingScoreToCanvas (int destroyedBuildings, int level, float timeLeft) {
    scoreTextStatic.text = "You win! No survivors! You scored" + Scoring.Bowling(destroyedBuildings,
                                                                             level,
                                                                             timeLeft) + "Points!";
  }

  public static void AddScoreToCanvas (int destroyedBuildings, int level, float timeLeft) {
    scoreTextStatic.text = "You win! You scored" + Scoring.ScoreGame(destroyedBuildings,
                                                                 level,
                                                                 timeLeft) + "Points!";
  }

  public void StartLevel () {
    gameManager.ResetGame();
    transform.gameObject.GetComponent<Canvas>().enabled = false;
    timer.enabled = false;
    StartCoroutine("delayStart");
  }

  public void EndLevel (bool success) {
    GameManager.playing = false;
    timer.enabled = false;
    StartCoroutine("delayEnd");
  }

  // Update is called once per frame
  void Update () {
    AddTextToCanvas();
  }

  IEnumerator delayStart () {
    yield return new WaitForSeconds(.5f);
    GameManager.playing = true;
  }

  IEnumerator delayEnd () {
    yield return new WaitForSeconds(2.0f);
    transform.gameObject.GetComponent<Canvas>().enabled = true;
  }

}
