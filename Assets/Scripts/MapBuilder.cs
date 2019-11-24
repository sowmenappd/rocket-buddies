using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]
public class MapBuilder : MonoBehaviour
{   
    public bool useRandomSeed;
    public float seedValue;
    public int mapSizeX, mapSizeZ;
    public float meshMapHeight;

    Mesh mapMesh;

    // int counter = 0;
    // public bool propagateForward = false;

    [Range(.05f, 10f)] public float vertexPlacementDistance;
    [Range(.05f, 20f)] public float noiseScale;
    
    public bool autoUpdate;

    //TESTING
    // Vector3[] PropagateValuesForward(Vector3[] vertices)
    // {
    //     for(int z = mapSizeZ - 1; z >= 0; z--){
    //         float endVal = vertices[z * mapSizeX + mapSizeX - 1].y;
    //         for(int x = mapSizeX - 1; x >= 1; x--){
    //                 vertices[z * mapSizeX + x].y = vertices[z * mapSizeX + x - 1].y;
    //         }
    //         vertices[z * mapSizeX].y = endVal;   
    //     }
    //     return vertices;
    // }

    public void GenerateMesh(){
        var seed = seedValue;
        if(useRandomSeed){
            seed = seedValue = UnityEngine.Random.Range(-100000f, 100000f);
        }   
        mapMesh = MeshGenerator.CreateMesh(transform, mapSizeX, mapSizeZ, meshMapHeight, noiseScale, vertexPlacementDistance, seed); 
    }

}
