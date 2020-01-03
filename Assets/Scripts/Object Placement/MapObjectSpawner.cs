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

        for(var i = 0; i < itemGroups.Length; i++){
            foreach(var item in itemGroups[i].items){
                int randomIndexX = Random.Range(1, nodeGrid.nodes.GetLength(1)-2);
                int randomIndexY = Random.Range(1, nodeGrid.nodes.GetLength(0)-2);
                Node node = nodeGrid.nodes[randomIndexY, randomIndexX];
                spawned.Add(Instantiate(item, Vector3.up * item.groundAdjustmentHeight + node.worldPos, item.spawnableObject.transform.rotation).gameObject);
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

    public MapItemGroup(string tag, List<SpawnableObject> items){
        this.tag = tag;
        this.items = items;
    }
}

[System.Serializable]
public struct MapItemGroupDistribution{
    public string targetTag;
    public Vector2 itemCountRange;

    public MapItemGroupDistribution(string tag, Vector2 countRange){
        targetTag = tag;
        itemCountRange = countRange;
    } 
}