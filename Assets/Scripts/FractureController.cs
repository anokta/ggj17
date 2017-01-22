using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FractureController : MonoBehaviour {
  // Fragment duration.
  public float minFragmentDuration;
  public float maxFragmentDuration;

  public float endingMagnitude;

  public Rigidbody[] FragmentBodies {
    get { return FragmentBodies; }
  }

  private Rigidbody[] fragmentBodies;

  private Collider sphereCollider;

  void Awake () {
    fragmentBodies = GetComponentsInChildren<Rigidbody>(true); 
    sphereCollider = GetComponent<SphereCollider>();
    sphereCollider.enabled = false;
  }

  void OnEnable () {
    for (int i = 0; i < fragmentBodies.Length; ++i) {
      fragmentBodies[i].isKinematic = false;
    }
    sphereCollider.enabled = true;
    StartCoroutine(Cleanup());
    StartCoroutine(FinalCleanup());
  }

  IEnumerator Cleanup () {
    yield return new WaitForSeconds(minFragmentDuration);
    for (int i = 0; i < fragmentBodies.Length; ++i) {
      if (fragmentBodies[i].velocity.sqrMagnitude < endingMagnitude) {
        // fragmentBodies[i].isKinematic = true;
      }
    }
    sphereCollider.enabled = false;
  }

  IEnumerator FinalCleanup () {
    yield return new WaitForSeconds(maxFragmentDuration);
    for (int i = 0; i < fragmentBodies.Length; ++i) {
      // fragmentBodies[i].isKinematic = true;
    }
  }
}

