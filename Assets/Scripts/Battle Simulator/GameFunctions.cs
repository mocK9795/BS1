using UnityEngine;

namespace GameFunctions
{
	static class Basic
	{
		public static Vector2 Invert(Vector2 value) { return new(value.y, value.x); }
		public static Vector2 Inverse(Vector2 value) { return value * -1; }
		public static Vector2 LookValue(Vector2 value) { return new(-value.y, value.x); }
		public static Vector3 vector3(Vector2 value) { return new(value.x, value.y); } 
	}
}
