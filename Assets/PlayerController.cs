using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : LivingEntity
{
    public float moveSpeed; 
    public float camRotationSpeed;
    public float jumpForce;

    Camera camera;
    Rigidbody rb;
    public Vector3 Velocity{
        get {
            return rb.velocity;
        }
    }

    Vector3 dir;
    float cameraRotationH, cameraRotationV;

    public bool isGrounded;

    public WeaponHolder holder;

    public Weapon activeWeapon;

    public static PlayerController Instance {
        get {
            return instance;
        }
    }
    private static PlayerController instance;
    
    protected override void Start(){
        instance = this;
        base.Start();
        camera = transform.GetChild(0).GetComponent<Camera>();
        rb = GetComponent<Rigidbody>();
        holder = transform.GetChild(0).GetChild(0).GetComponent<WeaponHolder>();
        activeWeapon = holder.weapons[holder.activeWeaponIndex];
        activeWeapon.Equip();
    }

    void Update(){
        if(!alive) return;

        Movement();
        CameraLook();
        Jump();
        Fire();
        SwitchWeapons();
    }

    void Jump(){
        if(Input.GetKeyDown(KeyCode.Space) && isGrounded){
            isGrounded = false;
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
        }
    }

    void SwitchWeapons(){
        if(Input.GetKeyDown(KeyCode.Q)){
            holder.CycleNextWeapon();
        } else if(Input.GetKeyDown(KeyCode.E)){
            holder.CyclePreviousWeapon();
        }
        activeWeapon = holder.weapons[holder.activeWeaponIndex];
    }

    protected virtual void Fire(){
        if(!Input.GetMouseButtonDown(0)) return;
        print("Firing rocket.");
        activeWeapon.Fire();
    }


    public override void TakeDamage(int damage){
        base.TakeDamage(damage);
    }

    void CameraLook(){
        if(Input.GetAxis("Mouse X") != 0){
            cameraRotationH += Input.GetAxis("Mouse X") > 0 ? camRotationSpeed : -camRotationSpeed;
        }
        if(Input.GetAxis("Mouse Y") != 0){
            cameraRotationV += Input.GetAxis("Mouse Y") > 0 ? -camRotationSpeed : camRotationSpeed;
        }
        transform.eulerAngles = new Vector3(0, cameraRotationH, 0);
        camera.transform.localEulerAngles = new Vector3(Mathf.Clamp(cameraRotationV, -45, 75), 0, 0);
    }

    void Movement(){
        dir = Vector3.zero;
        if(Input.GetKey(KeyCode.W)){
            dir += Vector3.forward * Time.deltaTime * moveSpeed;
        } else if (Input.GetKey(KeyCode.S)){
            dir -= Vector3.forward * Time.deltaTime * moveSpeed;
        }

        if(Input.GetKey(KeyCode.A)){
            dir -= Vector3.right * Time.deltaTime * moveSpeed;
        } else if (Input.GetKey(KeyCode.D)){
            dir += Vector3.right * Time.deltaTime * moveSpeed;
        }

        transform.Translate(dir);
    }

    protected override void Die(){
        base.Die();
        print("Player died.");
    }

    void OnCollisionEnter(Collision col){
        isGrounded = true;
    }
}
