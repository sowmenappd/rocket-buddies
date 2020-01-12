using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FenceBuilder : MonoBehaviour
{
    public GameObject fencePrefab;

    public Rect fenceBounds;

    public float testGap;
    public float fenceHeight;

    public float val1;

    public LayerMask collisionMask;

    private List<Vector3> points;
    private List<GameObject> instFences = new List<GameObject>();

    public void DeleteExistingItems(){
        if(instFences.Count > 0){
            instFences.ForEach(fence => {
                if(fence){GameObject.DestroyImmediate(fence);
                }});
        }
    }

    public void GenerateFence(){
        DeleteExistingItems();

        points = GetOutlinePoints(testGap);
        for(int i=0; i< points.Count; i++){
            points[i] = ElevatePointToSurface(points[i]);
            if(fencePrefab != null){
                var fence = Instantiate(fencePrefab, points[i] - Vector3.up * fenceHeight, Quaternion.identity, transform);
                RaycastHit hit;
                fence.transform.localScale = new Vector3(1, 4, 1);
                if(Physics.Raycast(points[i] + Vector3.up * 100f, Vector3.down, out hit, collisionMask)){
                    //fence.transform.up = hit.normal;
                    Debug.DrawRay(fence.transform.position, hit.normal * 10f, Color.red);
                }
                fence.transform.eulerAngles = GetFenceRotation(fence.transform);
                //fence.transform.rotation = Quaternion.LookRotation(fence.transform.right, hit.normal);
                //fence.transform.eulerAngles = GetFenceRotation(fence.transform);
                //fence.transform.rotation = Quaternion.LookRotation(transform.forward, GetGroundUp(fence));
                instFences.Add(fence);
            }
        }
    }

    Vector3 GetFenceRotation(Transform t){
        bool isLeftRightSide = Mathf.Abs(t.position.x) == fenceBounds.width/2f;//(fenceBounds.width/2 + position.x) < 4 || (fenceBounds.width/2 + position.x) > fenceBounds.width/2;
        bool isUpDownSide = Mathf.Abs(t.position.z) == fenceBounds.height/2f;//(fenceBounds.height/2 + position.z) < 4 || (fenceBounds.height/2 + position.z) > fenceBounds.height/2;

        Quaternion q = Quaternion.identity;
        if(isLeftRightSide){
            return new Vector3(t.eulerAngles.x, 90, t.eulerAngles.z);
        } else{
            return new Vector3(t.eulerAngles.x, 0, t.eulerAngles.z);
        }
    }

    Vector3 GetGroundUp(GameObject fence){
        RaycastHit hit;
        if(Physics.Raycast(fence.transform.position + Vector3.up * 2f, Vector3.down, out hit, 1000f, collisionMask)){
            fence.transform.up = hit.normal;
        }
        return hit.normal;
    }

    void OnDrawGizmos(){
        if(fenceBounds != null){
            Gizmos.DrawWireCube(new Vector3(fenceBounds.x, 0, fenceBounds.y), new Vector3(fenceBounds.size.x, 1f, fenceBounds.size.y));
            Gizmos.DrawWireSphere(fenceBounds.position, 2f);
        }

        // if(points == null || points.Count == 0) return;
        // for(int i=0; i< points.Count; i++){
        //     Gizmos.DrawSphere(points[i], 1f);
        // }
        
    }

    Vector3 ElevatePointToSurface(Vector3 pt){
        var lookDownPt = new Vector3(pt.x, 150, pt.z);
        RaycastHit hitInfo;
        if(Physics.Raycast(lookDownPt, Vector3.down, out hitInfo, collisionMask)){
            pt.y = hitInfo.point.y;
        }
        return pt;
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
        for(float i = topLeft.x; i <= topRight.x + testGap; i += distancePerPoint){
            var point = new Vector3(i, 0, topLeft.z);
            list.Add(point);
        }

        //topLeft and bottomLeft
        for(float i = topLeft.z; i <= bottomLeft.z + testGap; i += distancePerPoint){
            var point = new Vector3(topLeft.x, 0, i);
            list.Add(point);
        }

        //topRight and bottomRight
        for(float i = topRight.z; i <= bottomRight.z + testGap; i += distancePerPoint){
            var point = new Vector3(topRight.x, 0, i);
            list.Add(point);
        }

        //bottomLeft and bottomRight
        for(float i = bottomLeft.x; i <= bottomRight.x + testGap; i += distancePerPoint){
            var point = new Vector3(i, 0, bottomLeft.z);
            list.Add(point);
        }
        
        return list;    
    }
}
