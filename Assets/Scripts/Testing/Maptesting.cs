using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Maptesting : MonoBehaviour
{

    MapBuilder builder;
    // Start is called before the first frame update
    void Start()
    {
        builder = GetComponent<MapBuilder>();
    }

    // Update is called once per frame
    void Update()
    {
        //builder.GenerateMesh();
        //builder.mapOffset += Vector2.left * Time.deltaTime * 2f;
        //transform.Rotate(0, Time.deltaTime * 10, 0);
    }
}
