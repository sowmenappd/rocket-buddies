using UnityEngine;

public static class MeshGenerator
{
    public static Mesh CreateMesh(Transform target, int sizeX, int sizeZ, float meshMapHeight, float noiseScale, float adjacentPointDistance, float seed, Vector2 offset, int numWaves, float frequencyInfluence, float baseDiminishValue){
        MeshFilter meshFilter = target.GetComponent<MeshFilter>();
        MeshRenderer renderer = target.GetComponent<MeshRenderer>();

        Mesh mesh = new Mesh();
        mesh.name = "Auto-generated Mesh";

        mesh.vertices = CreateVertices(target.position, sizeX, sizeZ, meshMapHeight, noiseScale, adjacentPointDistance, seed, offset, numWaves, frequencyInfluence, baseDiminishValue); 
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

    private static Vector3[] CreateVertices(Vector3 startPoint, int sizeX, int sizeZ, float meshMapHeight, float noiseScale, float distanceBetweenVerts, float seed, Vector2 offset, int numWaves, float frequencyInfluence, float baseDiminishFactor){
        Vector3[] vertices = new Vector3[(sizeX + 1) * (sizeZ + 1)];
        
        float halfSizeX = sizeX * distanceBetweenVerts/ 2;
        float halfSizeZ = sizeZ * distanceBetweenVerts/ 2;

        for(int vertIndex = 0,z = 0; z < sizeZ + 1; z++){
            for(int x = 0; x < sizeX + 1; x++){
                float samplePosX = startPoint.x + (x * distanceBetweenVerts);
                float samplePosZ = startPoint.z + (z * distanceBetweenVerts);

                float amplitude = 1;
                float frequency = 1;
                float noiseHeight = 0;

                for(int i=0; i<numWaves; i++){
                    float basePerlineHeight = GetVertexHeight((halfSizeX - samplePosX) * frequency / noiseScale + seed + offset.x, (halfSizeZ - samplePosZ) * frequency / noiseScale + seed + offset.y);
                    float perlineValue = basePerlineHeight * meshMapHeight * amplitude;
                    
                    noiseHeight += (perlineValue * 2 - 1);

                    frequency *= frequencyInfluence;
                    amplitude *= baseDiminishFactor;
                }

                vertices[vertIndex++] = new Vector3(samplePosX, noiseHeight, samplePosZ);
            }
        }

        return vertices;
    }

    private static float GetVertexHeight(float x, float z){

        float yVal = 2 * Mathf.PerlinNoise(x, z) - 1;
        return yVal;
    }



}
