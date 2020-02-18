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

    public bool ignoreGroundNormal;
    public bool randomRotation = true;

    protected ISpawnable spawnable;

    public static bool debug = false; 

    public virtual ISpawnable Spawn(Vector3 spawnPos)
    {
        //if(!spawnableObject) return new GameObject("NULL").AddComponent<ISpawnable>();
        GameObject obj = null;
        if(spawnableObject != null){
            obj = Instantiate(spawnableObject, spawnPos, transform.rotation);
            obj.gameObject.name = spawnableObject.name;
            spawnable = obj.transform.GetComponent<ISpawnable>();
            RandomRotate();
        }
        return spawnable;
    }

    public virtual void RandomRotate(){
        if(randomRotation){
            transform.eulerAngles += Vector3.up * Random.Range(0f, 180f);
        }
        if(ignoreGroundNormal){
            transform.eulerAngles = new Vector3(0, transform.eulerAngles.y, 0);
        }
    }

    void OnDrawGizmos(){
        if(debug)
            Gizmos.DrawWireSphere(transform.position + centerOffset, radius);
    }

    public void Destroy(){
        Destroy(gameObject);
    }

    public GameObject GetGameObject(){
        return gameObject;
    }
}
