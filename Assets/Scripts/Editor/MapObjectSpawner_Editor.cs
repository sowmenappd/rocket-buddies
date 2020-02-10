using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(MapObjectSpawner))]
public class MapObjectSpawner_Editor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        if(GUILayout.Button("Toggle Spawnables Gizmos"))
        {
            SpawnableObject.debug = !SpawnableObject.debug;
        }
    }
}
