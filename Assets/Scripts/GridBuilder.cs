using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridBuilder : MonoBehaviour {

  public int sizeX, sizeY;
  public float nodeDiameter = 0.1f;
  public float displayNodeSize = 0.1f;

  private static GridBuilder instance;
  public static GridBuilder I {
    get {
      if (instance == null) instance = FindObjectOfType<GridBuilder> ();
      return instance;
    }
    set {
      instance = value;
    }
  }

  public Grid grid;
  public Vector3 gridBaseOffset;

  public bool debugNodes = false;

  void Start () {
    MeshGenerator.OnHeightMapGenerated += SetNodeHeightAndWalkability;
    I.GenerateGrid (sizeX, sizeY, nodeDiameter);
  }

  public void GenerateGrid (int sizeX, int sizeY, float nodeDiameter) {
    //grid = new Grid (sizeX, sizeY, nodeDiameter);
    SetNodeHeightAndWalkability (MeshGenerator.RequestHeightMap (), MeshGenerator.RequestMaxHeight ());
  }

    public void SetNodeHeightAndWalkability (float[, ] noiseMap, float maxHeight) {
    //MonoBehaviour.print("Received noise map of length " + noiseMap.GetLength(0));
    if(grid == null || grid.nodes == null)
    {
        grid = new Grid(sizeX, sizeY, nodeDiameter);
        grid.GenerateNodes(Vector3.zero);
    }
    for (int i = 0; i < grid.nodes.GetLength (0); i++) {
      for (int j = 0; j < grid.nodes.GetLength (1); j++) {
        //Noise Map vertices = (rows + 1) * (columns + 1)
        //skipping the 0th row and col in noisemap
        grid.nodes[i, j].worldPos.y = noiseMap[i, j];
      }
    }

    var maxRegionHeights = new float[MapBuilder.I.regions.Count];
    var regionWalkableStatus = new bool[MapBuilder.I.regions.Count];

    int x = 0;
    foreach (RegionType region in MapBuilder.I.regions) {
      maxRegionHeights[x] = maxHeight * region.maxHeightPercentage;
      regionWalkableStatus[x] = region.walkable;
      x++;
    }

    foreach (Node node in grid.nodes) {
      float nodeHeight = node.worldPos.y;
      for (x = 0; x < maxRegionHeights.Length; x++) {
        if (nodeHeight <= maxRegionHeights[x]) {
          node.walkable = regionWalkableStatus[x];
          break;
        }
      }
    }

  }

  void OnDrawGizmos () {
    if (!debugNodes) return;
    if (I == null || grid == null) return;
    if (grid.nodes == null) return;

    nodeDiameter = nodeDiameter >= 0.05 ? nodeDiameter : 0.05f;
    foreach (Node n in grid.nodes) {
      Gizmos.color = n.walkable ? Color.green : Color.red;
      Gizmos.DrawSphere (n.worldPos, displayNodeSize);
    }
  }

}

public class Grid {

  public int sizeX { get; private set; }
  public int sizeY { get; private set; }
  public float nodeDiameter;
  public Node[, ] nodes;

  public Grid () { sizeX = 0; sizeY = 0; nodeDiameter = 1f; }

  public Grid (int sizeX, int sizeY, float nodeDiameter) {
    this.sizeX = sizeX;
    this.sizeY = sizeY;
    this.nodeDiameter = nodeDiameter;
    GenerateNodes (Vector3.zero);
  }

  public void GenerateNodes (Vector3 center) {
    float nodeDisplacement = MapBuilder.I.vertexPlacementDistance;
    int nodesOnXaxis = Mathf.RoundToInt (sizeX / nodeDiameter);
    int nodesOnYaxis = Mathf.RoundToInt (sizeY / nodeDiameter);
    nodes = new Node[nodesOnYaxis, nodesOnXaxis];
    MonoBehaviour.print (nodesOnXaxis + " " + nodesOnYaxis);

    for (int i = 0; i < nodesOnYaxis; i++) {
      for (int j = 0; j < nodesOnXaxis; j++) {
        Vector3 spawnPos = new Vector3 (center.x - (sizeX * nodeDisplacement / 2) + (j * nodeDiameter * nodeDisplacement), 0, center.y - (sizeY * nodeDisplacement/ 2) + (i * nodeDiameter * nodeDisplacement)) + GridBuilder.I.gridBaseOffset;
        nodes[i, j] = new Node (spawnPos, null, true);
      }
    }
  }

    public Node NodeFromWorldPostion(Vector3 pos)
    {
        var index = NodeIndicesFromWorldPostion(pos);
        return nodes[index.y, index.x];
    }

    public Vector2Int NodeIndicesFromWorldPostion(Vector3 pos)
    {
        float worldX = pos.x;
        float worldZ = pos.z;

        int nodeX = Mathf.RoundToInt(((worldX + (sizeX / 2)) / (sizeX * nodeDiameter)) * sizeX);
        int nodeZ = Mathf.RoundToInt(((worldZ + (sizeY / 2)) / (sizeY * nodeDiameter)) * sizeY);
        return new Vector2Int(nodeX, nodeZ);
    }

}

public class Node {
  public Vector3 worldPos;
  public Node parent = null;
  public bool walkable = false;
  public Node (Vector3 pos, Node parent = null, bool walkable = true) {
    worldPos = pos;
    this.parent = parent;
    this.walkable = walkable;
  }
}