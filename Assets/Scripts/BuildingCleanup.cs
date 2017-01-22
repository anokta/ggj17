using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildingCleanup : MonoBehaviour {
  public float maxDuration = 3f;
  public void RemoveRigidbody()
  {
    StartCoroutine(Cleanup());

  }

  IEnumerator Cleanup()
  {
    yield return new WaitForSeconds(maxDuration);
    GameObject.Destroy(gameObject.GetComponent<Rigidbody>());

  }
}
