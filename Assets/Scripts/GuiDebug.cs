using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GuiDebug : MonoBehaviour {

  public static string debugText = "";

  void OnGUI () {
    GUILayout.Label(debugText);
  }
}
