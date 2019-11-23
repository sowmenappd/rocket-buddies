using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapBuilder : MonoBehaviour
{   

    public int mapSizeX, mapSizeZ;
    
    public Mesh mapMesh;


    [Range(.05f, 10f)] public float vertexPlacementDistance;

    void Start()
    {
        mapMesh = MeshGenerator.CreateMesh(transform, mapSizeX, mapSizeZ, vertexPlacementDistance);
    }

    void OnDrawGizmos(){
        if(mapMesh){
            foreach(Vector3 v in mapMesh.vertices){
                Gizmos.DrawSphere(v, 0.01f);
            }
        }

    }

}
