using UnityEngine;

public class FoliageSpawner : MonoBehaviour
{
	[SerializeField] Vector3Int mapBounds;
	[SerializeField] float spacing;

	[Header("Gizmos")]
	[SerializeField] Bool showGizmos;
	[SerializeField] float gizmosSize = 0.1f;

	private void OnDrawGizmos()
	{
		if (showGizmos == Bool.False) return;

		for (int z = 0; z < mapBounds.z;  z++)
		{
			for (int x = 0; x < mapBounds.x; x++)
			{
				Gizmos.DrawSphere(transform.position + new Vector3(x * spacing, mapBounds.y, z * spacing), gizmosSize * spacing);
			}
		}
	}
}
