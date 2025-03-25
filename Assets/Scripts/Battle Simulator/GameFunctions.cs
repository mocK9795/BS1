using UnityEngine;

namespace GameFunctions
{
	static class Basic
	{
		public static Vector2 Invert(Vector2 value) { return new(value.y, value.x); }
		public static Vector2 Inverse(Vector2 value) { return value * -1; }
		public static Vector2 LookValue(Vector2 value) { return new(-value.y, value.x); }
		public static Vector3 vector3(Vector2 value) { return new(value.x, value.y); } 
		public static Vector3 XYPlane(Vector3 value) { return new(value.x, value.y); }
		public static Vector3 XZPlane(Vector3 value) { return new(value.x, 0, value.z); }
		public static bool Within(float x, float min, float max) { return (min <= x && max >= x); }
		public static bool Within(Vector2 value, Vector2 min, Vector2 max)
		{
			return (Within(value.x, min.x, max.x) && Within(value.y, min.y, max.y));
		}
		public static bool Within(Vector3 value, Vector3 min, Vector3 max)
		{
			return (Within(value, min, max) && Within(value.z, min.z, max.z));
		}
	}
}
