using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RocketLauncher : Weapon{

    public Transform rocketSpawn;
    public Rocket rocketPrefab;

    public float launchForce;

    public override void Start(){
        base.Start();
        rocketSpawn = transform.GetChild(0);
    }

    void Update(){
        if(shotTimer > 0){
            shotTimer -= Time.deltaTime;
        } else {
            canFire = true;
        }        
    }

    public override void Fire(){
        if(!canFire || switching) return;
        LaunchRocket();
        canFire = false;
        shotTimer = waitDurationPerShot;
    }

    public void LaunchRocket(){
        var rocket = Instantiate(rocketPrefab, rocketSpawn.position, Quaternion.Euler(transform.forward));
        rocket.owner = transform.root;
        rocket.transform.forward = transform.forward;
        rocket.transform.GetComponent<Rigidbody>().AddForce(PlayerController.Instance.Velocity + rocket.transform.forward * launchForce, ForceMode.Impulse);
    }
}
