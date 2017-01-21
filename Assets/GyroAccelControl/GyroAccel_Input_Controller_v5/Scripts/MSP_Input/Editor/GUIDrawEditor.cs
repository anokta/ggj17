using UnityEditor;
using MSP_Input;

[CustomEditor (typeof(GUIDraw))]
public class GUIDrawEditor : Editor 
{

	public override void OnInspectorGUI() 
	{
		//base.OnInspectorGUI();
		string helpText = "Do not remove: " +
			"This component is part of MSP_Input and handles the drawing of " +
			"VirtualTouchpads, VirtualJoysticks and VirtualButtons.";
		EditorGUILayout.HelpBox(helpText,MessageType.None, true);
	}
}