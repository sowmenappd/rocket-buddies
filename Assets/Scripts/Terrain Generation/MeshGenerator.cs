using UnityEngine;

public static class MeshGenerator
{
    public static Mesh CreateMesh(Transform target, int sizeX, int sizeZ, float meshMapHeight, float noiseScale, float adjacentPointDistance, float offset){
        MeshFilter meshFilter = target.GetComponent<MeshFilter>();
        MeshRenderer renderer = target.GetComponent<MeshRenderer>();

        Mesh mesh = new Mesh();
        mesh.name = "Auto-generated Mesh";

        mesh.vertices = CreateVertices(target.position, sizeX, sizeZ, meshMapHeight, noiseScale, adjacentPointDistance, offset); 
        mesh.triangles = CreateTriangles(mesh.vertices, sizeX, sizeZ);
        mesh.RecalculateNormals();
        meshFilter.mesh = mesh;

        UnityEngine.MonoBehaviour.print("Mesh created.");
        return mesh;
    }

    private static int[] CreateTriangles(Vector3[] vertices, int sizeX, int sizeZ)
    {
        int[] triangles = new int[sizeX * sizeZ * 6];

        int tIndex = 0;
        int currentVertex = 0;

        for(int z = 0; z < sizeZ; z++){
            for(int x = 0; x < sizeX; x++){
                triangles[tIndex + 0] = currentVertex;
                triangles[tIndex + 1] = currentVertex + sizeX + 1;
                triangles[tIndex + 2] = currentVertex + 1;
                triangles[tIndex + 5] = currentVertex + sizeX + 2;
                triangles[tIndex + 3] = currentVertex + 1;
                triangles[tIndex + 4] = currentVertex + sizeX + 1;

                tIndex += 6;
                currentVertex++;
            }
            currentVertex++;
        }
        return triangles;
    }

    private static Vector3[] CreateVertices(Vector3 startPoint, int sizeX, int sizeZ, float meshMapHeight, float noiseScale, float distanceBetweenVerts, float offset){
        Vector3[] vertices = new Vector3[(sizeX + 1) * (sizeZ + 1)];

        for(int vertIndex = 0,z = 0; z < sizeZ + 1; z++){
            for(int x = 0; x < sizeX + 1; x++){
                float xPos = startPoint.x + (x * distanceBetweenVerts);
                float zPos = startPoint.z + (z * distanceBetweenVerts);
                vertices[vertIndex++] = new Vector3(xPos, GetVertexHeight(x/noiseScale + offset, z/noiseScale + offset) * meshMapHeight, zPos);
            }
        }

        return vertices;
    }

    private static float GetVertexHeight(float x, float z){
        float yVal = 2 * Mathf.PerlinNoise(x, z) - 1;
        return yVal;
    }



}
