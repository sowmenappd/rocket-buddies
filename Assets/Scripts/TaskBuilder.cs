using System.Collections.Generic;
using UnityEngine;

public class TaskBuilder
{
    private static TaskBuilder instance;

    public static TaskBuilder Instance {
        get {
            if(instance == null) instance = new TaskBuilder();
            return instance;
        } private set {}
    }
    
    public Queue<System.Action> queue = new Queue<System.Action>();

    public TaskBuilder Chain(System.Action task){
        if(queue == null) queue = new Queue<System.Action>();
        queue.Enqueue(task);
        return this;
    }

    public void ExecuteTaskChain(){
        while(queue.Count > 0){
            var task = queue.Dequeue();
            task?.Invoke();
        }
    }

    public void Task1(){
        Chain(() => {
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
