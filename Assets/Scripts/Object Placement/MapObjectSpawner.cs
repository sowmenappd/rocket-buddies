using System.Collections.Generic;
using UnityEngine;

public class MapObjectSpawner : MonoBehaviour
{

    private static MapObjectSpawner instance;
    public static MapObjectSpawner I
    {
        get
        {
            if (instance == null)
                instance = FindObjectOfType<MapObjectSpawner>();
            return instance;
        }
    }

    public Grid nodeGrid;

    public MapItemGroup[] itemGroups = new MapItemGroup[1];

    [HideInInspector]
    public List<GameObject> spawned;

    public void SpawnAllItems()
    {
        if (nodeGrid == null)
        {
            nodeGrid = GridBuilder.I.grid;
        }

        var nodes = nodeGrid.nodes;
        foreach(var node in nodes){
            node.walkable = true;
        }

        DeleteExistingItems();

        float nodeDisplacement = MapBuilder.I.vertexPlacementDistance;

        HashSet<Node> crossedNodes = new HashSet<Node>();
        spawned.Clear();
        for (var i = 0; i < itemGroups.Length; i++)
        {
            int numItemsPerGroup = Random.Range(itemGroups[i].itemCountRange.x, itemGroups[i].itemCountRange.y);
            int currentCount = 0;
            print("Item count in group " + i + ": " + numItemsPerGroup);
            while (currentCount < numItemsPerGroup)
            {
                foreach (var item in itemGroups[i].items)
                {
                    if (currentCount >= numItemsPerGroup) break;
                    Node node = null;
                    int itemCount = Random.Range(0, (numItemsPerGroup / itemGroups[i].items.Count));
                    currentCount += itemCount;
                    for (int x = 0; x < itemCount; x++)
                    {
                        int randomIndexX, randomIndexY;
                        do
                        {
                            randomIndexX = Random.Range(2, nodeGrid.nodes.GetLength(1) - 2);
                            randomIndexY = Random.Range(2, nodeGrid.nodes.GetLength(0) - 2);
                        } while (crossedNodes.Contains(nodeGrid.nodes[randomIndexY, randomIndexX]));
                        crossedNodes.Add(node);
                        nodeGrid.nodes[randomIndexY, randomIndexX].walkable = false;
                        RaycastHit hit;
                        if (Physics.Raycast(nodeGrid.nodes[randomIndexY, randomIndexX].worldPos + Vector3.up * 6f, Vector3.down, out hit, 1000f))
                        {
                            var obj = item.Spawn(hit.point).GetGameObject();
                            obj.transform.parent = transform;
                            var tree = obj.transform.GetComponent<Tree>();
                            if (tree && !tree.ignoreGroundNormal){
                                obj.transform.up = hit.normal;
                                tree.RandomRotate();
                            } 
                            
                            spawned.Add(obj);
                            var spawnable = obj.transform.GetComponent<SpawnableObject>();

                            if(itemGroups[i].blocksPathfinding){
                              ProcessNeighboringNodeWalkability(randomIndexX, randomIndexY,
                            false, Mathf.FloorToInt(spawnable.radius / nodeDisplacement));
                            }
                        }
                    }
                }
            }

        }

    }

    void ProcessNeighboringNodeWalkability(int centerX, int centerY, bool walkable, int radius){
        for(int j = centerY - radius; j < centerY + radius; j++){
            int y = Mathf.Clamp(j, 2, nodeGrid.sizeY-3);
            for(int i = centerX - radius; i < centerX + radius; i++){
                int x = Mathf.Clamp(i, 2, nodeGrid.sizeX-3);
                nodeGrid.nodes[y, x].walkable = walkable;
            }
        }
    }

    private void VerifyItemTouchesGround(GameObject obj)
    {
        if (Physics.Raycast(obj.transform.position, Vector3.down, 10f, (LayerMask)8))
        {
            print("Need to shift down " + obj.transform.position);
        }
    }

    public void DeleteExistingItems()
    {
        if (spawned != null)
        {
            foreach (var t in spawned)
            {
                if (t == null) continue;
#if UNITY_EDITOR
                DestroyImmediate(t);
#else
                Destroy(t);
#endif
            }

        }
        spawned.Clear();
    }
}

[System.Serializable]
public struct MapItemGroup
{
    public string tag;
    public bool blocksPathfinding;
    public List<SpawnableObject> items;
    public Vector2Int itemCountRange;
}
