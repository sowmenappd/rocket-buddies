using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]
public class MapBuilder : MonoBehaviour{

    public static MapBuilder Instance{
        get; set;
    }  

    public bool useRandomSeed;
    public bool blurEdges;
    public bool useFlatShading;

    public float seedValue;
    public int mapSize;
    public float meshMapHeight;
    public AnimationCurve meshMapHeightInfluence;

    public Vector2 mapOffset;

    [Space][Header("Noise parameters")] public int noiseWaveCount;
    public float freqeuncyInfluence; //lacunarity
    public float amplitudeDiminishFactor; //persistance

    [Space][Header("Mesh region data")]
    public List<RegionType> regions;

    Mesh mapMesh;

    [Space][Range(.05f, 10f)] public float vertexPlacementDistance;
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

    void Awake(){
        Instance = this;
    }

    public void GenerateMesh(){
        var seed = seedValue;
        if(useRandomSeed){
            seed = seedValue = UnityEngine.Random.Range(-100000f, 100000f);
        }   
        mapMesh = MeshGenerator.CreateMesh(transform, mapSize, mapSize, meshMapHeight, meshMapHeightInfluence, noiseScale, vertexPlacementDistance, seed, mapOffset, noiseWaveCount, freqeuncyInfluence, amplitudeDiminishFactor, regions, useFlatShading); 
    }

    public string GetAllRegionData(){
        string data = "";
        foreach(RegionType r in regions){
            data += r.regionName + "\n";
            data += r.maxHeightPercentage + "\n";
            data += r.regionColor.r + " " + r.regionColor.g + " " + r.regionColor.b +"\n";
        }
        return data;
    }

}
