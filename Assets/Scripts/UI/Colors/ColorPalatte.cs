using UnityEngine;

[CreateAssetMenu()]
public class ColorPalatte : ScriptableObject
{
	public Color[] colors;

	[ContextMenu("Set Alpha Max")]
	void SetAlphaMax()
	{
		for (int i = 0; i < colors.Length; i++)
		{
			colors[i] = RGB(colors[i]);
		}
	}

	public static Color RGB(Color color)
	{
		return new(color.r, color.g, color.b);
	}
}
