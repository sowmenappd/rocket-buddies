using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RocketLauncher : Weapon {

  public Transform rocketSpawn;
  public Rocket rocketPrefab;

  public float launchForce;

  public Vector2 recoilForces;
  public float recoilDuration;
  public float maxRecoilLocalDistance;

  LaunchArcRenderer rangeFinder;
  public float rangeFinderMoveSpeed;

  public bool testing;
  public bool hasRangeFinder;

  public Vector2 rangeFinderMinMaxRotation;

  float rfRotation;

  public override void Start () {
    base.Start ();
    rocketSpawn = transform.GetChild (0);
    if (hasRangeFinder) {
      rangeFinder = rocketSpawn.GetChild (0).GetComponent<LaunchArcRenderer> ();
      rfRotation = rangeFinder.transform.localEulerAngles.y;
    }
  }

  void Update () {
    if (shotTimer > 0) {
      shotTimer -= Time.deltaTime;
    } else {
      canFire = true;
    }

    if (testing) {
      transform.localPosition = equippedPosition;
      transform.localEulerAngles = equippedRotation;
    }
    ProcessRangefinderVerticalMovement ();
  }

  void ProcessRangefinderVerticalMovement () {
    float mov = -Input.GetAxis ("Mouse Y");

    float amount = mov * rangeFinderMoveSpeed;
    rfRotation += amount;
    rfRotation = Mathf.Clamp(rfRotation, rangeFinderMinMaxRotation.x, rangeFinderMinMaxRotation.y);
    rocketSpawn.localEulerAngles = new Vector3 (rfRotation, rocketSpawn.localEulerAngles.y, rocketSpawn.localEulerAngles.z);
  }

  IEnumerator WeaponRecoil () {
    //if(!canFire) yield break;
    Init ();
    float duration = recoilDuration;
    float speed = 1 / duration;

    float t = 0;

    var normalPos = transform.localPosition;
    var targetPos = normalPos - Vector3.forward * maxRecoilLocalDistance;

    while (t < duration / 2f) {
      t += Time.deltaTime;
      transform.localPosition = Vector3.Lerp (transform.localPosition, targetPos, Time.deltaTime * speed);
      yield return null;
    }
    //transform.localPosition = targetPos;

    while (t < duration) {
      t += Time.deltaTime;
      transform.localPosition = Vector3.Lerp (transform.localPosition, normalPos, Time.deltaTime * speed);
      yield return null;
    }
    transform.localPosition = normalPos;
  }

  public override void Fire () {
    if (!canFire || switching) return;
    LaunchRocket ();
    canFire = false;
    shotTimer = waitDurationPerShot;
  }

  public void LaunchRocket () {
    var rocket = Instantiate (rocketPrefab, rocketSpawn.position, Quaternion.Euler (transform.forward));
    rocket.owner = transform.root;
    rocket.transform.forward = transform.forward;
    rocket.transform.GetComponent<Rigidbody> ().AddForce (PlayerController.Instance.Velocity + rocket.transform.forward * launchForce, ForceMode.Impulse);
    StartCoroutine (WeaponRecoil ());
  }
}