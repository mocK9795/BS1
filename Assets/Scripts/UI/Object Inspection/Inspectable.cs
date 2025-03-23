using System;
using UnityEngine;

public interface Inspectable
{
	public InspectionData GetInspectableData();
}

public struct InspectionData {
	public enum Layout { Basic, Grid, Vertical, Horizontal }
	public Layout layout;
	public Vector2 gridCellSize;

	// Basic Layout Information
	public string basicTextData;
	public Vector2 aspectRatio;

	// Advanced Layout Information
	public InspectionElement[] elements;  

	// Basic Layout Information Initializers
	public InspectionData(string data, Vector2 uiAspect) : this(data)
	{
		aspectRatio = uiAspect;
	}
	public InspectionData(string data) : this()
	{
		this.basicTextData = data;
		aspectRatio = Vector2.one;
		layout = Layout.Basic;
	}
}

public struct InspectionElement {
	public enum Type {String, Image}
	public Type type;
	public string text;
	public Sprite sprite;
}
