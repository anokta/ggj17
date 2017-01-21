using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EarthquakeController : MonoBehaviour {
    // Upwards modifier to lift objects via explosion.
    public GameObject cubeFragments;
    public float upwardsModifier = 2.0f;

  // Sends an explosive force within |radius| from the origin |position|.
    public void AddForce (Vector3 position, float power, float radius, ForceMode mode) {
        Collider[] colliders = Physics.OverlapSphere(position, radius);
        foreach (var collider in colliders)
        {
            Debug.Log(collider.tag);
            if (collider.tag != "Ground" && collider.tag != "Fragment")
            {
                Instantiate(cubeFragments, collider.transform.position, collider.transform.rotation);
                Rigidbody[] fragmentBodies = cubeFragments.GetComponentsInChildren<Rigidbody>();
                Destroy(collider.gameObject);
                if (fragmentBodies != null)
                {
                    foreach (var fragmentBody in fragmentBodies)
                    {
                        fragmentBody.AddExplosionForce(power, position, radius, upwardsModifier, mode);
                    }
                }
            }
        }
  }
}
