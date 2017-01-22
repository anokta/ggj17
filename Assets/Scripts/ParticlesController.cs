using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticlesController : MonoBehaviour {

  public ParticleSystem particles;
	
	void Update () {
    if (!particles.IsAlive()) {
      GameObject.Destroy(gameObject);
    }
	}
}
