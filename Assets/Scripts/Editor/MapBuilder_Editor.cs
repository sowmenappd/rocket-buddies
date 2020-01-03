using System.IO;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(MapBuilder))]
public class MapBuilder_Editor : Editor
{   

    string savePath;

    bool showButton;
    public override void OnInspectorGUI(){
        MapBuilder builder = MapBuilder.I;
        
        if(DrawDefaultInspector()){
            if(builder.autoUpdate){
                builder.GenerateMesh();
            }
        }

        if(GUILayout.Button("Generate Map")){
            builder.GenerateMesh();
        }

        if(GUILayout.Button("Save Region Values")){
            string data = builder.GetAllRegionData();
            System.IO.File.WriteAllText(savePath + "data" + GetFileNumber() + ".txt", data);
        }

    }

    void OnEnable(){
        savePath = Application.dataPath + "/regionData/";
    }

    int GetFileNumber(){
        int n = 0;
        while(File.Exists(savePath + "data" + n + ".txt")){
            n++;
        }
        return n;
    }
}
