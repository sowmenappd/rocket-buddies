﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(GridBuilder))]
public class GridBuilder_Editor : Editor
{
    public override void OnInspectorGUI(){
        base.OnInspectorGUI();

        if(GUILayout.Button("Build Gridmap")){
            var t = target as GridBuilder;
            GridBuilder.I = t;
            t?.GenerateGrid(t.sizeX, t.sizeY, t.nodeDiameter);
        }

        if(GUILayout.Button("Toggle: Display Only Unwalkable Nodes"))
        {
            var t = target as GridBuilder;
            t.displayOnlyUnwwalkable = !t.displayOnlyUnwwalkable;
        }
    }
}
