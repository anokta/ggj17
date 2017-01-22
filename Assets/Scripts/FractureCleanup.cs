using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FractureCleanup : MonoBehaviour {
  // Fragment duration.
  public float minFragmentDuration;
  public float maxFragmentDuration;
  public float endingMagnitude;
  void Start() {
    StartCoroutine(Cleanup());
    StartCoroutine(FinalCleanup());
  }

  IEnumerator Cleanup() {
    yield return new WaitForSeconds(minFragmentDuration);
    foreach (Transform child in transform)
    {

      if (child.gameObject.GetComponent<Rigidbody>().velocity.sqrMagnitude <= endingMagnitude)
      {
        GameObject.Destroy(child.gameObject.GetComponent<Rigidbody>());
        //Debug.Log("Destroyed");
      }
      else Cleanup();
    }
  }
  IEnumerator FinalCleanup()
  {
    yield return new WaitForSeconds(maxFragmentDuration);
    foreach (Transform child in transform)
    {
      GameObject.Destroy(child.gameObject.GetComponent<Rigidbody>());
    }
  }
}

