using UnityEngine;

[RequireComponent (typeof(MeshFilter))]
public class MeshGenerator : MonoBehaviour
{
    [SerializeField] Bool updateMesh;
    [SerializeField] int xSize;
    [SerializeField] int zSize;
    [SerializeField] Gradient mapHeightGradient;
    [SerializeField] HeightMapGenerator heightMapGenerator;

    MeshFilter meshFilter = null;

    [ContextMenu("Create Mesh")]
    public void CreateMesh()
    {
        if (meshFilter == null) meshFilter = GetComponent<MeshFilter>();

        Vector3[] vertexData = CreateVerticies();
        int[] triangleData = CreateTriangles();


        float[,] map = null;
        if (heightMapGenerator)
        {
            map = heightMapGenerator.GetHeightMap(xSize + 1, zSize + 1);
			vertexData = ApplyHeightMap(map, vertexData);
        }

        Color[] colors = null;
        if (map != null) colors = CreateColors(map);

		Mesh plane = new Mesh();
        plane.vertices = vertexData;
        plane.triangles = triangleData;
        if (colors != null) plane.colors = colors;

		plane.RecalculateNormals();
        meshFilter.sharedMesh = plane;
    }

    Vector3[] CreateVerticies()
    {
        Vector3[] verticies = new Vector3[(xSize + 1) * (zSize + 1)];
        
        for (int i = 0, z = 0; z <= zSize; z++) {
            for (int x = 0; x <= xSize; x++) {
                verticies[i] = new Vector3(x, 0, z); i++;
            }
        }
        
        return verticies;
    }

    int[] CreateTriangles()
    {
        int[] triangles = new int[xSize * zSize * 6];

        int vertexIndex = 0; int triangleIndex = 0;
        for (int z = 0; z < zSize; z++)
        {
            for (int x = 0; x < xSize; x++)
            {
                triangles[triangleIndex] = vertexIndex;
                triangles[triangleIndex + 1] = vertexIndex + xSize + 1;
                triangles[triangleIndex + 2] = vertexIndex + 1;

                triangles[triangleIndex + 3] = vertexIndex + 1;
                triangles[triangleIndex + 4] = vertexIndex + xSize + 1;
                triangles[triangleIndex + 5] = vertexIndex + xSize + 2;

                vertexIndex++; triangleIndex += 6;
            }
            vertexIndex++;
        }

        return triangles;
    }

    Color[] CreateColors(float[,] map) 
    {
        map = Noise.NormalizeLocalNoiseMap(map);
		Color[] colors = new Color[(xSize + 1) * (zSize + 1)];
		for (int i = 0, z = 0; z <= zSize; z++) {
			for (int x = 0; x <= xSize; x++) {
				colors[i] = mapHeightGradient.Evaluate(map[x, z]); i++;
			}
		}
        return colors;
	}

    Vector3[] ApplyHeightMap(float[,] map, Vector3[] verticies)
    {
        for (int i = 0, z = 0; z <= zSize; z++) {
            for (int x = 0; x <= xSize; x++) {
                verticies[i].y = map[x, z]; i++;
			}
        }

        return verticies;
    }

	private void OnValidate()
	{
        if (updateMesh == Bool.True) CreateMesh(); 
	}
}

