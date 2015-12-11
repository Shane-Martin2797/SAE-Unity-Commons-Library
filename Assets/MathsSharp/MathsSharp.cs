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

        public static Vector3 Vector3Pow (Vector3 _BaseVector, float _Power)
        {
            return new Vector3 (Mathf.Pow(_BaseVector.x, _Power),
                                Mathf.Pow(_BaseVector.y, _Power),
                                Mathf.Pow(_BaseVector.z, _Power));
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

        public static Vector2 Vector2Exponential(Vector2 _BaseVector, float _Power)
        {
            return new Vector2(Mathf.Pow(_BaseVector.x, _Power),
                               Mathf.Pow(_BaseVector.y, _Power));
        }

        //REMOVE IF FOUND TO BE REDUNDANT

        /// <summary>
        /// Makes the vector a Vector3 by adding a Z-value of 0.
        /// </summary>
        public static Vector3 Vector2ToVector3(Vector2 _Vector2)
        {
            return new Vector3(_Vector2.x, _Vector2.y, 0);
        }

        /// <summary>
        /// Makes the vector a Vector3 by adding the specified Z-value.
        /// </summary>
        public static Vector3 Vector2ToVector3(Vector2 _Vector2, float _ZValue)
        {
            return new Vector3(_Vector2.x, _Vector2.y, _ZValue);
        }

        /// <summary>
        /// Removes an axis from a Vector3 to return a Vector2. Axis calculated as 0 is X-axis, 1 is Y-axis and 2 is Z-axis.
        /// </summary>
        public static Vector2 Vector3ToVector2(Vector3 _Vector3, int _AxisToBeExcluded)
        {
            switch (_AxisToBeExcluded)
            {
                case 0:
                    return new Vector2(_Vector3.y, _Vector3.z);
                case 1:
                    return new Vector2(_Vector3.x, _Vector3.z);
                case 2:
                    return new Vector2(_Vector3.x, _Vector3.y);
                default:
                    Debug.LogError("Vector transformation defaulted to x,y. Undesired effects may occur.");
                    return new Vector2(_Vector3.x, _Vector3.y);
            }
        }

        /// <summary>
        /// Removes the Z-axis from a Vector3 to return a Vector2.
        /// </summary>
        public static Vector2 Vector3ToVector2(Vector3 _Vector3)
        {
            return new Vector2(_Vector3.x, _Vector3.y);
        }

	}

	public static class MathfAddititve
	{

        public const float PI = 3.1415926535897932384626433832795028841971693993751058209749445923078164062862089986280f;


        public static float DegToRad (float _Degrees)
        {
            return _Degrees * (PI / 180);
        }
        public static float DegToRad(float _Degrees, int _DecimalPlaces)
        {
            return RoundToNumDecimals(_Degrees * (PI / 180), _DecimalPlaces);
        }

        public static float RadToDeg(float _Radians)
        {
            return _Radians * (180 / PI);
        }
        public static float RadToDeg(float _Radians, int _DecimalPlaces)
        {
            return RoundToNumDecimals(_Radians * (180 / PI), _DecimalPlaces);
        }


        public static float RoundToNumDecimals (float _Number, int _NumPlaces)
        {
            float numScalar = Mathf.Pow(10, _NumPlaces);
            return (Mathf.RoundToInt(_Number * numScalar)/numScalar);
        }

        public static float FloorToNumDecimals(float _Number, int _NumPlaces)
        {
            float numScalar = Mathf.Pow(10, _NumPlaces);
            return (Mathf.FloorToInt(_Number * numScalar) / numScalar);
        }

        public static float CeilToNumDecimals(float _Number, int _NumPlaces)
        {
            float numScalar = Mathf.Pow(10, _NumPlaces);
            return (Mathf.CeilToInt(_Number * numScalar) / numScalar);
        }
        

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
		public static Vector2 Vector2LerpUsingSpeed (Vector2 _StartingPosition, Vector2 _FinishingPosition, float _Timer, float _Speed)
		{
			float distance = Vector2.Distance (_StartingPosition, _FinishingPosition);
            float totalTime = distance / _Speed;
			return Vector2.Lerp (_StartingPosition, _FinishingPosition, _Timer / totalTime);			
		}
		/// <summary>
		/// Linearly interpolates between /_StartingPosition/ and /_FinishingPosition/ by /_Timer/.
		/// </summary>
		/// <returns> The next position in the lerp.</returns>
		public static Vector3 Vector3LerpUsingSpeed (Vector3 _StartingPosition, Vector3 _FinishingPosition, float _Timer, float _Speed)
		{
			float distance = Vector3.Distance (_StartingPosition, _FinishingPosition);
            float totalTime = distance / _Speed;
			return Vector3.Lerp (_StartingPosition, _FinishingPosition, _Timer / totalTime);			
		}
	}	
}
