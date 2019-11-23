using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class MeshGenerator
{
    public static Mesh CreateMesh(Transform target, int sizeX, int sizeZ, float noiseScale, float adjacentPointDistance, int randomSeed = -1){
        MeshFilter meshFilter = target.GetComponent<MeshFilter>();
        MeshRenderer renderer = target.GetComponent<MeshRenderer>();

        Mesh mesh = new Mesh();
        mesh.name = "Auto-generated Mesh";

        Vector3[] vertices = CreateVertices(target.position, sizeX, sizeZ, noiseScale, adjacentPointDistance, randomSeed == -1 ? 1 : randomSeed); 
        mesh.vertices = vertices;
        
        //int[] triangles = new int[sizeX * sizeZ * 6];

        
        meshFilter.mesh = mesh;
        UnityEngine.MonoBehaviour.print("Mesh created.");
        return mesh;
    }

    private static Vector3[] CreateVertices(Vector3 startPoint, int sizeX, int sizeZ, float noiseScale, float distanceBetweenVerts, int randomSeed){
        Vector3[] vertices = new Vector3[(sizeX + 1) * (sizeZ + 1)];

        for(int z = 0; z < sizeZ; z++){
            for(int x = 0; x < sizeX; x++){
                int vertIndex = z * sizeX + x;
                float xPos = startPoint.x + x * distanceBetweenVerts;
                float zPos = startPoint.z + z * distanceBetweenVerts;
                vertices[vertIndex] = new Vector3(xPos, GetVertexHeight(xPos * noiseScale, zPos * noiseScale, randomSeed), zPos);
            }
        }

        return vertices;
    }

    private static float GetVertexHeight(float x, float z, int seed){
        float yVal = Mathf.PerlinNoise(x * seed, z * seed);
        return yVal;
    }



}
