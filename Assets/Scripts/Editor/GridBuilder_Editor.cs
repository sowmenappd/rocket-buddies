using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(GridBuilder))]
public class GridBuilder_Editor : Editor
{

    TaskBuilder taskBuilder;

    public override void OnInspectorGUI(){
        base.OnInspectorGUI();

        if(GUILayout.Button("Build Gridmap")){
            var t = target as GridBuilder;
            GridBuilder.I = t;
            t?.GenerateGrid(t.sizeX, t.sizeY, t.nodeDiameter);
        }

        if(GUILayout.Button("Build All Tasks")){
            taskBuilder = TaskBuilder.Instance;
            taskBuilder.Task1();
        }

    }
}
