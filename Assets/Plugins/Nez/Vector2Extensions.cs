using UnityEngine;

public static class VectorExtensions {

	/// <summary>
	/// Returns a dot product of two vectors.
	/// </summary>
	/// <param name="value1">The first vector.</param>
	/// <param name="value2">The second vector.</param>
	/// <param name="result">The dot product of two vectors as an output parameter.</param>
	public static void Dot (ref Vector2 value1, ref Vector2 value2, out float result) {
		result = ( value1.x * value2.x ) + ( value1.y * value2.y );
	}

	/// <summary>
	/// Returns the squared length of this <see cref="Vector2"/>.
	/// </summary>
	/// <returns>The squared length of this <see cref="Vector2"/>.</returns>
	public static float LengthSquared (this Vector2 self ) {
		return ( self.x * self.x ) + ( self.y * self.y );
	}

	/// <summary>
	/// Creates a new <see cref="Vector2"/> that contains a normalized values from another vector.
	/// </summary>
	/// <param name="value">Source <see cref="Vector2"/>.</param>
	/// <param name="result">Unit vector as an output parameter.</param>
	public static void Normalize (ref Vector2 value, out Vector2 result) {
		float val = 1.0f / (float)Mathf.Sqrt ( ( value.x * value.x ) + ( value.y * value.y ) );
		result.x = value.x * val;
		result.y = value.y * val;
	}
}
