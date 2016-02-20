using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(ProcGenTerrainController))]
public class ProcGenTerrainControllerInspector : Editor
{

	private SerializedProperty hasTarget;
	private SerializedProperty softedge;
	
	private SerializedProperty xScale;
	private SerializedProperty xSinCurve;
	private SerializedProperty yScale;
	private SerializedProperty ySinCurve;
	
	private SerializedProperty minHeightDifference;
	private SerializedProperty maxHeightDifference;
	
	private SerializedProperty heightLimit;
	private SerializedProperty depthLimit;
	
	private SerializedProperty floorDepth;
	private SerializedProperty randomiseFloorDepth;
	private SerializedProperty minFloorDepth;
	private SerializedProperty maxFloorDepth;
	
	private SerializedProperty xIntervals;
	private SerializedProperty zIntervals;
	
	private SerializedProperty type;

	void OnEnable ()
	{
		hasTarget = serializedObject.FindProperty ("hasTarget");
		
		softedge = serializedObject.FindProperty ("SoftEdge");
		
		xScale = serializedObject.FindProperty ("xScale");
		xSinCurve = serializedObject.FindProperty ("xSinCurveDegrees");
		yScale = serializedObject.FindProperty ("yScale");
		ySinCurve = serializedObject.FindProperty ("ySinCurveDegrees");

		minHeightDifference = serializedObject.FindProperty ("minHeightDifference");
		maxHeightDifference = serializedObject.FindProperty ("maxHeightDifference");
		
		heightLimit = serializedObject.FindProperty ("heightLimit");
		depthLimit = serializedObject.FindProperty ("depthLimit");
		floorDepth = serializedObject.FindProperty ("floorDepth");

		randomiseFloorDepth = serializedObject.FindProperty ("randomiseFloorDepth");
		minFloorDepth = serializedObject.FindProperty ("minimumFloorDepth");
		maxFloorDepth = serializedObject.FindProperty ("maximumFloorDepth");

		xIntervals = serializedObject.FindProperty ("xIntervals");
		zIntervals = serializedObject.FindProperty ("zIntervals");
		
		type = serializedObject.FindProperty ("typeOfFlooring");
	}
	
	public override void OnInspectorGUI ()
	{
		serializedObject.Update ();
		hasTarget.boolValue = EditorGUILayout.Toggle ("Target GameObject", hasTarget.boolValue);
		if (hasTarget.boolValue) {
			EditorGUI.indentLevel++;
			EditorGUILayout.PropertyField (serializedObject.FindProperty ("target"));
			EditorGUI.indentLevel--;
		}
		
		softedge.floatValue = EditorGUILayout.FloatField ("Vision Distance", softedge.floatValue);
		
		xIntervals.intValue = EditorGUILayout.IntField ("Chunk Size X", xIntervals.intValue);
		zIntervals.intValue = EditorGUILayout.IntField ("Chunk Size Z", zIntervals.intValue);
		
		xScale.floatValue = EditorGUILayout.Slider ("X Value Scale", xScale.floatValue, 0, 20);
		xSinCurve.floatValue = EditorGUILayout.Slider ("X Sin Curve Value", xSinCurve.floatValue, 0, 360);
		yScale.floatValue = EditorGUILayout.Slider ("Y Value Scale", yScale.floatValue, 0, 20);
		ySinCurve.floatValue = EditorGUILayout.Slider ("Y Sin Curve Value", ySinCurve.floatValue, 0, 360);
		
		minHeightDifference.floatValue = EditorGUILayout.Slider ("Min delta Height", minHeightDifference.floatValue, 0, maxHeightDifference.floatValue);
		maxHeightDifference.floatValue = EditorGUILayout.Slider ("Max delta Height", maxHeightDifference.floatValue, minHeightDifference.floatValue, 50);
		
		heightLimit.floatValue = EditorGUILayout.FloatField ("Height Limit", heightLimit.floatValue);
		depthLimit.floatValue = EditorGUILayout.FloatField ("Depth Limit", depthLimit.floatValue);
		
		randomiseFloorDepth.boolValue = EditorGUILayout.Toggle ("Randomise Floor Depth", randomiseFloorDepth.boolValue);
		if (randomiseFloorDepth.boolValue) {
			EditorGUI.indentLevel++;
			minFloorDepth.floatValue = EditorGUILayout.Slider ("Minimum Floor Depth", minFloorDepth.floatValue, 0, maxFloorDepth.floatValue);
			maxFloorDepth.floatValue = EditorGUILayout.Slider ("Maximum Floor Depth", maxFloorDepth.floatValue, minFloorDepth.floatValue, (heightLimit.floatValue - depthLimit.floatValue));
			EditorGUI.indentLevel--;
			
		} else {
			floorDepth.floatValue = EditorGUILayout.FloatField ("Floor Depth", floorDepth.floatValue);
		}
		
		EditorGUILayout.PropertyField (type);
		
		EditorGUI.indentLevel++;
		switch (type.intValue) {
		case((int)ProcGenTerrainController.FlooringType.MultipleObjects):
			EditorGUILayout.PropertyField (serializedObject.FindProperty ("flooringObject"));
			break;
		case((int)ProcGenTerrainController.FlooringType.SingleMesh):
			break;
		case((int)ProcGenTerrainController.FlooringType.TerrainObject):
			break;
		}
		EditorGUI.indentLevel--;
		
		
		serializedObject.ApplyModifiedProperties ();
	}
	
}
