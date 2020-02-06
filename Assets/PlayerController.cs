using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : LivingEntity {
  public float moveSpeed;
  public float rotationSpeed;
  public float jumpForce;

  Rigidbody rb;
  public Vector3 Velocity {
    get {
      return rb.velocity;
    }
  }

  Vector3 dir;
  float cameraRotationH;

  Animator animator;

  public bool isGrounded;

  public WeaponHolder holder;

  public Weapon activeWeapon;

  public static PlayerController Instance {
    get {
      return instance;
    }
  }
  private static PlayerController instance;

  public bool IsIdle {
    get {
      return animator.GetFloat ("moveX") == 0 && animator.GetFloat ("moveY") == 0;
    }
  }

  protected override void Start () {
    base.Start ();
    instance = this;
    rb = GetComponent<Rigidbody> ();
    animator = GetComponent<Animator> ();
    holder = transform.GetChild (0).GetComponent<WeaponHolder> ();
    activeWeapon = holder.weapons[holder.activeWeaponIndex];
    activeWeapon.Equip ();

    Cursor.lockState = CursorLockMode.Locked;
    Cursor.visible = false;
    SetReloadAnimationCallback();
  }

  void Update () {
    if (Input.GetKeyDown (KeyCode.Escape)) Cursor.visible = !Cursor.visible;
    if (!alive) return;

    Movement ();
    Rotate(CalculateRotation ());
    Jump ();
    Fire ();
    SwitchWeapons ();

    if (Input.GetKeyDown (KeyCode.R)) {
      animator.SetTrigger("reload");
    }
  }

  void Jump () {
    if (Input.GetKeyDown (KeyCode.Space) && isGrounded) {
      isGrounded = false;
      rb.AddForce (Vector3.up * jumpForce, ForceMode.Impulse);
    }
  }

  void SetReloadAnimationCallback(){
    //var controller = animator;
    //var c = controller.runtimeAnimatorController.animationClips.First(clip => clip.name == "Rifle Reloading");
    //var reloadEvent = c.events.FirstOrDefault(e => e.functionName == "ReloadAmmo");
    //reloadEvent.functionName = "SendReloadAmmoMsg";
  }

  void ReloadAmmo(){
    activeWeapon.SendMessage("ReloadAmmo");
  }

  void SwitchWeapons () {
    if (Input.GetKeyDown (KeyCode.Q)) {
      holder.CycleNextWeapon ();
    } else if (Input.GetKeyDown (KeyCode.E)) {
      holder.CyclePreviousWeapon ();
    }
    activeWeapon = holder.weapons[holder.activeWeaponIndex];
    activeWeapon.Equip ();
  }

  protected virtual void Fire () {
    if (!Input.GetMouseButtonDown (0)) return;
    print ("Firing rocket.");
    activeWeapon.Fire ();
  }

  public override void TakeDamage (int damage) {
    base.TakeDamage (damage);
  }

  float CalculateRotation () {
    if (Input.GetAxis ("Mouse X") != 0) {
      cameraRotationH += Input.GetAxis ("Mouse X") > 0 ? rotationSpeed : -rotationSpeed;
    }
    return cameraRotationH;
  }

  void Rotate(float cameraRotation){
    float y = Mathf.LerpAngle(transform.eulerAngles.y, cameraRotation, rotationSpeed * Time.deltaTime);
    transform.eulerAngles = new Vector3 (0, y, 0);
  }

  void Movement () {
    dir = Vector3.zero;
    if (Input.GetKey (KeyCode.W)) {
      dir += Vector3.forward;
    } else if (Input.GetKey (KeyCode.S)) {
      dir -= Vector3.forward;
    }

    if (Input.GetKey (KeyCode.A)) {
      dir -= Vector3.right;
    } else if (Input.GetKey (KeyCode.D)) {
      dir += Vector3.right;
    }

    transform.Translate (dir.normalized * moveSpeed * Time.deltaTime);
    ProcessAnimation (dir);
  }

  void ProcessAnimation (Vector3 moveDir) {
    float moveX = moveDir.x > 0 ? 1 : moveDir.x < 0 ? -1 : 0;
    float moveY = moveDir.z > 0 ? 1 : moveDir.z < 0 ? -1 : 0;

    Vector2 animDir = new Vector2 (moveX, moveY);
    //animDir.Normalize();

    animator.SetFloat ("moveX", animDir.x);
    animator.SetFloat ("moveY", animDir.y);

    animator.SetBool ("grounded", isGrounded);
  }

  protected override void Die () {
    base.Die ();
    print ("Player died.");
  }

  void OnCollisionEnter (Collision col) {
    isGrounded = true;
  }
}