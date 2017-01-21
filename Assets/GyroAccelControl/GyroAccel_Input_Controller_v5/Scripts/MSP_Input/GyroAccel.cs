using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace MSP_Input 
{
	public class GyroAccel : MonoBehaviour 
	{
		public bool forceAccelerometer = false;
		public float smoothingTime = 0.1f;
		public float headingOffset;
		
		public float pitchOffset = 30.0f;
		public float pitchOffsetMinimum = -70f;
		public float pitchOffsetMaximum = 70f;
		
		public float gyroHeadingAmplifier = 1f;
		public float gyroPitchAmplifier = 1f;
		//
		[HideInInspector]
		private Quaternion rotation = Quaternion.identity;
		[HideInInspector]
		public float heading;
		[HideInInspector]
		public float pitch;
		[HideInInspector]
		public float roll;
		
		[System.Serializable]
		public class AutoUpdateList 
		{
			public Transform targetTransform;
			public bool copyHeading = true;
			public bool copyPitch = true;
			public bool copyRoll = true;
			public bool enabled = true;
			public float headingMin = -180f;
			public float headingMax =  180f;
			public float headingDefault = 0f;
			public float pitchMin = -90f;
			public float pitchMax =  90f;
			public float pitchDefault = 0f;
			public float rollMin = -180f;
			public float rollMax =  180f;
			public float rollDefault = 0f;
			public float smoothingTime = 0f;
		}
		
		public AutoUpdateList selfUpdate;
		public List<AutoUpdateList> autoUpdateList = new List<AutoUpdateList>();	
		
		// STATIC VARIABLES:
		static private bool _forceAccelerometer;
		static private float _smoothingTime;
		static private float _headingOffset;
		static private float _pitchOffset;
		static private float _pitchOffsetMinimum;
		static private float _pitchOffsetMaximum;
		static private float _gyroHeadingAmplifier;
		static private float _gyroPitchAmplifier;
		//
		static private Quaternion _rotation = Quaternion.identity;
		static private float _heading;
		static private float _pitch;
		static private float _roll;
		//
		static private string _transformName;
		static private AutoUpdateList _selfUpdate;
		static private List<AutoUpdateList> _autoUpdateList = new List<AutoUpdateList>();

		//================================================================================
		
		void Awake() 
		{
			Input.compensateSensors = true;
			Input.gyro.enabled = true;
			_forceAccelerometer = forceAccelerometer;
			_smoothingTime = smoothingTime;
			_headingOffset = headingOffset;
			_pitchOffset = pitchOffset;
			_pitchOffsetMinimum = pitchOffsetMinimum;
			_pitchOffsetMaximum = pitchOffsetMaximum;
			_gyroHeadingAmplifier = gyroHeadingAmplifier;
			_gyroPitchAmplifier = gyroPitchAmplifier;
			_selfUpdate = selfUpdate;
			_autoUpdateList.Clear();
			_autoUpdateList = autoUpdateList;
			_transformName = transform.name;
		}

		//================================================================================
		
		void Update() 
		{
			if (!SystemInfo.supportsGyroscope) 
			{
				forceAccelerometer = true;
				ErrorHandling.LogError("Warning [GyroAccel]: No gyroscope available -> forcing accelerometer");
			}
			if (Application.isEditor && Input.gyro.attitude == Quaternion.identity) 
			{
				MSP_Input.ErrorHandling.LogError("Warning [GyroAccel]: There seems to be a problem reading the gyroscope: did you set up Unity Remote correctly?");
			}
			//
			forceAccelerometer = _forceAccelerometer;
			smoothingTime = _smoothingTime;
			headingOffset = _headingOffset;
			pitchOffset = _pitchOffset;
			pitchOffsetMinimum = _pitchOffsetMinimum;
			pitchOffsetMaximum = _pitchOffsetMaximum;
			gyroHeadingAmplifier = _gyroHeadingAmplifier;
			gyroPitchAmplifier = _gyroPitchAmplifier;
			selfUpdate = _selfUpdate;
			autoUpdateList = _autoUpdateList;
			//
			CheckHeadingAndPitchBoundaries ();
			//
			if (!_forceAccelerometer && SystemInfo.supportsGyroscope) {
				UpdateGyroscopeOrientation ();
			} else {
				UpdateAccelerometerOrientation ();
			}
			//
			_rotation = rotation;
			_heading = heading;
			_pitch = pitch;
			_roll = roll;
			_headingOffset = headingOffset;
			_pitchOffset = pitchOffset;
			//
		}
		
		//================================================================================
		
		void LateUpdate ()
		{
			AutoUpdate ();
		}
		
		//================================================================================
		
		void UpdateGyroscopeOrientation() 
		{
			Quaternion gyroQuat = Input.gyro.attitude;
			Quaternion A = new Quaternion(0.5f,0.5f,-0.5f,0.5f);
			Quaternion B = new Quaternion(0f,0f,1f,0f);
			gyroQuat = A * gyroQuat * B;
			//
			float devicePitch;
			float deviceRoll;
			GetDevicePitchAndRollFromGravityVector(out devicePitch, out deviceRoll);
			//
			float rcosin = Mathf.Cos(Mathf.Deg2Rad * deviceRoll);
			float rsinus = Mathf.Sin(Mathf.Deg2Rad * deviceRoll);
			//
			float deltaHeading;
			deltaHeading = (-Input.gyro.rotationRateUnbiased.x * rsinus - Input.gyro.rotationRateUnbiased.y * rcosin);
			gyroHeadingAmplifier = Mathf.Clamp(gyroHeadingAmplifier,0.1f,4f);
			deltaHeading *= (gyroHeadingAmplifier-1f);
			headingOffset += deltaHeading;
			//
			float deltaPitch;
			deltaPitch = (-Input.gyro.rotationRateUnbiased.y * rsinus + Input.gyro.rotationRateUnbiased.x * rcosin);
			gyroPitchAmplifier = Mathf.Clamp(gyroPitchAmplifier,0.1f,4f);
			deltaPitch *= (gyroPitchAmplifier-1f);
			if (devicePitch > pitchOffsetMinimum && devicePitch < pitchOffsetMaximum) {
				pitchOffset += deltaPitch;
			}
			if (devicePitch <= pitchOffsetMinimum) {
				pitchOffset -= Mathf.Abs(deltaPitch);
			}
			if (devicePitch >= pitchOffsetMaximum) {
				pitchOffset += Mathf.Abs(deltaPitch);
			}
			//
			CheckHeadingAndPitchBoundaries();
			// PITCH OFFSET:
			Vector3 gyro_forward = gyroQuat * Vector3.forward;
			Vector3 rotAxis = Vector3.Cross(Vector3.up,gyro_forward);
			AnimationCurve devicePitchAdjustmentCurve = new AnimationCurve(new Keyframe(-90f, 0f), new Keyframe(pitchOffset, -pitchOffset), new Keyframe(90f, 0f));
			Quaternion extra_pitch = Quaternion.AngleAxis(devicePitchAdjustmentCurve.Evaluate(devicePitch),rotAxis);
			gyroQuat = extra_pitch * gyroQuat;
			// HEADING OFFSET:
			Quaternion extra_heading = Quaternion.AngleAxis(headingOffset,Vector3.up);
			gyroQuat = extra_heading * gyroQuat;
			// Smooth gyro quaternion
			float smoothFactor = (smoothingTime > Time.deltaTime) ? Time.deltaTime / smoothingTime : 1f;
			rotation = Quaternion.Slerp(rotation, gyroQuat, smoothFactor);
			// Compute heading, pitch, roll
			Vector3 rf = rotation * Vector3.forward;
			Vector3 prf = Vector3.Cross(Vector3.up,Vector3.Cross(rf,Vector3.up));
			float newHeading = Vector3.Angle(Vector3.forward,prf) * Mathf.Sign (rf.x);
			AnimationCurve headingSmoothCurve = new AnimationCurve(new Keyframe(-90f, 0f, 0f, 0f), 
			                                                       new Keyframe(-85, smoothFactor, 0f, 0f),
			                                                       new Keyframe(85f, smoothFactor, 0f, 0f),
			                                                       new Keyframe(90f,0f,0f,0f));
			heading = Mathf.LerpAngle(heading,newHeading,headingSmoothCurve.Evaluate(pitch));
			pitch = Mathf.LerpAngle(pitch,devicePitch+devicePitchAdjustmentCurve.Evaluate(devicePitch),smoothFactor);
			roll = Mathf.LerpAngle(roll,deviceRoll,smoothFactor);
		} // void UpdateGyroscopeOrientation()
		
		//================================================================================
		
		void UpdateAccelerometerOrientation() 
		{
			float devicePitch;
			float deviceRoll;
			GetDevicePitchAndRollFromGravityVector(out devicePitch, out deviceRoll);
			//
			AnimationCurve devicePitchAdjustmentCurve = new AnimationCurve(new Keyframe(-90f, 0f), new Keyframe(pitchOffset, -pitchOffset), new Keyframe(90f, 0f));
			Quaternion accelQuat = Quaternion.identity;
			accelQuat = GetQuaternionFromHeadingPitchRoll(headingOffset, devicePitch+devicePitchAdjustmentCurve.Evaluate(devicePitch), deviceRoll);
			// Smooth gyro quaternion
			float smoothFactor = (smoothingTime > Time.deltaTime) ? Time.deltaTime / smoothingTime : 1f;
			rotation = Quaternion.Slerp(rotation, accelQuat, smoothFactor);
			// Compute heading, pitch, roll
			heading = Mathf.LerpAngle(heading,headingOffset,smoothFactor);
			pitch = Mathf.LerpAngle(pitch,devicePitch+devicePitchAdjustmentCurve.Evaluate(devicePitch),smoothFactor);
			roll = Mathf.LerpAngle(roll,deviceRoll,smoothFactor);
		} // void UpdateAccelerometerOrientation()
		
		//================================================================================
		
		void AutoUpdate()
		{
			float h;
			float p;
			float r;
			Quaternion rot;
			float smoothFactor;

			if (selfUpdate.enabled)
			{
				if (selfUpdate.copyHeading && selfUpdate.copyPitch && selfUpdate.copyRoll && selfUpdate.headingMin == -180f && selfUpdate.headingMax == 180f && selfUpdate.pitchMin == -90f && selfUpdate.pitchMax == 90f && selfUpdate.rollMin == -180f && selfUpdate.rollMax == 180f) 
				{
					smoothFactor = (selfUpdate.smoothingTime > Time.deltaTime) ? Time.deltaTime / selfUpdate.smoothingTime : 1f;
					transform.rotation = Quaternion.Lerp(transform.rotation,_rotation,smoothFactor);
				} else {
					h = selfUpdate.copyHeading ? _heading : selfUpdate.headingDefault;
					if (h>180f) h-=360f;
					h = Mathf.Clamp(h,selfUpdate.headingMin,selfUpdate.headingMax);
					//
					p = selfUpdate.copyPitch ? _pitch : selfUpdate.pitchDefault;
					p = Mathf.Clamp(p,selfUpdate.pitchMin,selfUpdate.pitchMax);
					//
					r = selfUpdate.copyRoll ? _roll : selfUpdate.rollDefault;
					if (r>180f) r-=360f;
					r = Mathf.Clamp(r,selfUpdate.rollMin,selfUpdate.rollMax);
					//
					rot = MSP_Input.GyroAccel.GetQuaternionFromHeadingPitchRoll(h,p,r);
					smoothFactor = (selfUpdate.smoothingTime > Time.deltaTime) ? Time.deltaTime / selfUpdate.smoothingTime : 1f;
					transform.rotation = Quaternion.Lerp(transform.rotation,rot,smoothFactor);
				}
			}

			foreach (AutoUpdateList aut in autoUpdateList) 
			{
				if (aut.targetTransform && aut.enabled) 
				{
					if (aut.copyHeading && aut.copyPitch && aut.copyRoll && aut.headingMin == -180f && aut.headingMax == 180f && aut.pitchMin == -90f && aut.pitchMax == 90f && aut.rollMin == -180f && aut.rollMax == 180f)
					{
						smoothFactor = (aut.smoothingTime > Time.deltaTime) ? Time.deltaTime / aut.smoothingTime : 1f;
						aut.targetTransform.rotation = Quaternion.Lerp(aut.targetTransform.rotation,_rotation,smoothFactor);
					} else {
						h = aut.copyHeading ? _heading : aut.headingDefault;
						if (h>180f) h-=360f;
						h = Mathf.Clamp(h,aut.headingMin,aut.headingMax);
						//
						p = aut.copyPitch ? _pitch : aut.pitchDefault;
						p = Mathf.Clamp(p,aut.pitchMin,aut.pitchMax);
						//
						r = aut.copyRoll ? _roll : aut.rollDefault;
						if (r>180f) r-=360f;
						r = Mathf.Clamp(r,aut.rollMin,aut.rollMax);
						//
						rot = MSP_Input.GyroAccel.GetQuaternionFromHeadingPitchRoll(h,p,r);
						smoothFactor = (aut.smoothingTime > Time.deltaTime) ? Time.deltaTime / aut.smoothingTime : 1f;
						aut.targetTransform.rotation = Quaternion.Lerp(aut.targetTransform.rotation,rot,smoothFactor);
					}
				}
			}
		} // void AutoUpdate()
		
		//================================================================================
		
		static public void GetDevicePitchAndRollFromGravityVector(out float devicePitch, out float deviceRoll) 
		{
			if (!SystemInfo.supportsGyroscope && !SystemInfo.supportsAccelerometer) 
			{
				devicePitch = 0f;
				deviceRoll = 0f;
			} else {
				Input.gyro.enabled = true;
				// Vector holding the direction of gravity
				Vector3 gravity = Input.gyro.gravity;
				// find the projections of the gravity vector on the YZ-plane
				Vector3 gravityProjectedOnXYplane = Vector3.Cross (Vector3.forward, Vector3.Cross (gravity, Vector3.forward));
				// calculate the pitch = rotation around x-axis ("dive forward/backward")
				devicePitch = Vector3.Angle (gravity, Vector3.forward) - 90;
				// calculate the roll = rotation around z-axis ("steer left/right")
				deviceRoll = Vector3.Angle (gravityProjectedOnXYplane, -Vector3.up) * Mathf.Sign (Vector3.Cross (gravityProjectedOnXYplane, Vector3.down).z);
				AnimationCurve rollAdjustmentCurve = new AnimationCurve (new Keyframe (-90f, 0f), new Keyframe (-80f, 1f), new Keyframe (80f, 1f), new Keyframe (90f, 0f));
				deviceRoll *= rollAdjustmentCurve.Evaluate (devicePitch);
			}
		} // static public void GetDevicePitchAndRollFromGravityVector(out float devicePitch, out float deviceRoll)
		
		//================================================================================
		
		void CheckHeadingAndPitchBoundaries() 
		{
			if (heading > 360f) 
			{
				heading -=360f;
			}
			if (heading < 0f) 
			{
				heading += 360f;
			}
			//
			if (pitchOffset < pitchOffsetMinimum) 
			{
				pitchOffset = pitchOffsetMinimum;
			}
			if (pitchOffset > pitchOffsetMaximum) 
			{
				pitchOffset = pitchOffsetMaximum;
			}
		} // void CheckHeadingAndPitchBoundaries()
		
		//================================================================================
		
		static public Quaternion GetQuaternionFromHeadingPitchRoll(float inputHeading, float inputPitch, float inputRoll) 
		{
			Quaternion returnQuat = Quaternion.Euler(0f,inputHeading,0f) * Quaternion.Euler(inputPitch,0f,0f) * Quaternion.Euler(0f,0f,inputRoll);
			return returnQuat;
		} // static public Quaternion GetQuaternionFromHeadingPitchRoll(float inputHeading, float inputPitch, float inputRoll)
		
		//================================================================================
		// public Get functions:
		//================================================================================
		
		static public Quaternion GetRotation() 
		{
			return _rotation;
		}	
		
		//================================================================================
		
		static public float GetHeading() 
		{
			return _heading;
		}	
		
		//================================================================================
		
		static public float GetPitch() 
		{
			return _pitch;
		}	
		
		//================================================================================
		
		static public float GetRoll() 
		{
			return _roll;
		}	
		
		//================================================================================
		
		static public void GetHeadingPitchRoll(out float h, out float p, out float r) 
		{
			h = _heading;
			p = _pitch;
			r = _roll;
		}	
		
		//================================================================================
		
		static public float GetHeadingOffset() 
		{
			return _headingOffset;
		}
		
		//================================================================================
		
		static public float GetPitchOffset() 
		{
			return _pitchOffset;
		}
		
		//================================================================================
		
		static public float GetSmoothingTime() 
		{
			return _smoothingTime;
		}
		
		//================================================================================
		
		static public float GetGyroHeadingAmplifier() 
		{
			return _gyroHeadingAmplifier;
		}
		
		//================================================================================
		
		static public float GetGyroPitchAmplifier() 
		{
			return _gyroPitchAmplifier;
		}
		
		//================================================================================
		
		static public bool GetForceAccelerometer() 
		{
			return _forceAccelerometer;
		}
		
		//================================================================================
		// public Set/Add functions:
		//================================================================================

		static public void SetHeading(float newHeading) 
		{
			while (newHeading < -180f) 
			{
				newHeading += 360f;
			}
			while (newHeading > 180f) 
			{
				newHeading -= 360f;
			}
			SetHeadingOffset(_headingOffset - _heading + newHeading);
		}

		//================================================================================

		static public void SetHeadingOffset(float newHeadingOffset) 
		{
			_headingOffset = newHeadingOffset;
		}
		
		//================================================================================

		static public void SetPitch(float newPitch) 
		{
			newPitch = Mathf.Clamp (newPitch, -90f, 90f);
			float devicePitch;
			float deviceRoll;
			GetDevicePitchAndRollFromGravityVector(out devicePitch, out deviceRoll);
			SetPitchOffset (devicePitch - newPitch);
		}

		//================================================================================

		static public void SetPitchOffset(float newPitchOffset) 
		{
			_pitchOffset = newPitchOffset;
		}
		
		//================================================================================
		
		static public void SetPitchOffsetMinumumMaximum(float newPitchOffsetMinimum, float newPitchOffsetMaximum) 
		{
			_pitchOffsetMinimum = newPitchOffsetMinimum;
			_pitchOffsetMaximum = newPitchOffsetMaximum;
		}
		
		//================================================================================
		
		static public void SetGyroHeadingAmplifier(float newValue) 
		{
			_gyroHeadingAmplifier = newValue;
		}
		
		//================================================================================
		
		static public void SetGyroPitchAmplifier(float newValue) 
		{
			_gyroPitchAmplifier = newValue;
		}
		
		//================================================================================
		
		static public void SetSmoothingTime(float smoothTime) 
		{
			_smoothingTime = smoothTime;
		}
		
		//================================================================================
		
		static public void SetForceAccelerometer(bool newValue) 
		{
			_forceAccelerometer = newValue;
		}
		
		//================================================================================
		
		static public void AddFloatToHeadingOffset(float extraHeadingOffset) 
		{
			_headingOffset += extraHeadingOffset;
		}
		
		//================================================================================
		
		static public void AddFloatToPitchOffset(float extraPitchOffset) 
		{
			_pitchOffset += extraPitchOffset;
		}
		
		//================================================================================

		static public void EnableAutoUpdate() 
		{
			_selfUpdate.enabled = true;
			return;
		}

		//================================================================================
		
		static public void EnableAutoUpdate(string name) 
		{
			if (name == "self" || name == "Self" || name == _transformName)
			{
				_selfUpdate.enabled = true;
				return;
			}
			foreach (AutoUpdateList aut in _autoUpdateList) 
			{
				if (name == aut.targetTransform.name)
				{
					aut.enabled = true;
					return;
				}
			}
			MSP_Input.ErrorHandling.LogError("Warning [GyroAccel]: You are trying to enable AutoUpdate on object "+name+", but this object doesn't exist in the AutoUpdateList.");
			return;
		}

		//================================================================================

		static public void DisableAutoUpdate() 
		{
			_selfUpdate.enabled = false;
			return;
		}
		
		//================================================================================
		
		static public void DisableAutoUpdate(string name) 
		{
			if (name == "self" || name == "Self" || name == _transformName)
			{
				_selfUpdate.enabled = false;
				return;
			}
			foreach (AutoUpdateList aut in _autoUpdateList) 
			{
				if (name == aut.targetTransform.name)
				{
					aut.enabled = false;
					return;
				}
			}
			MSP_Input.ErrorHandling.LogError("Warning [GyroAccel]: You are trying to disable AutoUpdate on object "+name+", but this object doesn't exist in the AutoUpdateList.");
			return;
		}
		
		//================================================================================
	} // public class GyroAccel : MonoBehaviour
	
} // namespace MSP_Input