using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridBuilder : MonoBehaviour
{

    public int sizeX, sizeY;
    public float nodeDiameter = 0.1f;
    public float displayNodeSize = 0.1f;

    private static GridBuilder instance;
    public static GridBuilder Instance{
        get {
            if(instance == null) instance = FindObjectOfType<GridBuilder>();
            return instance;
        }
        private set{
            instance = value;
        }
    }

    public Grid grid;
    public float gridBaseHeight = 0;
    
    
    void Start(){
        MeshGenerator.OnHeightMapGenerated += Instance.SetNodeHeight;
        Instance.GenerateGrid(sizeX, sizeY, nodeDiameter);
    }

    public void GenerateGrid(int sizeX, int sizeY, float nodeDiameter){
        grid = new Grid(sizeX, sizeY, nodeDiameter);
    }

    public void SetNodeHeight(float[,] noiseMap){
        MonoBehaviour.print("Received noise map of length " + noiseMap.GetLength(0));
    }

    void OnDrawGizmos(){
        if(Instance == null || grid == null) return;
        if(grid.nodes == null) return;

        nodeDiameter = nodeDiameter >= 0.05 ? nodeDiameter : 0.05f;
        foreach(Node n in grid.nodes){
            Gizmos.DrawSphere(n.worldPos + Vector3.up * gridBaseHeight, displayNodeSize);
        }
    }
    
}

public class Grid {
    
    public int sizeX { get; private set; }
    public int sizeY { get; private set; } 
    public float nodeDiameter;
    public Node[,] nodes;

    public Grid() { sizeX = 0; sizeY = 0; nodeDiameter = 1f;}

    public Grid(int sizeX, int sizeY, float nodeDiameter){
        this.sizeX = sizeX; 
        this.sizeY = sizeY;
        this.nodeDiameter = nodeDiameter;
        GenerateNodes(Vector3.zero);
    }

    private void GenerateNodes(Vector3 center){
        int nodesOnXaxis = Mathf.RoundToInt(sizeX / nodeDiameter);
        int nodesOnYaxis = Mathf.RoundToInt(sizeY / nodeDiameter);
        nodes = new Node[nodesOnYaxis, nodesOnXaxis];
        MonoBehaviour.print(nodesOnXaxis + " " + nodesOnYaxis);

        for(int i=0; i < nodesOnYaxis; i++){
            for(int j=0; j < nodesOnXaxis; j++){
                Vector3 spawnPos = new Vector3(center.x - (sizeX / 2) + (j * nodeDiameter), 0, center.y - (sizeY / 2) + (i * nodeDiameter));
                nodes[i, j] = new Node(spawnPos);
            }
        }
    }

}

public class Node {
    public Vector3 worldPos;
    public Node parent = null;
    public bool walkable;
    public Node(Vector3 pos, Node parent = null, bool walkable = true){
        worldPos = pos;
        this.parent = parent; 
        this.walkable = walkable;
    }

    public void SetWalkability(float checkRadius, LayerMask mask){
        if(Physics.CheckSphere(worldPos, GridBuilder.Instance.nodeDiameter/2, mask)){

        }
    }
}
