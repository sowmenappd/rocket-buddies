using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Test : MonoBehaviour
{

    public Transform ground;
    public Transform rayPoint;

    public float value;


    // Update is called once per frame
    void Update()
    {
        RaycastHit hit;
        if(Physics.Raycast(rayPoint.position, Vector3.down, out hit, 1000f)){
            print("hit");
            Debug.DrawRay(hit.point, hit.normal * 10f, Color.red);
            //transform.rotation *= Quaternion.FromToRotation(transform.up, hit.normal);
            Vector3 groundForward = Vector3.Cross(transform.right, hit.normal);
            Vector3 groundRight = Vector3.Cross(groundForward, hit.normal);
            Vector3 groundUp = Vector3.Cross(groundRight, groundForward);
            transform.rotation = Quaternion.LookRotation(groundForward, groundUp);
            //transform.rotation *= Quaternion.FromToRotation(transform.eulerAngles, new Vector3(transform.eulerAngles.x, value, transform.eulerAngles.z));
        }
    }
}
