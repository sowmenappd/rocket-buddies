using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(MapBuilder))]
public class MapBuilder_Editor : Editor
{   

    bool showButton;
    public override void OnInspectorGUI(){
        MapBuilder builder = target as MapBuilder;
        MapBuilder.Instance = builder;
        
        if(DrawDefaultInspector()){
            if(builder.autoUpdate){
                builder.GenerateMesh();
            }
        }

        if(GUILayout.Button("Generate Map")){
            builder.GenerateMesh();
        }

    }
}
