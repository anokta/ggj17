using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class IntroMessage : MonoBehaviour {
    
        public Button startButton;
    public Text timeText;
    public Canvas timer;

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
        transform.gameObject.GetComponent<Canvas>().enabled = false;
        timer.enabled = false;
        StartCoroutine("delayStart");
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
