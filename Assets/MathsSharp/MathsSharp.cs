using UnityEngine;
using System.Collections;


public static class MathsSharp
{
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
	
		public static Vector2 Vector2Multiplication (Vector2 _Vector1, Vector2 _Vector2)
		{
			return new Vector2 (_Vector1.x * _Vector2.x,
		                    _Vector1.y * _Vector2.y);
		}
	
		public static Vector2 Vector2Division (Vector2 _VectorNumerator, Vector2 _VectorDenominator)
		{
			return new Vector2 (_VectorNumerator.x / _VectorDenominator.x,
		                    _VectorNumerator.y / _VectorDenominator.y);
		}
	}
	
	public static class MathfAddititve
	{
	
		public static bool CompareValues (int _Value1, int _Value2, int _Difference)
		{
			return (Mathf.Abs (_Value1 - _Value2) <= _Difference);
		}
		
		public static bool CompareValues (float _Value1, float _Value2, float _Difference)
		{
			return (Mathf.Abs (_Value1 - _Value2) <= _Difference);
		}
	}
	
	public static class LerpAdditive
	{
		/// <summary>
		/// Linearly interpolates between /_StartingPosition/ and /_FinishingPosition/ by /_Timer/.
		/// </summary>
		/// <returns> The next position in the lerp.</returns>
		public static Vector2 Vector2LerpUsingSpeed (Vector2 _StartingPosition, Vector2 _FinishingPosition, float _Timer, float speed)
		{
			float distance = Vector2.Distance (_StartingPosition, _FinishingPosition);
			float totalTime = distance / speed;
			return Vector2.Lerp (_StartingPosition, _FinishingPosition, _Timer / totalTime);			
		}
		/// <summary>
		/// Linearly interpolates between /_StartingPosition/ and /_FinishingPosition/ by /_Timer/.
		/// </summary>
		/// <returns> The next position in the lerp.</returns>
		public static Vector3 Vector3LerpUsingSpeed (Vector3 _StartingPosition, Vector3 _FinishingPosition, float _Timer, float speed)
		{
			float distance = Vector3.Distance (_StartingPosition, _FinishingPosition);
			float totalTime = distance / speed;
			return Vector3.Lerp (_StartingPosition, _FinishingPosition, _Timer / totalTime);			
		}
	}	
}
