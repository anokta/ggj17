using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EarthquakeController : MonoBehaviour {
  // Upwards modifier to lift objects via explosion.
  public float upwardsModifier = 2.0f;

  // Sends an explosive force within |radius| from the origin |position|.
  public void AddForce (Vector3 position, float power, float radius, ForceMode mode) {
    Collider[] colliders = Physics.OverlapSphere(position, radius);
    foreach (var collider in colliders) {
      Rigidbody body = collider.GetComponent<Rigidbody>();
      if (body != null) {
        body.AddExplosionForce(power, position, radius, upwardsModifier, mode);
      }
    }
  }
}
