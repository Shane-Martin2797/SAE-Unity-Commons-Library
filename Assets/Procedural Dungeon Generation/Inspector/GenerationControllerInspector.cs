using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(GenerationController))]
public class GenerationControllerInspector : Editor
{

	private SerializedProperty softEdge;
	private SerializedProperty hasTarg;
	private SerializedProperty limitRepeats;
	private SerializedProperty consecutiveRepeats;
	
	void OnEnable ()
	{
		softEdge = serializedObject.FindProperty ("generationSoftEdge");
		hasTarg = serializedObject.FindProperty ("hasTarget");
		limitRepeats = serializedObject.FindProperty ("maximumRepeats");
		consecutiveRepeats = serializedObject.FindProperty ("maxConsecutiveRepeats");
	}
	
	public override void OnInspectorGUI ()
	{
		serializedObject.Update ();
		
		
		
		EditorGUILayout.Separator ();
		EditorGUILayout.HelpBox ("Distance between prefab and object before it disables", MessageType.None);
		softEdge.floatValue = EditorGUILayout.FloatField ("", softEdge.floatValue);
		EditorGUILayout.Separator ();
		
		hasTarg.boolValue = EditorGUILayout.Toggle ("Target GameObject", hasTarg.boolValue);
		if (hasTarg.boolValue) {
			EditorGUI.indentLevel++;
			EditorGUILayout.PropertyField (serializedObject.FindProperty ("target"));
			EditorGUI.indentLevel--;
		}
		
		limitRepeats.boolValue = EditorGUILayout.ToggleLeft ("Minimise Consecutive Repeats", limitRepeats.boolValue);
		if (limitRepeats.boolValue) {
			consecutiveRepeats.intValue = EditorGUILayout.IntSlider ("Maximum Consecutive Repeats", consecutiveRepeats.intValue, 0, 10);
		}
		
		
		
		EditorSharp.ShowList (serializedObject.FindProperty ("sectionPrefabs"), EditorSharp.GenRatio);
		
		EditorGUILayout.Separator ();
		serializedObject.ApplyModifiedProperties ();
	}
}
