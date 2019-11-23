using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class MeshGenerator
{
    public static Mesh CreateMesh(Transform target, int sizeX, int sizeZ, float adjacentPointDistance){
        MeshFilter meshFilter = target.gameObject.AddComponent<MeshFilter>();
        MeshRenderer renderer = target.gameObject.AddComponent<MeshRenderer>();

        Mesh mesh = new Mesh();

        Vector3[] vertices = CreateVertices(target.position, sizeX, sizeZ, adjacentPointDistance); 
        mesh.vertices = vertices;
        
        //int[] triangles = new int[sizeX * sizeZ * 6];

        
        meshFilter.mesh = mesh;
        return mesh;
    }

    private static Vector3[] CreateVertices(Vector3 startPoint, int sizeX, int sizeZ, float distanceBetweenVerts){
        Vector3[] vertices = new Vector3[(sizeX + 1) * (sizeZ + 1)];

        for(int z = 0; z < sizeZ; z++){
            for(int x = 0; x < sizeX; x++){
                int vertIndex = z * sizeX + x;
                vertices[vertIndex] = new Vector3(startPoint.x + x * distanceBetweenVerts, GetVertexHeight(startPoint.x * distanceBetweenVerts, startPoint.z * distanceBetweenVerts), startPoint.z + z * distanceBetweenVerts);
            }
        }

        return vertices;
    }

    private static float GetVertexHeight(float x, float z){
        float yVal = Mathf.PerlinNoise(x, z);
        return 0;
    }



}
