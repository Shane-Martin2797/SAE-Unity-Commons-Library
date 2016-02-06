using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;

public class Operations : MonoBehaviour
{
	public const int IntReturnValue = -19835;


	public static bool TestPlace (int place, int flag)
	{
		int testVal = flag & (1 << place);
		return testVal != 0;
	}
	
	public static bool CheckBounds (GenerationSection section, Vector3 position)
	{
		
		return position.x > section.bounds.min.x
			&& position.x < section.bounds.max.x
			&& position.y > section.bounds.min.y
			&& position.y < section.bounds.max.y
			&& position.z > section.bounds.min.z
			&& position.z < section.bounds.max.z;
	}
	
	public static bool CheckBounds (GenerationSection section, Vector3 min, Vector3 max)
	{
		
		return section.bounds.min.x < max.x
			&& section.bounds.max.x > min.x 
			&& section.bounds.min.y < max.y 
			&& section.bounds.max.y > min.y
			&& section.bounds.min.z < max.z 
			&& section.bounds.max.z > min.z;
	}
	
	
	public static bool CheckBounds (GenerationSection section, GenerationSection _section)
	{
		return section.bounds.min.x < _section.bounds.max.x 
			&& section.bounds.max.x > _section.bounds.min.x 
			&& section.bounds.min.y < _section.bounds.max.y 
			&& section.bounds.max.y > _section.bounds.min.y
			&& section.bounds.min.z < _section.bounds.max.z 
			&& section.bounds.max.z > _section.bounds.min.z;
	}
	
	
	public static bool CheckTouching (GenerationSection section, GenerationSection _section)
	{
		
		return section.bounds.min.x == _section.bounds.max.x 
			|| section.bounds.max.x == _section.bounds.min.x 
			|| section.bounds.min.y == _section.bounds.max.y 
			|| section.bounds.max.y == _section.bounds.min.y
			|| section.bounds.min.z == _section.bounds.max.z 
			|| section.bounds.max.z == _section.bounds.min.z;
	}
	
	public static Vector3 GetPosition (GenerationSection previousSection, GenerationSection generatedSection, int index)
	{
		if (previousSection == null)
		{
			if (generatedSection.parent != null)
			{
				return generatedSection.parent.transform.position;
			}
			else
			{
				return Vector3.zero;
			}
		}
		
		Vector3 position = previousSection.transform.position;
		int index2 = 0;
		if (generatedSection != null)
		{
			index2 = GetGeneratedSectionIndexFromPreviousSectionIndex (previousSection, generatedSection, index);
		}
		
		
		switch (previousSection.AllEntries [index].location)
		{
			case(GenerationSection.EntryPoints.Up):
				position.y += previousSection.bounds.extents.y;
				position += previousSection.AllEntries [index].offset;
				if (generatedSection != null)
				{
					position += generatedSection.AllEntries [index2].offset;
					position.y += generatedSection.bounds.extents.y;
				}
				break;
			case(GenerationSection.EntryPoints.Left):
				position.x -= previousSection.bounds.extents.x;
				position += previousSection.AllEntries [index].offset;
				if (generatedSection != null)
				{
					position += generatedSection.AllEntries [index2].offset;
					position.x -= generatedSection.bounds.extents.x;
				}
				break;
			case(GenerationSection.EntryPoints.Down):
				position.y -= previousSection.bounds.extents.y;
				position += previousSection.AllEntries [index].offset;
				if (generatedSection != null)
				{
					position += generatedSection.AllEntries [index2].offset;
					position.y -= generatedSection.bounds.extents.y;
				}
				break;
			case(GenerationSection.EntryPoints.Right):
				position.x += previousSection.bounds.extents.x;
				position += previousSection.AllEntries [index].offset;
				if (generatedSection != null)
				{
					position += generatedSection.AllEntries [index2].offset;
					position.x += generatedSection.bounds.extents.x;
				}
				break;
			case(GenerationSection.EntryPoints.Forward):
				position.z += previousSection.bounds.extents.z;
				position += previousSection.AllEntries [index].offset;
				if (generatedSection != null)
				{
					position += generatedSection.AllEntries [index2].offset;
					position.z += generatedSection.bounds.extents.z;
				}
				break;
			case(GenerationSection.EntryPoints.Backward):
				position.z -= previousSection.bounds.extents.y;
				position += previousSection.AllEntries [index].offset;
				if (generatedSection != null)
				{
					position += generatedSection.AllEntries [index2].offset;
					position.z -= generatedSection.bounds.extents.y;
				}
				break;
		}
		return position;
	}
	
