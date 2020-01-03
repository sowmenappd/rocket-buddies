using UnityEngine;
using System.Diagnostics;

public class Tree : SpawnableObject {

    public override ISpawnable Spawn(UnityEngine.Vector3 spawnPos){
        base.Spawn(spawnPos + GetGroundContactPointLocal());
        return spawnable;
    }

}