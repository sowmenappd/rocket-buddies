using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelBuilder : MonoBehaviour
{
    public void MainTask(){
        TaskBuilder.I.Chain(() => {
            MapBuilder.I.GenerateMesh();
            MonoBehaviour.print("1");
        })
        .Chain(() => {
            MonoBehaviour.print("2");
            var sizeX = GridBuilder.I.sizeX;
            var sizeY = GridBuilder.I.sizeY;
            var diam = GridBuilder.I.nodeDiameter;
            GridBuilder.I.GenerateGrid(sizeX, sizeY, diam);
        })
        .Chain(() => {
            MapObjectSpawner.I.SpawnAllItems();
        })
        .ExecuteTaskChain();
    }
}
