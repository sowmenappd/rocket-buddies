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

    protected ISpawnable spawnable;

    public virtual ISpawnable Spawn(Vector3 spawnPos)
    {
        //if(!spawnableObject) return new GameObject("NULL").AddComponent<ISpawnable>();
        GameObject obj = null;
        if(spawnableObject){
            obj = Instantiate(spawnableObject, spawnPos, transform.rotation);
            spawnable = obj.transform.GetComponent<ISpawnable>();
        }
        return spawnable;
    }

    void OnDrawGizmos(){
        Gizmos.DrawWireSphere(transform.position + centerOffset, radius);
    }

    public void Destroy(){
        Destroy(gameObject);
    }

    public GameObject GetGameObject(){
        return gameObject;
    }
}
