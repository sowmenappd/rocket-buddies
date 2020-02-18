using UnityEngine;
using System.Diagnostics;

public class Tree : SpawnableObject {
    
    public override ISpawnable Spawn(Vector3 spawnPos){
        base.Spawn(spawnPos);
        base.RandomRotate();
        return spawnable;
    }

}