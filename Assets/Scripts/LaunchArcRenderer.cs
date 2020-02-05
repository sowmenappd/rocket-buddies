using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent (typeof (LineRenderer))]
public class LaunchArcRenderer : MonoBehaviour {

  LineRenderer lr;

  public float velocity;
  public float angle;
  public int resolution = 10;

  float g;
  float radianAngle;

  void SetArcProperties (float velocity, float angle) {
    this.velocity = velocity;
    this.angle = angle;
    RenderArc ();
  }

  void Awake () {
    lr = GetComponent<LineRenderer> ();
    g = Mathf.Abs (Physics.gravity.y);
  }

  // Start is called before the first frame update
  void Start () {
    RenderArc ();
  }

  public void RenderArc () {
    lr.SetVertexCount (resolution + 1);
    lr.SetPositions (CalculateArcArray ());
    transform.localEulerAngles = new Vector3 (angle, 0, 0);
  }

  Vector3[] CalculateArcArray () {
    Vector3[] arcArray = new Vector3[resolution + 1];
    radianAngle = Mathf.Deg2Rad * angle;
    float maxDistance = Mathf.Pow (velocity, 2) * Mathf.Sin (2 * radianAngle) / g;
    for (int i = 0; i <= resolution; i++) {
      float t = (float) i / (float) resolution;
      arcArray[i] = GetArcPoint (t, maxDistance);
    }

    return arcArray;
  }

  Vector3 GetArcPoint (float t, float maxDist) {
    float z = t * maxDist;
    float y = 0 + z * Mathf.Tan (radianAngle) - ((g * z * z) / (2 * velocity * velocity * Mathf.Cos (radianAngle)));

    return new Vector3 (0, y, z);
  }

}