using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class IntroMessage : MonoBehaviour {

  public GameManager gameManager;

  public Text timeText, scoreText;

  public Text startText;

  public Button startButton;
  //  private static Text scoreTextStatic;
  public Canvas timer;

  private bool win = true;
  private bool started = false;


  void Awake () {
//    scoreTextStatic = scoreText;
  }

  public void AddTextToCanvas () {
    float levelThing = GameManager.level == 1 ? 0.0f : 1.0f / (float) GameManager.level;
    float time = Mathf.Round((1.0f + levelThing) * gameManager.timeMultiplier);

    timeText.text = win ? "Level" + GameManager.level + "\n\n-Save " + (GameManager.level > 1 ? ("those ") : "the") + " YELLOW building" + (GameManager.level > 1 ? "s" : "") + "\n\n-You have " + time + " seconds"
      : "Poor YELLOW! :(";
    startText.text = started ? (win ? "Continue" : "Restart") : "Start";

    if (started && win) {
      timeText.text = "You win!!\n FYI, You scored exactly " + Scoring.ScoreGame(CityBuilder.destroyCount,
                                                                                 GameManager.level,
                                                                                 GameManager.remainingTime) + " Points!\n\n" + timeText.text;
    }
  }

  //  public static void AddBowlingScoreToCanvas (int destroyedBuildings, int level, float timeLeft) {
  //    scoreTextStatic.text = "You win! No survivors! You scored" + Scoring.Bowling(destroyedBuildings,
  //                                                                                 level,
  //                                                                                 timeLeft) + "Points!";
  //  }
  //
  //  public static void AddScoreToCanvas (int destroyedBuildings, int level, float timeLeft) {
  //    scoreTextStatic.text = "You win! You scored" + Scoring.ScoreGame(destroyedBuildings,
  //                                                                     level,
  //                                                                     timeLeft) + "Points!";
  //  }

  public void StartLevel () {
    gameManager.ResetGame();
    transform.gameObject.GetComponent<Canvas>().enabled = false;
    timer.enabled = false;
    StartCoroutine("delayStart");
  }

  public void EndLevel (bool success) {
    started = true;
    win = success;  
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
