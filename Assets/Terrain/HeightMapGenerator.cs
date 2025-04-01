using UnityEngine;

public class HeightMapGenerator : MonoBehaviour
{
    [SerializeField] float amplitude;
    [SerializeField] AnimationCurve heightCurve;
    [SerializeField] NoiseSettings noiseData;
    [SerializeField] FalloffMapGenerator falloffGenerator;
    [SerializeField] MeshGenerator updateTarget;

	private void OnValidate()
	{
		if (updateTarget) updateTarget.CreateMesh();
	}

	public float[,] GetHeightMap(int xSize, int zSize)
    {
        noiseData.ValidateValues();
        float[,] map = Noise.GenerateNoiseMap(xSize, zSize, noiseData, new Vector2(0, 0));
        map = ApplyHeightCurve(map);
		map = ApplyAmplitude(map);
        if (falloffGenerator) map = ApplyFalloffMap(map, falloffGenerator.CreateFalloffMap(xSize, zSize));
        return map;
    }

    float[,] ApplyAmplitude(float[,] map)
    {
        for (int z = 0; z < map.GetLength(1); z++) {
            for (int x = 0; x < map.GetLength(0); x++) {
                map[x, z] *= amplitude;
            } 
        }
        return map;
    }

    float[,] ApplyHeightCurve(float[,] map)
    {
		for (int z = 0; z < map.GetLength(1); z++){
			for (int x = 0; x < map.GetLength(0); x++) {
				map[x, z] = heightCurve.Evaluate(map[x, z]);
			}
		}

		return map;
	}

    float[,] ApplyFalloffMap(float[,] map, float[,] falloffMap)
    {
        for (int z = 0; z < map.GetLength(1); z++) {
            for (int x = 0; x < map.GetLength(0); x++) {
                map[x, z] = map[x, z] * falloffMap[x, z];
            }
        }

        return map;
    }
}