	public static bool CheckDoorways (GenerationSection section, GenerationSection _section)
	{
		if (section == null)
		{
			return true;
		}
		for (int i = 0; i < section.AllEntries.Length; ++i)
		{
		
			if (!section.AllEntries [i].available)
			{
				continue;
			}
			for (int j = 0; j < _section.AllEntries.Length; ++j)
			{
				
				if (!_section.AllEntries [j].available)
				{
					continue;
				}
				
			
				if ((section.AllEntries [i].location == GenerationSection.EntryPoints.Up
					&& _section.AllEntries [j].location == GenerationSection.EntryPoints.Down)
				    ||
					(section.AllEntries [i].location == GenerationSection.EntryPoints.Left
					&& _section.AllEntries [j].location == GenerationSection.EntryPoints.Right)
				    ||
					(section.AllEntries [i].location == GenerationSection.EntryPoints.Down
					&& _section.AllEntries [j].location == GenerationSection.EntryPoints.Up)
				    ||
					(section.AllEntries [i].location == GenerationSection.EntryPoints.Right
					&& _section.AllEntries [j].location == GenerationSection.EntryPoints.Left)
					||
					(section.AllEntries [i].location == GenerationSection.EntryPoints.Forward
					&& _section.AllEntries [j].location == GenerationSection.EntryPoints.Backward)
				    ||
					(section.AllEntries [i].location == GenerationSection.EntryPoints.Backward
					&& _section.AllEntries [j].location == GenerationSection.EntryPoints.Forward))
				{
					return true;
				}
			}
		}
		return false;
	}
	
	public static int GetPreviousSectionIndex (GenerationSection previousSection, GenerationSection generatedSection)
	{
		
		if (previousSection == null)
		{
			Debug.Log ("This is the first object (most likely) returning " + Operations.IntReturnValue);
			return Operations.IntReturnValue;
		}
		
		for (int i = 0; i < previousSection.AllEntries.Length; ++i)
		{
			if (!previousSection.AllEntries [i].available)
			{
				continue;
			}
			for (int j = 0; j < generatedSection.AllEntries.Length; ++j)
			{
				if (!generatedSection.AllEntries [j].available)
				{
					continue;
				}
				
				if ((previousSection.AllEntries [i].location == GenerationSection.EntryPoints.Up
					&& generatedSection.AllEntries [j].location == GenerationSection.EntryPoints.Down)
				    ||
					(previousSection.AllEntries [i].location == GenerationSection.EntryPoints.Left
					&& generatedSection.AllEntries [j].location == GenerationSection.EntryPoints.Right)
				    ||
					(previousSection.AllEntries [i].location == GenerationSection.EntryPoints.Down
					&& generatedSection.AllEntries [j].location == GenerationSection.EntryPoints.Up)
				    ||
					(previousSection.AllEntries [i].location == GenerationSection.EntryPoints.Right
					&& generatedSection.AllEntries [j].location == GenerationSection.EntryPoints.Left)
				    ||
					(previousSection.AllEntries [i].location == GenerationSection.EntryPoints.Forward
					&& generatedSection.AllEntries [j].location == GenerationSection.EntryPoints.Backward)
				    ||
					(previousSection.AllEntries [i].location == GenerationSection.EntryPoints.Backward
					&& generatedSection.AllEntries [j].location == GenerationSection.EntryPoints.Forward))
				{
					return i;
				}
			}
		}

		return Operations.IntReturnValue;
	}
	
