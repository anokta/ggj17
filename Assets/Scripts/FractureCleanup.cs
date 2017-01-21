using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FractureCleanup : MonoBehaviour {
  // Fragment duration.
  public float fragmentDuration;

  void Start () {
    StartCoroutine(Cleanup());
  }

  IEnumerator Cleanup () {
    yield return new WaitForSeconds(fragmentDuration);
    GameObject.Destroy(gameObject);
  }
}
