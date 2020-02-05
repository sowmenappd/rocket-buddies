using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public Transform player;
    public float lookUpOffset;

    public float followSpeed;

    public Vector3 followPoint;

    //Camera camera;

    // Start is called before the first frame update
    void Start()
    {
        player = FindObjectOfType<PlayerController>().transform;
        //camera = GetComponent<Camera>();
    }

    // Update is called once per frame
    void Update()
    {
        transform.LookAt(player.position + Vector3.up * lookUpOffset);
        var followPos = player.TransformPoint(followPoint);
        transform.position = Vector3.Lerp(transform.position, followPos, followSpeed * Time.deltaTime);
    }
}