	public static int GetGeneratedSectionIndexFromPreviousSectionIndex (GenerationSection previousSection, GenerationSection generatedSection, int index)
	{
		if (previousSection == null)
		{
			return Operations.IntReturnValue;
		}
		
		for (int j = 0; j < generatedSection.AllEntries.Length; ++j)
		{
			if (!generatedSection.AllEntries [j].available)
			{
				continue;
			}
			if ((previousSection.AllEntries [index].location == GenerationSection.EntryPoints.Up
				&& generatedSection.AllEntries [j].location == GenerationSection.EntryPoints.Down)
			    ||
				(previousSection.AllEntries [index].location == GenerationSection.EntryPoints.Left
				&& generatedSection.AllEntries [j].location == GenerationSection.EntryPoints.Right)
			    ||
				(previousSection.AllEntries [index].location == GenerationSection.EntryPoints.Down
				&& generatedSection.AllEntries [j].location == GenerationSection.EntryPoints.Up)
			    ||
				(previousSection.AllEntries [index].location == GenerationSection.EntryPoints.Right
				&& generatedSection.AllEntries [j].location == GenerationSection.EntryPoints.Left)
				||
				(previousSection.AllEntries [index].location == GenerationSection.EntryPoints.Forward
				&& generatedSection.AllEntries [j].location == GenerationSection.EntryPoints.Backward)
				||
				(previousSection.AllEntries [index].location == GenerationSection.EntryPoints.Backward
				&& generatedSection.AllEntries [j].location == GenerationSection.EntryPoints.Forward))
			{
				return j;
			}
		}
		return Operations.IntReturnValue;
	}
	
	
	
	
	public static Vector3 CalculateClosestTargetBounds (GenerationSection section, Vector3 targetPos)
	{
		if (Operations.CheckBounds (section, targetPos))
		{
			return targetPos;
		}
		
		if (section == null)
		{
			return targetPos;
		}
		
		Vector3 closestPoint = section.transform.position;
		
		//Set the x to the closest x position.
		if (section.bounds.min.x < targetPos.x && section.bounds.max.x > targetPos.x)
		{
			closestPoint.x = targetPos.x;
		}
		else if (Mathf.Abs (section.bounds.min.x - targetPos.x) < Mathf.Abs (closestPoint.x - targetPos.x))
		{
			closestPoint.x = section.bounds.min.x;
		}
		else if (Mathf.Abs (section.bounds.max.x - targetPos.x) < Mathf.Abs (closestPoint.x - targetPos.x))
		{
			closestPoint.x = section.bounds.max.x;
		}
		
		//Set the y to the closest y position.
		if (section.bounds.min.y < targetPos.y && section.bounds.max.y > targetPos.y)
		{
			closestPoint.y = targetPos.y;
		}
		else if (Mathf.Abs (section.bounds.min.y - targetPos.y) < Mathf.Abs (closestPoint.y - targetPos.y))
		{
			closestPoint.y = section.bounds.min.y;
		}
		else if (Mathf.Abs (section.bounds.max.y - targetPos.y) < Mathf.Abs (closestPoint.y - targetPos.y))
		{
			closestPoint.y = section.bounds.max.y;
		}
		
		//Set the z to the closest z position.
		if (section.bounds.min.z < targetPos.z && section.bounds.max.z > targetPos.z)
		{
			closestPoint.z = targetPos.z;
		}
		else if (Mathf.Abs (section.bounds.min.z - targetPos.z) < Mathf.Abs (closestPoint.z - targetPos.z))
		{
			closestPoint.z = section.bounds.min.z;
		}
		else if (Mathf.Abs (section.bounds.max.z - targetPos.z) < Mathf.Abs (closestPoint.z - targetPos.z))
		{
			closestPoint.z = section.bounds.max.z;
		}
		
		return closestPoint;
	}
	
	

