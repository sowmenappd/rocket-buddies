using UnityEngine;
using System.Diagnostics;

public class Tree : SpawnableObject {

    public bool ignoreGroundNormal;

    public override ISpawnable Spawn(Vector3 spawnPos){
        base.Spawn(spawnPos);
        RandomRotate();
        return spawnable;
    }

    public void RandomRotate(){
        transform.eulerAngles += Vector3.up * Random.Range(0f, 180f);
    }

}