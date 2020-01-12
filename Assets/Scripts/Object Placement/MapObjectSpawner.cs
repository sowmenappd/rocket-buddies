using System.Collections.Generic;
using UnityEngine;

public class MapObjectSpawner : MonoBehaviour
{

    private static MapObjectSpawner instance;
    public static MapObjectSpawner I { 
        get{
            if(instance == null)
                instance = FindObjectOfType<MapObjectSpawner>();
            return instance;
        }
    }

    public Grid nodeGrid;

    public MapItemGroup[] itemGroups = new MapItemGroup[1];

    [HideInInspector]
    public List<GameObject> spawned;

    public void SpawnAllItems(){
        if(nodeGrid == null){
            nodeGrid = GridBuilder.I.grid;
        }

        DeleteExistingItems();

        HashSet<Node> crossedNodes = new HashSet<Node>();

        for(var i = 0; i < itemGroups.Length; i++){
            int numItemsPerGroup = Random.Range(itemGroups[i].itemCountRange.x, itemGroups[i].itemCountRange.y);
            int currentCount = 0;
            foreach(var item in itemGroups[i].items){
                if(currentCount >= numItemsPerGroup) break;
                Node node = null;
                int itemCount = Random.Range(0, (numItemsPerGroup - currentCount));   
                currentCount += itemCount;
                for(int x = 0; x < itemCount; x++){
                    do{
                        int randomIndexX = Random.Range(1, nodeGrid.nodes.GetLength(1)-2);
                        int randomIndexY = Random.Range(1, nodeGrid.nodes.GetLength(0)-2);
                        node = nodeGrid.nodes[randomIndexY, randomIndexX];
                    } while(crossedNodes.Contains(node));
                    crossedNodes.Add(node);
                    var obj = item.Spawn(Vector3.up * item.groundAdjustmentHeight + node.worldPos).GetGameObject();
                    spawned.Add(obj);
                    //spawned.Add(Instantiate(item, Vector3.up * item.groundAdjustmentHeight + node.worldPos, item.spawnableObject.transform.rotation).gameObject);
                } 
            }
        }
        

        //get nodes to spawn items on (non-colliding)
        //spawn walkable items
    }

    private void DeleteExistingItems(){
        if(spawned != null){
            foreach(var t in spawned)
            #if UNITY_EDITOR
                DestroyImmediate(t.gameObject);
            #else
                Destroy(t.gameObject);
            #endif
        }
        spawned.Clear();
    }
}

[System.Serializable]
public struct MapItemGroup{
    public string tag;
    public List<SpawnableObject> items;
    public Vector2Int itemCountRange;
}
