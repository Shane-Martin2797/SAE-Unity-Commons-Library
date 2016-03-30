using UnityEditor;
using UnityEngine;

[CustomEditor(typeof (GenerationSection))]
public class GenerationSectionInspector : Editor
{

	private SerializedProperty EntArr;
	
	private SerializedProperty val;
	
	void OnEnable ()
	{
		val = serializedObject.FindProperty ("AllEntries");
	}
	
	public override void OnInspectorGUI ()
	{
		serializedObject.Update ();
		
		
		
		EditorSharp.ShowList (val, EditorSharp.Doors);
		
		
		serializedObject.ApplyModifiedProperties ();
	}
	
	
	
}
