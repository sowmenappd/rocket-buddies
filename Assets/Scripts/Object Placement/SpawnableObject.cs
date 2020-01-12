using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SpawnableObject : MonoBehaviour, ISpawnable
{
    public GameObject spawnableObject;
    //this is for the graph node calculation
    public Vector3 centerOffset;
    public float radius = 0.5f;

    //this is for the ground contact point adjustment
    public float groundAdjustmentHeight;


    protected ISpawnable spawnable;

    public virtual void Start(){
    }

    public virtual ISpawnable Spawn(Vector3 spawnPos)
    {
        if(!spawnableObject) return null;
        print("Spawned " + spawnableObject + " as " + name);
        GameObject obj = null;
        if(spawnableObject){
            obj = Instantiate(spawnableObject, spawnPos, transform.rotation);
        }
        spawnable = obj.transform.GetComponent<ISpawnable>();
        return spawnable;
    }


    void OnDrawGizmos(){
        Gizmos.DrawWireSphere(transform.position + centerOffset, radius);
        Gizmos.DrawIcon(transform.position + Vector3.up * groundAdjustmentHeight, "curvekeyframe", true);
    }

    public void Destroy(){
        Destroy(gameObject);
    }

    public GameObject GetGameObject()
    {
        return gameObject;
    }
}
