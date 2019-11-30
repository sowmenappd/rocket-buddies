using System;
using UnityEngine;

[System.Serializable]
public struct RegionType : IComparable{
    public string regionName;
    [Range(0,1f)] public float maxHeightPercentage; //x for min, y for max
    public Color regionColor;
    public bool walkable;


    public int CompareTo(object obj)
    {
        RegionType b = (RegionType) obj;
        return (maxHeightPercentage < b.maxHeightPercentage) ? 1 : (maxHeightPercentage == b.maxHeightPercentage) ? 0 : -1;
    }

    public static Color ColorFromString(string s){
        var parts = s.Split('(', ')', ',', ' ');
        if(parts.Length > 4)
            return Color.blue;
        else{
            Color c = new Color(float.Parse(parts[0]), float.Parse(parts[1]), float.Parse(parts[2]));
            return c;
        }
    }
}
