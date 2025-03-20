using UnityEngine;

public interface Inspectable
{
	public InspectionData GetInspectableData();
}

public struct InspectionData {
	public string data;
	public Vector2 aspectRatio;

	public InspectionData(string data, Vector2 uiAspect) : this()
	{
		this.data = data;
		this.aspectRatio = uiAspect;
	}

	public InspectionData(string data) : this()
	{
		this.data = data;
		aspectRatio = Vector2.one;
	}
}
