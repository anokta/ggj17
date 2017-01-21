using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace MSP_Input 
{
	public class VirtualJoystick : MonoBehaviour 
	{
		[System.Serializable]
		public class SendMessageReceiver
		{
			public GameObject messageReceiver;
			public string functionName;
		}

		[System.Serializable]
		public class Joystick 
		{
			public string name = "virtual joystick";
			public bool enabled = true;
			public Texture backgroundTexture; 
			public Texture buttonTexture;
			[Range(0.0f,1.0f)]
			public float backgroundSize = 0.1f;
			[Range(0.0f,1.0f)]
			public float buttonSize = 0.25f;
			public Vector2 centerDefault = new Vector2 (0.5f, 0.5f);
			[HideInInspector]
			public Vector2 centerBackground = new Vector2(0.5f,0.5f);
			[HideInInspector]
			public Vector2 centerBackgroundNew = new Vector2(0.5f,0.5f);
			[HideInInspector]
			public Vector2 centerButton = new Vector2(0.5f,0.5f);
			[HideInInspector]
			public Vector2 centerButtonNew = new Vector2(0.5f,0.5f);
			[HideInInspector]
			public Rect backgroundRect = new Rect(0,0,0,0);
			[HideInInspector]
			public Rect buttonRect = new Rect(0,0,0,0);
			[Range(0.0f,1.0f)]
			public float sensitivity = 0.1f;
			[Range(0.0f,1.0f)]
			public float smoothingTime = 0.1f;
			public Vector2 axisMultiplier = new Vector2(1f,1f); 
			public bool alwaysVisible = true;
			public bool autoUpdateEnabled = false;
			public bool autoUpdateGyroAccelScript = false;
			public List<SendMessageReceiver> autoUpdateAxisMessageReceivers = new List<SendMessageReceiver>();
			public List<SendMessageReceiver> autoUpdateDoubleTapMessageReceivers = new List<SendMessageReceiver>();
			public List<SendMessageReceiver> autoUpdateDoubleTapHoldMessageReceivers = new List<SendMessageReceiver>();
			[HideInInspector]
			public bool isActive = false;
			[HideInInspector]
			public Vector2 axis = Vector2.zero;
			[HideInInspector]
			public float angle = 0f;
			[HideInInspector]
			public float magnitude = 0f;
			[HideInInspector]
			public int touchID = -1;
			[HideInInspector]
			public AnimationCurve sensitivityCurve = new AnimationCurve(new Keyframe(-1f, 1f), new Keyframe(1f, 1f), new Keyframe(1f, 1f));
			[HideInInspector]
			public float time = 0f;
			[HideInInspector]
			public float deltaTime = 0f;
			[HideInInspector]
			public bool doubleTap = false;
			[HideInInspector]
			public bool doubleTapHold = false;
			[HideInInspector]
			public bool showSettingsInInspector = true;
		}

		public List<Joystick> virtualJoysticks = new List<Joystick>();	
		static public List<Joystick> _virtualJoysticks = new List<Joystick>();	

		private float sw;
		private float sh;
		private float maxTimeForDoubleTap = 0.5f;

		//================================================================================

		void Update() 
		{
			if (Application.isPlaying) 
			{
				UpdateRuntime ();
			}
		}

		//================================================================================

		void UpdateRuntime() 
		{
			_virtualJoysticks = new List<Joystick> (virtualJoysticks);
			//
			CalculateRects();
			//
			sw = (float)Screen.width;
			sh = (float)Screen.height;
			//
			foreach (Joystick j in virtualJoysticks)
			{
				if (!j.enabled) 
				{
					j.touchID = -1;
					j.isActive = false;
					j.axis = Vector2.zero;
					j.angle = 0f;
					j.magnitude = 0f;
					j.doubleTap = false;
					j.doubleTapHold = false;
				} else {
					//
					j.doubleTap = false;
					// Touch input
					foreach (Touch touch in Input.touches) 
					{
						Vector2 touchScreenPoint = new Vector2(touch.position.x, sh-touch.position.y);
						Vector2 centerTouch = new Vector2(touchScreenPoint.x/sw, 1f-touchScreenPoint.y/sh);
						//
						if (touch.phase == TouchPhase.Began && j.backgroundRect.Contains(touchScreenPoint)) 
						{
							j.isActive = true;
							j.touchID = touch.fingerId;
							j.centerBackgroundNew = centerTouch;
							j.centerButtonNew = centerTouch;
							j.axis = Vector2.zero;
							j.angle = 0f;
							j.magnitude = 0f;
							j.deltaTime = Time.time - j.time;
							j.time = Time.time;
							if (j.deltaTime < maxTimeForDoubleTap && j.time > 0f) 
							{
								j.doubleTap = true;
								j.doubleTapHold = true;
							} 
						}
						//
						if ((touch.phase == TouchPhase.Ended || touch.phase == TouchPhase.Canceled) && touch.fingerId == j.touchID) 
						{
							j.isActive = false;
							j.touchID = -1;
							j.centerBackgroundNew = j.centerDefault;
							j.centerButtonNew = j.centerDefault;
							j.axis = Vector2.zero;
							j.angle = 0f;
							j.magnitude = 0f;
							j.doubleTap = false;
							j.doubleTapHold = false;
						}
						//
						if (j.isActive && touch.fingerId == j.touchID) 
						{
							float maxJoyMagnitude = 0.5f * j.backgroundSize * (1f - j.buttonSize);
							Vector2 joyDir = centerTouch - j.centerBackgroundNew;
							joyDir.y *= sh / sw;
							j.axis = joyDir/maxJoyMagnitude;
							if (j.axis.sqrMagnitude > 1f) j.axis.Normalize();
							j.centerButtonNew.x = j.centerBackgroundNew.x + j.axis.x * maxJoyMagnitude;
							j.centerButtonNew.y = j.centerBackgroundNew.y + j.axis.y * maxJoyMagnitude * sw / sh;
							//
							j.angle = Vector2.Angle(Vector2.up,j.axis) * Mathf.Sign(joyDir.x);
							//
							j.axis.x *= j.sensitivityCurve.Evaluate(j.magnitude) * j.axisMultiplier.x;
							j.axis.y *= j.sensitivityCurve.Evaluate(j.magnitude) * j.axisMultiplier.y;
							//
							j.magnitude = j.axis.magnitude;
						}
					} // foreach (Touch touch in Input.touches)

					// Mouse input in editor
					if (Application.isEditor || !Input.touchSupported) 
					{
						Vector2 mouseScreenPoint = new Vector2(Input.mousePosition.x, sh-Input.mousePosition.y);
						Vector2 centerTouch = new Vector2(mouseScreenPoint.x/sw, 1f-mouseScreenPoint.y/sh);
						if (j.touchID == -1) 
						{ 
							if (Input.GetMouseButtonDown (0)) 
							{
								if (j.backgroundRect.Contains(mouseScreenPoint)) 
								{
									j.isActive = true;
									j.touchID = 100;
									j.centerBackgroundNew = centerTouch;
									j.centerButtonNew = centerTouch;
									j.axis = Vector2.zero;
									j.angle = 0f;
									j.magnitude = 0f;
									j.deltaTime = Time.time - j.time;
									j.time = Time.time;
									if (j.deltaTime < maxTimeForDoubleTap && j.time > 0f) 
									{
										j.doubleTap = true;
										j.doubleTapHold = true;
									} 
								}
							}
						}
						if (j.touchID == 100) 
						{
							if (Input.GetMouseButtonUp (0)) 
							{
								j.isActive = false;
								j.touchID = -1;
								j.centerBackgroundNew = j.centerDefault;
								j.centerButtonNew = j.centerDefault;
								j.axis = Vector2.zero;
								j.angle = 0f;
								j.magnitude = 0f;
								j.doubleTap = false;
								j.doubleTapHold = false;
							} else {
								float maxJoyMagnitude = 0.5f * j.backgroundSize * (1f - j.buttonSize);
								Vector2 joyDir = centerTouch - j.centerBackgroundNew;
								joyDir.y *= sh / sw;
								j.axis = joyDir/maxJoyMagnitude;
								if (j.axis.sqrMagnitude > 1f) j.axis.Normalize();
								j.centerButtonNew.x = j.centerBackgroundNew.x + j.axis.x * maxJoyMagnitude;
								j.centerButtonNew.y = j.centerBackgroundNew.y + j.axis.y * maxJoyMagnitude * sw / sh;
								//
								j.angle = Vector2.Angle(Vector2.up,j.axis) * Mathf.Sign(joyDir.x);
								//
								j.axis.x *= j.sensitivityCurve.Evaluate(j.magnitude) * j.axisMultiplier.x;
								j.axis.y *= j.sensitivityCurve.Evaluate(j.magnitude) * j.axisMultiplier.y;
								//
								j.magnitude = j.axis.magnitude;
							}
						}
					} // if (Application.isEditor || !Input.touchSupported)
					//
					float smoothFactor = (j.smoothingTime > Time.deltaTime) ? Time.deltaTime / j.smoothingTime : 1f;
					j.centerBackground = Vector2.Lerp(j.centerBackground,j.centerBackgroundNew,smoothFactor);
					j.centerButton = Vector2.Lerp(j.centerButton,j.centerButtonNew,smoothFactor);
					//
					if (j.autoUpdateEnabled) 
					{
						if (j.autoUpdateGyroAccelScript) 
						{
							MSP_Input.GyroAccel.AddFloatToHeadingOffset (j.axis.x);
							MSP_Input.GyroAccel.AddFloatToPitchOffset (j.axis.y);
						}
						foreach (SendMessageReceiver receiver in j.autoUpdateAxisMessageReceivers) 
						{
							if (receiver.messageReceiver) 
							{
								receiver.messageReceiver.SendMessage (receiver.functionName, j.axis, SendMessageOptions.DontRequireReceiver);
							}
						}
						foreach (SendMessageReceiver receiver in j.autoUpdateDoubleTapMessageReceivers) 
						{
							if (receiver.messageReceiver && j.doubleTap) 
							{
								receiver.messageReceiver.SendMessage (receiver.functionName, SendMessageOptions.DontRequireReceiver);
							}
						}
						foreach (SendMessageReceiver receiver in j.autoUpdateDoubleTapHoldMessageReceivers) 
						{
							if (receiver.messageReceiver && j.doubleTapHold) 
							{
								receiver.messageReceiver.SendMessage (receiver.functionName, SendMessageOptions.DontRequireReceiver);
							}
						}
					}
				} // if (!j.enabled)
			} // foreach (Joystick j in virtualJoysticks)

		} // void UpdateRuntime()
				
		//================================================================================
		
		public void CalculateRects() 
		{
			sw = (float)Screen.width;
			sh = (float)Screen.height;
			foreach (Joystick j in virtualJoysticks) 
			{
				j.backgroundRect.width = (int)(j.backgroundSize * sw);
				j.backgroundRect.height = j.backgroundRect.width; 
				j.backgroundRect.center = new Vector2 ((int)(sw * j.centerBackground.x), (int)(sh * (1f - j.centerBackground.y)));
				//
				j.buttonRect.width = (int)(j.backgroundSize * j.buttonSize * sw);
				j.buttonRect.height = j.buttonRect.width; 
				j.buttonRect.center = new Vector2 ((int)(sw * j.centerButton.x), (int)(sh * (1f - j.centerButton.y)));
			}
		}

		//================================================================================
		
		static public Vector2 GetAxis(string name) 
		{
			foreach (Joystick _virtualJoystick in _virtualJoysticks) 
			{
				if (_virtualJoystick.name == name) 
				{
					return _virtualJoystick.axis;
				}
			}
			ErrorHandling.LogError("Warning: Joystick with name '"+name+"' does not exist");
			return Vector2.zero;
		}
		
		//================================================================================
		
		static public void GetAngleAndMagnitude(string name, out float angle, out float magnitude) 
		{
			angle = 0f;
			magnitude = 0f;
			foreach (Joystick _virtualJoystick in _virtualJoysticks) 
			{
				if (_virtualJoystick.name == name) 
				{
					angle = _virtualJoystick.angle;
					magnitude = _virtualJoystick.magnitude;
					return;
				}
			}
			ErrorHandling.LogError("Warning: Joystick with name '"+name+"' does not exist");
		}

		//================================================================================
		
		static public bool GetDoubleTap(string name) 
		{
			foreach (Joystick _virtualJoystick in _virtualJoysticks) 
			{
				if (_virtualJoystick.name == name) 
				{
					return _virtualJoystick.doubleTap;
				}
			}
			ErrorHandling.LogError("Warning: Joystick with name '"+name+"' does not exist");
			return false;
		}

		//================================================================================

		static public bool GetDoubleTapHold(string name) 
		{
			foreach (Joystick _virtualJoystick in _virtualJoysticks) 
			{
				if (_virtualJoystick.name == name) 
				{
					return _virtualJoystick.doubleTapHold;
				}
			}
			ErrorHandling.LogError("Warning: Joystick with name '"+name+"' does not exist");
			return false;
		}
		
		//================================================================================
		
		static public void Enable(string name) 
		{
			foreach (Joystick _virtualJoystick in _virtualJoysticks) 
			{
				if (_virtualJoystick.name == name) 
				{
					_virtualJoystick.enabled = true;
					return;
				}
			}
			ErrorHandling.LogError("Warning: Joystick with name '"+name+"' does not exist");
			return;
		}
		
		//================================================================================
		
		static public void Disable(string name) 
		{
			foreach (Joystick _virtualJoystick in _virtualJoysticks) 
			{
				if (_virtualJoystick.name == name) 
				{
					_virtualJoystick.enabled = false;
					return;
				}
			}
			ErrorHandling.LogError("Warning: Joystick with name '"+name+"' does not exist");
			return;
		}

		//================================================================================
		
		static public void SetCenter(string name, Vector2 newCenter)
		{
			foreach (Joystick _virtualJoystick in _virtualJoysticks) 
			{
				if (_virtualJoystick.name == name) 
				{
					_virtualJoystick.centerDefault = newCenter;
					_virtualJoystick.centerBackgroundNew = newCenter;
					_virtualJoystick.centerButtonNew = newCenter;
				}
			}
			return;
		}
		
		//================================================================================
		
		static public void SetSize(string name, float newSize)
		{
			foreach (Joystick _virtualJoystick in _virtualJoysticks) 
			{
				if (_virtualJoystick.name == name) 
				{
					_virtualJoystick.backgroundSize = newSize;
				}
			}
			return;
		}

		//================================================================================

	} // public class VirtualJoystick

} // namespace MSP_Input