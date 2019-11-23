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
    
    public Mesh mapMesh;

    int counter = 0;
    public bool propagateForward = false;

    [Range(.05f, 10f)] public float vertexPlacementDistance;
    [Range(.05f, 1f)] public float noiseScale;
    
    void Update(){
        // counter = (counter + 1) % 3;
        if(Input.GetKeyDown(KeyCode.Space)){
            GenerateMesh();
        }
        // if(mapMesh && propagateForward && counter == 0){
        //     mapMesh.vertices = PropagateValuesForward(mapMesh.vertices);
        // }
    }

    //TESTING
    Vector3[] PropagateValuesForward(Vector3[] vertices)
    {
        for(int z = mapSizeZ - 1; z >= 0; z--){
            float endVal = vertices[z * mapSizeX + mapSizeX - 1].y;
            for(int x = mapSizeX - 1; x >= 1; x--){
                    vertices[z * mapSizeX + x].y = vertices[z * mapSizeX + x - 1].y;
            }
            vertices[z * mapSizeX].y = endVal;   
        }
        return vertices;
    }

    void GenerateMesh(){
        var seed = seedValue;
        if(useRandomSeed){
            seed = seedValue = UnityEngine.Random.Range(-4f, 5f);
        }   
        mapMesh = MeshGenerator.CreateMesh(transform, mapSizeX, mapSizeZ, noiseScale, vertexPlacementDistance, seedValue); 
        mapMesh.RecalculateNormals();
        //mapMesh.RecalculateTangents();
    }

    void OnDrawGizmosSelected(){
        if(mapMesh){
            foreach(Vector3 v in mapMesh.vertices){
                Gizmos.DrawSphere(v, 0.05f);
            }
        }

    }

}
