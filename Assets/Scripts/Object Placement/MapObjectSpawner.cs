using System.Collections;
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

    public List<Tree> treeTypesToSpawn;

    [HideInInspector]
    public List<GameObject> spawned;

    public void SpawnAllItems(){
        if(nodeGrid == null){
            nodeGrid = GridBuilder.I.grid;
        }
        //destroy existing spawned
        if(spawned != null){
            foreach(var t in spawned)
            #if UNITY_EDITOR
                DestroyImmediate(t.gameObject);
            #else
                Destroy(t.gameObject);
            #endif
        }
        spawned.Clear();

        foreach(Tree t in treeTypesToSpawn){
            //if(t != null)
            spawned.Add(Instantiate(t, new Vector3(Random.Range(-15f, 15f), 10f, Random.Range(-15f, 15f)), t.spawnableObject.transform.rotation).gameObject);
        }

        //get nodes to spawn items on (non-colliding)
        //spawn walkable items
    }
}
