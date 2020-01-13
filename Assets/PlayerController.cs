using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float moveSpeed; 
    public float camRotationSpeed;
    public float jumpForce;

    Camera camera;
    Rigidbody rb;

    Vector3 dir;
    float camerRotationH, cameraRotationV;

    public bool isGrounded;
    
    // Start is called before the first frame update
    void Start()
    {
        camera = transform.GetChild(0).GetComponent<Camera>();
        rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        Movement();
        CameraLook();
        Jump();
    }

    void Jump(){
        if(Input.GetKeyDown(KeyCode.Space)){
            isGrounded = false;
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
        }
    }

    void CameraLook(){
        if(Input.GetAxis("Mouse X") != 0){
            camerRotationH += Input.GetAxis("Mouse X") > 0 ? camRotationSpeed : -camRotationSpeed;
        }
        if(Input.GetAxis("Mouse Y") != 0){
            cameraRotationV += Input.GetAxis("Mouse Y") > 0 ? -camRotationSpeed : camRotationSpeed;
        }
        transform.eulerAngles = new Vector3(0, camerRotationH, 0);
        camera.transform.localEulerAngles = new Vector3(Mathf.Clamp(cameraRotationV, -45, 45), 0, 0);
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

        // float slowX = 0, slowZ = 0;
        // if(!isGrounded) {
        //     slowX = Mathf.Lerp(dir.x, 0, Time.deltaTime);
        //     slowZ = Mathf.Lerp(dir.z, 0, Time.deltaTime);
        // }

        transform.Translate(dir);
    }

    void OnCollisionEnter(Collision col){
        isGrounded = true;
    }
}
