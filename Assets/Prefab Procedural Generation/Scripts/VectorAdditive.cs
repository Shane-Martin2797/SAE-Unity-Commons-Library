using UnityEngine;
using System.Collections;

public static class VectorAdditive
{

	public static Vector3 Vector3Multiplication (Vector3 _Vector1, Vector3 _Vector2)
	{
		return new Vector3 (_Vector1.x * _Vector2.x,
		                    _Vector1.y * _Vector2.y,
		                    _Vector1.z * _Vector2.z);
	}
	
	public static Vector3 Vector3Division (Vector3 _VectorNumerator, Vector3 _VectorDenominator)
	{
		return new Vector3 (_VectorNumerator.x / _VectorDenominator.x,
		                    _VectorNumerator.y / _VectorDenominator.y,
		                    _VectorNumerator.z / _VectorDenominator.z);
	}
	
}
