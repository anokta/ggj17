using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class IntroMessage : MonoBehaviour {
    
        public Button startButton;
    public Text timeText;

    public void AddTextToCanvas()
    {
        timeText.text = "-Dont destroy the YELLOW buildings\n\n-You have " + Mathf.Round(GameManager.remainingTime) + " seconds";
        
    }

    private void Start()
    {
    }

    public void StartLevel()
    {
        Debug.Log("PRESSING BUTTON");
        GameManager.playing = true;
        transform.gameObject.SetActive(false);
    }

    // Update is called once per frame
    void Update ()
    {
        AddTextToCanvas();
    }
}
