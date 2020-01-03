using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(LevelBuilder))]
public class LevelBuilder_Editor : Editor
{
    LevelBuilder levelBuilder;

    public override void OnInspectorGUI(){
        base.OnInspectorGUI();

        if(GUILayout.Button("Build All Tasks")){
            levelBuilder = (LevelBuilder) target;
            levelBuilder.MainTask();
        }
    }
}
