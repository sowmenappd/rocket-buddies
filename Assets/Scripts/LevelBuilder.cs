using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelBuilder : MonoBehaviour
{
    public void MainTask(){
        TaskBuilder.I.Chain(() => {
            MapBuilder.I.GenerateMesh();
            var sizeX = GridBuilder.I.sizeX;
            var sizeY = GridBuilder.I.sizeY;
            var diam = GridBuilder.I.nodeDiameter;
            GridBuilder.I.GenerateGrid(sizeX, sizeY, diam);
            FindObjectOfType<FenceBuilder>().GenerateFence();
            MapObjectSpawner.I.SpawnAllItems();
        })
        .ExecuteTaskChain();
    }

    public void CleanCachedItems(){
        TaskBuilder.I.Chain(() => {
            MapObjectSpawner.I.DeleteExistingItems();
            FindObjectOfType<FenceBuilder>().DeleteExistingItems();
            MapBuilder.I.GenerateMesh();
            var sizeX = GridBuilder.I.sizeX;
            var sizeY = GridBuilder.I.sizeY;
            var diam = GridBuilder.I.nodeDiameter;
            GridBuilder.I.GenerateGrid(sizeX, sizeY, diam);
        }).ExecuteTaskChain();
    }
}
