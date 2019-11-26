using UnityEngine;

[System.Serializable]
public class MeshData{
    public string regionName;
    [RangeAttribute(-10, 10)] public float heighRange;
    public Color regionColor;
    public bool walkable = true;
}
