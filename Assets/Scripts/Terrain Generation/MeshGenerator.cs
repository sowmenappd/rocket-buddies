using System.Collections.Generic;
using UnityEngine;

public static class MeshGenerator
{

    static Vector3[] flatShadedVertices;
    static Vector2[] flatShadedUVs;
    static int[] flatShadedTriangles;

    private static float[,] noiseMap;
    private static float maxHeight;

    public static System.Action<float[,], float> OnHeightMapGenerated;

    public static Mesh CreateMesh(Transform target, int sizeX, int sizeZ, float meshMapHeight, AnimationCurve heightCurve, float noiseScale, float adjacentPointDistance, float seed, Vector2 offset, int numWaves, float frequencyInfluence, float baseDiminishValue, List<RegionType> regions, bool useFlatShading){
        MeshFilter meshFilter = target.GetComponent<MeshFilter>();
        MeshRenderer renderer = target.GetComponent<MeshRenderer>();

        Mesh mesh = new Mesh();
        mesh.name = "Auto-generated Mesh";

        float[,] noiseMap;
        mesh.vertices = CreateVertices(target.position, sizeX, sizeZ, meshMapHeight, heightCurve, noiseScale, adjacentPointDistance, seed, offset, numWaves, frequencyInfluence, baseDiminishValue, out noiseMap); 
        mesh.triangles = CreateTriangles(mesh.vertices, sizeX, sizeZ);
        
        var regionColors = GetRegionColorsFromNoisemap(sizeX, sizeZ, regions, noiseMap);
        renderer.sharedMaterial.mainTexture = CreateNoiseTexture(sizeX, sizeZ, regionColors, MapBuilder.I.blurEdges);

        mesh.uv = CreateUVs(sizeX, sizeZ);

        if(useFlatShading){
            ApplyFlatShading(sizeX, sizeZ, mesh.vertices, mesh.triangles, mesh.uv);
            mesh.vertices = flatShadedVertices;
            mesh.triangles = flatShadedTriangles;
            mesh.uv = flatShadedUVs;
        }
        mesh.RecalculateNormals();

        meshFilter.mesh = mesh;
        UnityEngine.MonoBehaviour.print("Mesh created.");
        return mesh;
    }

    private static Color[] GetRegionColorsFromNoisemap(int sizeX, int sizeZ, List<RegionType> regions, float[,] noiseMap){
        if(regions.Count == 0 || regions == null) throw new System.Exception("Regions list must contain at least one element.");
        Color[] colors = new Color[sizeX * sizeZ];
        //use this if you want automatic region sorting based on height
        //regions.Sort();
        
        maxHeight = float.MinValue;
        for(int x = 0; x <= sizeX; x++){
            for(int z = 0; z <= sizeZ; z++){
                maxHeight = Mathf.Max(maxHeight, noiseMap[z,x]);
            }
        }

        int colorIndex = 0; 
        for(int x = 0; x < sizeX; x++){
            for(int z = 0; z < sizeZ; z++){
                RegionType r = new RegionType();
                r.regionColor = Color.blue;
                for(int i=0; i < regions.Count; i++){
                    if((noiseMap[x,z] / maxHeight) <= regions[i].maxHeightPercentage) {
                        r = regions[i];
                        break;
                    }
                }
                colors[colorIndex] = r.regionColor;
                colorIndex++;
            }
        }
            
        return colors;
    }

    private static Vector2[] CreateUVs(int sizeX, int sizeZ){
        var uvs = new Vector2[(sizeX+1) * (sizeZ+1)];

        for(int i=0; i <= sizeZ; i++){
            for(int j=0; j <= sizeX; j++){
                uvs[i * (sizeX+1) + j] = new Vector2(Mathf.Clamp01(j / (float)sizeX), Mathf.Clamp01(i / (float)sizeZ));
            }
        }

        return uvs;
    }

    private static Texture2D CreateNoiseTexture(int sizeX, int sizeZ, Color[] colors, bool blur = false){
        Texture2D tex =  new Texture2D(sizeX, sizeZ);
        tex.SetPixels(colors);
        if(!blur)
            tex.filterMode = FilterMode.Point;
        tex.wrapMode = TextureWrapMode.Clamp;
        tex.Apply();
        return tex;
    }

    private static int[] CreateTriangles(Vector3[] vertices, int sizeX, int sizeZ){
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

    private static Vector3[] CreateVertices(Vector3 startPoint, int sizeX, int sizeZ, float meshMapHeight, AnimationCurve heightCurve, float noiseScale, float distanceBetweenVerts, float seed, Vector2 offset, int numWaves, float frequencyInfluence, float baseDiminishFactor, out float[,] noiseMap){
        Vector3[] vertices = new Vector3[(sizeX + 1) * (sizeZ + 1)];
        noiseMap = new float[sizeZ + 1, sizeX + 1];
        
        float halfSizeX = sizeX * distanceBetweenVerts / 2;
        float halfSizeZ = sizeZ * distanceBetweenVerts / 2;

        for(int vertIndex = 0,z = 0; z < sizeZ + 1; z++){
            for(int x = 0; x < sizeX + 1; x++){
                float samplePosX = (-sizeX / 2 + (startPoint.x + x)) * distanceBetweenVerts;
                float samplePosZ = (-sizeZ / 2 + (startPoint.z + z)) * distanceBetweenVerts;

                float amplitude = 1;
                float frequency = 1;
                float noiseHeight = 0;

                for(int i=0; i<numWaves; i++){
                    float basePerlinHeight = GetVertexHeight((samplePosX) * frequency / noiseScale + seed + offset.x, (samplePosZ) * frequency / noiseScale + seed + offset.y);
                    float perlinValue = basePerlinHeight * meshMapHeight * heightCurve.Evaluate(basePerlinHeight) * amplitude;
                    
                    noiseHeight += perlinValue;

                    frequency *= frequencyInfluence;
                    amplitude *= baseDiminishFactor;
                }

                noiseMap[z,x] = noiseHeight;
                vertices[vertIndex++] = new Vector3(samplePosX, noiseHeight, samplePosZ);
            }
        }
        MeshGenerator.noiseMap = noiseMap;
        OnHeightMapGenerated?.Invoke(noiseMap, maxHeight);
        return vertices;
    }

    public static float[,] RequestHeightMap(){
        return MeshGenerator.noiseMap;
    }

    public static float RequestMaxHeight(){
        return MeshGenerator.maxHeight;
    }

    private static float GetVertexHeight(float x, float z){
        return Mathf.Clamp01(Mathf.PerlinNoise(x, z));
    }


    private static void ApplyFlatShading(int sizeX, int sizeZ, Vector3[] vertices, int[] triangles, Vector2[] uvs){
        flatShadedVertices = new Vector3[triangles.Length];
        flatShadedUVs = new Vector2[triangles.Length];
        flatShadedTriangles = (int[])triangles.Clone();
        for(int i=0; i<triangles.Length; i++){
            flatShadedVertices[i] = vertices[triangles[i]];
            flatShadedUVs[i] = uvs[triangles[i]];
            flatShadedTriangles[i] = i;
        }
    }

}