	public static GenRatios[] InitialiseRatios (GenRatios[] list)
	{
		//We could do this by using a 'Total' variable that just gets reset and it does this calculation
		//in the calculate section to spawn script.
		
		float total = 0;
		for (int i = 0; i < list.Length; ++i)
		{
			total += list [i].ratio;
		}
		for (int i = 0; i < list.Length; ++i)
		{
			list [i].ratio = (list [i].ratio / total);
		}
		return list;
	}
	
	public static List<GenRatios> InitialiseRatios (List<GenRatios> list)
	{
		//We could do this by using a 'Total' variable that just gets reset and it does this calculation
		//in the calculate section to spawn script.
		
		float total = 0;
		for (int i = 0; i < list.Count; ++i)
		{
			total += list [i].ratio;
		}
		for (int i = 0; i < list.Count; ++i)
		{
			list [i].ratio = (list [i].ratio / total);
		}
		return list;
	}
	
	public static void DisableSections (List<GenerationSection> sections)
	{
		for (int i = 0; i < sections.Count; ++i)
		{
			if (sections [i] != null)
			{
				if (sections [i].gameObject.activeSelf)
				{
					sections [i].gameObject.SetActive (false);
				}
			}
		}
	}
}

public static class EditorSharp
{

	public const string GenRatio = "GenRatios";
	public const string Doors = "Doors";
	
	public static void ShowList (SerializedProperty list, string typeOfList, bool showListSize = true, bool showListLabel = true)
	{
		{
			if (showListLabel)
			{
				EditorGUILayout.PropertyField (list);
				EditorGUI.indentLevel += 1;
			}
			if (!showListLabel || list.isExpanded)
			{
				if (showListSize)
				{
					EditorGUILayout.PropertyField (list.FindPropertyRelative ("Array.size"));
				}
				for (int i = 0; i < list.arraySize; i++)
				{
					SerializedProperty MyListRef = list.GetArrayElementAtIndex (i);
					
					switch (typeOfList)
					{
						case(GenRatio):
							SerializedProperty MyRatio = MyListRef.FindPropertyRelative ("ratio");
							SerializedProperty MySection = MyListRef.FindPropertyRelative ("section");
							EditorGUILayout.PropertyField (MyListRef);
						
							if (MyListRef.isExpanded)
							{
								EditorGUILayout.PropertyField (MySection);	
								EditorGUILayout.PropertyField (MyRatio);
							}
							break;
						case(Doors):
						
							SerializedProperty point = MyListRef.FindPropertyRelative ("location");
							SerializedProperty offset = MyListRef.FindPropertyRelative ("offset");						
							SerializedProperty aval = MyListRef.FindPropertyRelative ("available");						
							EditorGUILayout.PropertyField (MyListRef);
						
							if (MyListRef.isExpanded)
							{
								EditorGUILayout.PropertyField (point);	
								EditorGUILayout.PropertyField (offset);
								EditorGUILayout.PropertyField (aval);
							}					
							break;
						default:
							EditorGUILayout.PropertyField (list.GetArrayElementAtIndex (i));
							break;
					}
				}
			}
			if (showListLabel)
			{
				EditorGUI.indentLevel -= 1;
			}
		}
	}
	
	public static void HelpNone (string message)
	{
		EditorGUILayout.HelpBox (message, MessageType.None);
	}
	public static void HelpInfo (string message)
	{
		EditorGUILayout.HelpBox (message, MessageType.Info);
	}
	public static void HelpWarning (string message)
	{
		EditorGUILayout.HelpBox (message, MessageType.Warning);
	}
	
}


[System.Serializable]
public class GenRatios
{
	public GenerationSection section;
	public float ratio;
}

[System.Serializable]
public class GenListHolder
{
	public GenListHolder ()
	{
		listOfSections = new List<GenRatios> ();
		listOfIndicies = new List<int> ();
	}
	public List<GenRatios> listOfSections;
	public List<int> listOfIndicies;
}