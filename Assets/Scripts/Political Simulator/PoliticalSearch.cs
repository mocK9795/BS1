using System;
using Unity.Collections;
using UnityEngine;

public class PoliticalSearch : MonoBehaviour
{
	/// <summary>
	/// Find Object By Nationality and Return Closest to Location
	/// </summary>
	/// <typeparam name="T"></typeparam>
	/// <param name="nationality"></param>
	/// <param name="location"></param>
	/// <returns></returns>
	public static T Find<T>(FixedString32Bytes nationality, Vector3 location) where T : MonoBehaviour
    {
		T[] allScripts = FindObjectsByType<T>(FindObjectsSortMode.None);
		T closest = null;
		float closestDistance = float.MaxValue;

		foreach (T script in allScripts)
		{
			PEBObject scriptNationality = script.GetComponent<PEBObject>();
			if (scriptNationality == null) continue;
			if (scriptNationality.nation.Value == nationality) continue;

			float distance = Vector3.Distance(script.transform.position, location);
			if (distance > closestDistance) continue; 
			closestDistance = distance;
			closest = script;
		}

		return closest;
	}
}
