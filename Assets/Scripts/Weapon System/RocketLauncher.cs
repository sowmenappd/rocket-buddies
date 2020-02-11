using System.Linq;
using System.Collections;
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

  public Vector2 rangeFinderMinMaxVelocity;

  float rfVelocity;

  public override void Start () {
    base.Start ();
    rocketSpawn = transform.GetChild (0);
    if (hasRangeFinder) {
      rangeFinder = rocketSpawn.GetChild (0).GetComponent<LaunchArcRenderer> ();
      rfVelocity = rangeFinderMinMaxVelocity.x;
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
    if (rangeFinder == null || !hasRangeFinder) return;
    if (rangeFinder.botControl) return;

    //control with mouse Y
    float mov = Input.GetAxis ("Mouse Y");

    float amount = mov * rangeFinderMoveSpeed;
    rfVelocity += amount;
    rfVelocity = Mathf.Clamp(rfVelocity, rangeFinderMinMaxVelocity.x, rangeFinderMinMaxVelocity.y);
    rangeFinder.velocity = rfVelocity;
    launchForce = rfVelocity;
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
    if(HasLoadedAmmo()){
      LaunchRocket ();
      canFire = false;
      shotTimer = waitDurationPerShot;
    } else {
      StartWeaponReload();
    }
  }

  bool HasLoadedAmmo(){
    return currentAmmo > 0;
  }

  void ReloadAmmo(){
    print("in reload ammo function");
    if(totalWeaponAmmoCapacity > 0){
      int amountToLoad = Mathf.Min(totalWeaponAmmoCapacity - currentAmmo, magazineAmmoCap - currentAmmo);
      currentAmmo += amountToLoad;
      totalWeaponAmmoCapacity -= amountToLoad;
      canFire = true;
    }
  }

  void StartWeaponReload(){
    if (HasLoadedAmmo()) return;
    canFire = false;
    print("reloading in animator");
    var controller = transform.root.GetComponent<Animator>();
    controller.SetTrigger("reload");
  }

  

  public void LaunchRocket () {
    var rocket = Instantiate (rocketPrefab, rocketSpawn.position, Quaternion.Euler(rocketSpawn.forward));
    rocket.owner = transform.root;
    rocket.transform.forward = transform.forward;
    var a = (rangeFinder.angle) * Mathf.Deg2Rad;
    var launchDir = rocket.transform.TransformDirection(new Vector3(0, Mathf.Sin(a), Mathf.Cos(a)));
    //PlayerController.Instance.Velocity + 
    rocket.transform.GetComponent<Rigidbody> ().AddForce ((launchDir) * launchForce * 1.15f, ForceMode.Impulse);

    currentAmmo--;
    canFire = false;
    shotTimer = waitDurationPerShot;
    if(currentAmmo == 0){
      var controller = transform.root.GetComponent<Animator>();
      controller.SetTrigger("reload");
    }

    StartCoroutine (WeaponRecoil ());
  }
}