using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Scoring : MonoBehaviour {


	public static int ScoreGame(int destroyedBuildings, int level, float timeLeft)
  {
    // Rounds down due to int cast, might want to adjust time scale for score  
    return (int) (destroyedBuildings * level * Mathf.Clamp(timeLeft, 1f, 20f));
  }

  public static int Bowling(int destroyedBuildings, int level, float timeLeft)
  {
    // use bowling if all buildings including special are destroyed (alternate win)
    return (int) (3 * destroyedBuildings * level * Mathf.Clamp(timeLeft, 1f, 20f));

  }
}
