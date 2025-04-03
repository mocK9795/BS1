using Unity.VisualScripting;
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
		public static bool AreAnglesEqual(float angle1, float angle2, float tolerance)
		{
			// Mathf.DeltaAngle returns the shortest difference in degrees between two angles.
			float difference = Mathf.DeltaAngle(angle1, angle2);
			return Mathf.Abs(difference) <= tolerance;
		}
	}

	static class BasicPhysics
	{
		public static Collider[] GetAllWithinBox(Vector3 pointA, Vector3 pointB)
		{
			// Calculate the center and size of the bounding box
			Vector3 center = (pointA + pointB) * 0.5f;
			Vector3 size = new Vector3(
				Mathf.Abs(pointB.x - pointA.x),
				Mathf.Abs(pointB.y - pointA.y),
				Mathf.Abs(pointB.z - pointA.z)
			);

			Collider[] colliders = Physics.OverlapBox(center, size * 0.5f);
			return colliders;
		}
	}

	static class BasicComponent
	{
		/// <summary>
		/// This function only finds the component in the object itself, or its parent one layer.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="focus"></param>
		/// <returns></returns>
		public static T GetInParent<T>(Component focus) where T : Component, ICommandable, IControllable
		{
			T component = focus.GetComponent<T>();
			if (component) return component;
			else if (focus.transform.parent) return focus.transform.parent.GetComponent<T>();
			return null;
		}
	}
}
