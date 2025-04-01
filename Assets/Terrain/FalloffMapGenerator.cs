using UnityEngine;

public class FalloffMapGenerator : MonoBehaviour
{
    [SerializeField] float falloffStart;
    [SerializeField] float falloffEnd;
    [SerializeField] MeshGenerator updateTarget;

    public float[,] CreateFalloffMap(int sizeX, int sizeZ)
    {
        Vector2 centre = new(sizeX / 2 , sizeZ / 2);
        float maxDistanceFromCentre = Vector2.Distance(centre, new(falloffEnd, falloffEnd));

        float[,] map = new float[sizeX, sizeZ];
        
        for (int x = 0; x < sizeX; x++)
        {
            for (int z = 0; z < sizeZ; z++)
            {
                float distanceFromCentre = Vector2.Distance(centre, new(x, z));
                distanceFromCentre -= falloffStart;
                float value = distanceFromCentre / maxDistanceFromCentre;

                map[x, z] = 1 - value;
            }
        }

        return map;
    }

	private void OnValidate()
	{
		if (updateTarget) updateTarget.CreateMesh();
	}
}
