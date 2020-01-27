using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : LivingEntity{

    public int damage;

    protected override void Start(){
        base.Start();
    }

    void Update(){
        if(alive && health <= 0) base.Die();
        if(!alive) return;
    }

    protected virtual void Fire(Transform target){
        print("Enemy fired rockets.");
    }

    public override void TakeDamage(int damage){
        base.TakeDamage(damage);
        print("Enemy dead.");
    }

    protected override void Die(){
        alive = false;
    }
}
