using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class IntroMessage : MonoBehaviour {
    
    public Button startButton;
    public static Text timeText;
    public static Text scoreText;
    public Canvas timer;

    public void AddTextToCanvas()
    {
        timeText.text = "You are on level" + GameManager.level + "\n-Save the YELLOW buildings\n\n-You have " + Mathf.Round(GameManager.remainingTime) + " seconds";
        
    }
    public static void AddBowlingScoreToCanvas(int destroyedBuildings, int level, float timeLeft)
    {
      scoreText.text = "You win! No survivors! You scored" + Scoring.Bowling(destroyedBuildings, level, timeLeft) + "Points!";
    }
    public static void AddScoreToCanvas(int destroyedBuildings, int level, float timeLeft)
    {
      scoreText.text = "You win! You scored" + Scoring.ScoreGame(destroyedBuildings, level, timeLeft) + "Points!";
    }
  private void Start()
    {
    }

    public void StartLevel()
    {
        Debug.Log("PRESSING BUTTON");
        transform.gameObject.GetComponent<Canvas>().enabled = false;
        timer.enabled = false;
        StartCoroutine("delayStart");
    }
  public void EndLevel()
  {
    
    transform.gameObject.GetComponent<Canvas>().enabled = true;
    timer.enabled = false;
  }

  // Update is called once per frame
  void Update ()
    {
        AddTextToCanvas();
    }

    IEnumerator delayStart()
    {
        yield return new WaitForSeconds(.5f);
        GameManager.playing = true;
    }
}
