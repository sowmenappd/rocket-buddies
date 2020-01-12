using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FenceBuilder : MonoBehaviour
{
    public GameObject fencePrefab;

    public Rect fenceBounds;

    public float testGap;

    void Start(){
        //bounds.
    }

    void Update(){
        
    }

    void OnDrawGizmos(){
        if(fenceBounds != null){
            Gizmos.DrawWireCube(new Vector3(fenceBounds.x, 0, fenceBounds.y), new Vector3(fenceBounds.size.x, 1f, fenceBounds.size.y));
            Gizmos.DrawWireSphere(fenceBounds.position, 2f);
        }
        
        var points = GetOutlinePoints(testGap);
        for(int i=0; i< points.Count; i++){
            var pt = points[i];
            ElevatePointToSurface(ref pt);
            Gizmos.DrawSphere(pt, 1f);
        }
    }

    void ElevatePointToSurface(ref Vector3 pt){
        var lookDownPt = new Vector3(pt.x, 150, pt.z);
        RaycastHit hitInfo;
        if(Physics.Raycast(lookDownPt, Vector3.down, out hitInfo, 200f)){
            pt.y = hitInfo.point.y;
        }
    }

    List<Vector3> GetOutlinePoints(float distancePerPoint){
        if(distancePerPoint <= 0) distancePerPoint = 0.05f;

        var list = new List<Vector3>();
        
        var center = fenceBounds.position;
        var halfSizeX = fenceBounds.size.x / 2;
        var halfSizeZ = fenceBounds.size.y / 2;

        var topLeft = new Vector3(center.x - halfSizeX, 0, center.y - halfSizeZ);
        var topRight = new Vector3(center.x + halfSizeX, 0, center.y - halfSizeZ);
        var bottomLeft = new Vector3(center.x - halfSizeX, 0, center.y + halfSizeZ);
        var bottomRight = new Vector3(center.x + halfSizeX, 0, center.y + halfSizeZ);

        //get all distancePerPoints spaced points between topLeft and topRight 
        for(float i = topLeft.x; i < topRight.x; i += distancePerPoint){
            var point = new Vector3(i, 0, topLeft.z);
            list.Add(point);
        }

        //topLeft and bottomLeft
        for(float i = topLeft.z + 1.75f; i < bottomLeft.z; i += distancePerPoint){
            var point = new Vector3(topLeft.x, 0, i);
            list.Add(point);
        }

        //topRight and bottomRight
        for(float i = topRight.z + 1.75f; i < bottomRight.z; i += distancePerPoint){
            var point = new Vector3(topRight.x, 0, i);
            list.Add(point);
        }

        //bottomLeft and bottomRight
        for(float i = bottomLeft.x; i < bottomRight.x; i += distancePerPoint){
            var point = new Vector3(i, 0, bottomLeft.z);
            list.Add(point);
        }
        
        return list;    
    }
}
