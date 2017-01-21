using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputManager : MonoBehaviour {
  // Input camera.
  public Camera inputCamera;

  // Raycast layer mask for interaction.
  public LayerMask inputMask;
	
  // Max raycast distance.
  public float maxRaycastDistance = 100.0f;

  // Current input state.
  private enum InputState {
    Down,
    Held,
    Up,
    None,
  };

  // Raycast hit.
  private RaycastHit hit;

  // Press position.
  private Vector3 pressPosition;

  // Press time in seconds.
  private float pressTime;

  // Current position.
  private Vector3 position;

  void Update () {
    InputState state = GetInputState();
    if (state == InputState.None) {
      return;
    }

    // TODO(anokta): Change these for touch input.
    Vector3 screenPosition = Input.mousePosition;
    Ray ray = inputCamera.ScreenPointToRay(screenPosition);
    bool interact = Physics.Raycast(ray, out hit, maxRaycastDistance, inputMask);

    switch (state) {
    case InputState.Down:
      if (interact) {
        pressPosition = hit.point;
        pressTime = Time.time;
        OnInputDown();
      }
      break;
    case InputState.Up:
      if (interact) {
        position = hit.point;
      }
      OnInputUp();
      break;
    case InputState.Held:
      if (interact && position != hit.point) {
        Vector3 delta = hit.point - position;
        position = hit.point;
        OnInputDrag(delta);
      }
      break;
    }
  }

  private void OnInputDown () {
    Debug.Log("Down: " + pressPosition);
  }

  private void OnInputUp () {
    Debug.Log("Up: " + position);

  }

  private void OnInputDrag (Vector3 delta) {
    Debug.Log("Drag: " + delta);

  }

  // Returns the current input state.
  private InputState GetInputState() {
    // TODO(anokta): Change these for touch input.
    if (Input.GetMouseButtonDown(0)) {
      return InputState.Down;
    } else if (Input.GetMouseButtonUp(0)) {
      return InputState.Up;
    } else if (Input.GetMouseButton(0)) {
      return InputState.Held;
    } else {
      return InputState.None;
    }
  }
}
