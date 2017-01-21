using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FractureCleanup : MonoBehaviour {
    public float fragmentDuration;
    // Use this for initialization
    void Start()
    {
        tagChildren();
        StartCoroutine(Cleanup());
    }

    IEnumerator Cleanup()
    {
        yield return new WaitForSeconds(fragmentDuration);
        Destroy(gameObject);
    }
    void tagChildren()
    {
        Transform[] childTransforms = GetComponentsInChildren<Transform>();
        foreach (Transform t in childTransforms)
        {
            t.gameObject.tag = "Fragment";
        }
    }

}
