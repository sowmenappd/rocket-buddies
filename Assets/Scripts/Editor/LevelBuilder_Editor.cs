using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(LevelBuilder))]
public class LevelBuilder_Editor : Editor
{
    LevelBuilder levelBuilder;

    public override void OnInspectorGUI(){
        base.OnInspectorGUI();

        if(GUILayout.Button("Execute Task: Build Map")){
            levelBuilder = (LevelBuilder) target;
            levelBuilder.MainTask();
        }

        if(GUILayout.Button("Execute Task: Clean Cached Objects")){
            levelBuilder = (LevelBuilder) target;
            levelBuilder.CleanCachedItems();
        }
    }
}
