using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]
public class MapBuilder : MonoBehaviour
{   
    public bool useRandomSeed;
    public int seedValue;
    public int mapSizeX, mapSizeZ;
    
    public Mesh mapMesh;


    [Range(.05f, 10f)] public float vertexPlacementDistance;
    [Range(.05f, 1f)] public float noiseScale;
    
    void Update(){
        if(Input.GetKeyDown(KeyCode.Space)){
            GenerateMesh();
        }
    }

    void GenerateMesh(){
        var seed = seedValue;
        if(useRandomSeed){
            seed = seedValue = Random.Range(-4, 5);
        }   
        mapMesh = MeshGenerator.CreateMesh(transform, mapSizeX, mapSizeZ, noiseScale, vertexPlacementDistance, seedValue); 
    }

    void OnDrawGizmos(){
        if(mapMesh){
            foreach(Vector3 v in mapMesh.vertices){
                Gizmos.DrawSphere(v, 0.01f);
            }
        }

    }

}
