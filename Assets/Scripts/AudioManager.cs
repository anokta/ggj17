using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class AudioManager : MonoBehaviour {

  public AudioSource impact, hum, wave, crackStable, crackShake;

  public AudioMixerSnapshot initial, humming, shaking;

  public float transitionTime = 1.5f;

  public void Start () {
    crackStable.Play();
    crackShake.Play();
    hum.Play();
  }

  public void StartEarthquakeSfx () {
    impact.pitch = Random.Range(0.9f, 1.1f);
    impact.Play();

    humming.TransitionTo(0.25f * transitionTime);
  }

  public void EndEarthquakeSfx(){ 
    initial.TransitionTo(2.0f * transitionTime);
  }

  public void Intensify(float percent){
    if (percent > 0.25f) {
      shaking.TransitionTo(0.25f * transitionTime);
    } else {
      humming.TransitionTo(2.0f * transitionTime);
    }
  }

  public void PlayWaveSfx () {
    wave.PlayOneShot(wave.clip, wave.volume);
    initial.TransitionTo(4.0f * transitionTime);
  }
}
