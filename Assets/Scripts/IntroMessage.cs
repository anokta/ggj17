using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class IntroMessage : MonoBehaviour {

    GameManager gameInstance;
        public Button startButton;


    public static Text AddTextToCanvas(string textString, GameObject canvasGameObject)
    {
        Text text = canvasGameObject.AddComponent<Text>();
        text.text = textString;

        Font CrackveticaFont = (Font)Resources.GetBuiltinResource(typeof(Font), "Crackvetica.ttf");
        text.font = CrackveticaFont;
        text.material = CrackveticaFont.material;

        return text;
    }

    void Start ()
    {
        gameInstance = GetObjectByType(GameManager);
    }

    public void StartLevel()
    {
        GameManager.level = 1;
        FindObjectOfType<GameManager>().ResetGame();
    }

    // Update is called once per frame
    void Update () {
		
	}
}
