using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FracturePool : MonoBehaviour {
  // Contains fragments.
  public GameObject fracturePrefab;
  public GameObject specialFracturePrefab;

  public int maxPoolSize = 500;

  private GameObject fractureRoot;
  private FractureController[] fractures;

  private int currentIndex;

  void Awake () {
    fractures = new FractureController[maxPoolSize];
    currentIndex = 0;
  }

  public void GenerateFragments () {
    if (fractureRoot != null) {
      GameObject.Destroy(fractureRoot);
    }
    fractureRoot = new GameObject("Fractures");

    for (int i = 0; i < maxPoolSize; ++i) {
      fractures[i] = ((GameObject) Instantiate(fracturePrefab)).GetComponent<FractureController>();
      fractures[i].transform.parent = fractureRoot.transform;
      fractures[i].gameObject.SetActive(false);
    }
  }

  public FractureController GetNextFracture () {
    int index = currentIndex;
    currentIndex = (index + 1) % fractures.Length;
    return fractures[index];
  }

  public FractureController GenerateSpecialFracture () {
    FractureController specialFracture = 
      ((GameObject) Instantiate(specialFracturePrefab)).GetComponent<FractureController>();
    specialFracture.transform.parent = fractureRoot.transform;
    return specialFracture;
  }
}
