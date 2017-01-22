using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UITimer : MonoBehaviour {
	
	// Update is called once per frame
	void Update () {
        transform.gameObject.GetComponent<Text>().text = "" + Mathf.Round(GameManager.remainingTime);
	}
}
