﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RocketLauncher : Weapon{

    public Transform rocketSpawn;
    public Rocket rocketPrefab;

    public float launchForce;

    public Vector2 recoilForces;
    public float recoilDuration;
    public float maxRecoilLocalDistance;

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

    IEnumerator WeaponRecoil(){
        //if(!canFire) yield break;
        float duration = recoilDuration;
        float speed = 1 / duration;

        float t = 0;

        var normalPos = transform.localPosition;
        var targetPos = normalPos - Vector3.forward * maxRecoilLocalDistance;

        while(t < duration / 2f){
            t += Time.deltaTime;
            transform.localPosition = Vector3.Lerp(transform.localPosition, targetPos, Time.deltaTime * speed);
            yield return null;
        }
        //transform.localPosition = targetPos;

        while(t < duration){
            t += Time.deltaTime;
            transform.localPosition = Vector3.Lerp(transform.localPosition, normalPos, Time.deltaTime * speed);
            yield return null;
        }
        transform.localPosition = normalPos;
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
        StartCoroutine(WeaponRecoil());
    }
}
