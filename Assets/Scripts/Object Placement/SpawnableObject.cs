﻿using System.Collections;
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

    public static bool debug = true; 

    public virtual ISpawnable Spawn(Vector3 spawnPos)
    {
        //if(!spawnableObject) return new GameObject("NULL").AddComponent<ISpawnable>();
        GameObject obj = null;
        if(spawnableObject != null){
            obj = Instantiate(spawnableObject, spawnPos, transform.rotation);
            obj.gameObject.name = spawnableObject.name;
            spawnable = obj.transform.GetComponent<ISpawnable>();
        }
        return spawnable;
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
