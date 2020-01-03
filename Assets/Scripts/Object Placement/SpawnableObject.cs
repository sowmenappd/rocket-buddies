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
        print("Spawned " + spawnableObject + " as " + name);
        GameObject obj = null;
        if(spawnableObject){
            obj = Instantiate(spawnableObject, spawnPos, spawnableObject.transform.rotation);
        }
        spawnable = obj.transform.GetComponent<ISpawnable>();
        return spawnable;
    }

    public Vector3 GetGroundContactPointLocal(){
        return new Vector3(centerOffset.x, -groundAdjustmentHeight, centerOffset.z);
    }

    void OnDrawGizmos(){
        Gizmos.DrawWireSphere(transform.position + centerOffset, radius);
        Gizmos.DrawIcon(transform.position + GetGroundContactPointLocal(), "curvekeyframe", true);
    }

    public void Destroy(){
        Destroy(gameObject);
    }
}
