using UnityEngine;
using System.Diagnostics;

public class Tree : SpawnableObject {

    public override ISpawnable Spawn(Vector3 spawnPos){
        base.Spawn(spawnPos + Vector3.up * groundAdjustmentHeight);
        RandomRotate();
        return spawnable;
    }

    private void RandomRotate(){
        transform.eulerAngles += Vector3.up * Random.Range(0f, 180f);
        //transform.rotation = Quaternion.Euler(transform.rotation.ToEuler().x, Random.Range(0f, 180f), transform.rotation.ToEuler().z);
        print(transform.rotation.ToString());
    }

}