using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : LivingEntity {

  public const float MAX_STAMINA = 100;
  public float stamina;
  public int damage;

  public float attackRange = 5f;

  Weapon activeWeapon;
  Transform player;

  public enum State { Idle, Moving, Fleeing, Attacking, Reloading }
  State state;

  protected override void Start () {
    base.Start ();
    state = State.Idle;
    stamina = MAX_STAMINA;
    player = PlayerController.Instance.transform;

    activeWeapon = transform.GetChild (0).GetChild (0).GetComponent<Weapon> ();
    activeWeapon.Equip ();
  }

  void Update () {
    if (alive && health <= 0) base.Die ();
    if (!alive) return;

    state = CalculateNextState (state);
  }

  State CalculateNextState (State state) {
    if (state == State.Idle) {
      if (stamina > 0) {
        if (IsAttackIncoming (player)) {
          return State.Fleeing;
        } else {
          return State.Moving;
        }
      } else {
        if (IsTargetWithinAttackRange (player)) {
          if (IsWeaponLoaded (activeWeapon)) {
            return State.Attacking;
          } else {
            return State.Reloading;
          }
        } else {
          if (stamina > 0) {
            return State.Fleeing;
          } else {
            return State.Idle;
          }
        }
      }

    } else if (state == State.Moving) {
      if (IsTargetWithinAttackRange (player)) {
        if (IsWeaponLoaded (activeWeapon)) {
          return State.Attacking;
        } else {
          return State.Reloading;
        }
      } else {
        if (stamina > 0) {
          return State.Fleeing;
        } else {
          return State.Idle;
        }
      }
    } else if (state == State.Fleeing) {
      if (stamina > 0) {
        if (IsAttackIncoming (player)) {
          return State.Fleeing;
        } else {
          return State.Moving;
        }
      } else {
        if (IsTargetWithinAttackRange (player)) {
          if (IsWeaponLoaded (activeWeapon)) {
            return State.Attacking;
          } else {
            return State.Reloading;
          }
        } else {
          if (stamina > 0) {
            return State.Fleeing;
          } else {
            return State.Idle;
          }
        }
      }
    } else if (state == State.Attacking) {
      if (stamina > 0) {
        return State.Attacking;
      } else {
        return State.Idle;
      }
    }
    else if (state == State.Reloading){
      if(stamina > 0){
        return State.Fleeing;
      } else {
        return State.Idle;
      }
    }
    return state;
  }

  bool IsWeaponLoaded (Weapon weapon) {
    return (weapon.currentAmmo > 0 && weapon.canFire && !weapon.switching);
  }

  bool IsTargetWithinAttackRange (Transform attackTarget) {
    return (attackTarget.position - transform.position).sqrMagnitude <= attackRange;
  }

  bool IsAttackIncoming (Transform fromTarget) {
    return false;
  }

  protected virtual void Fire (Transform target) {
    print ("Enemy fired rockets.");
  }

  public override void TakeDamage (int damage) {
    base.TakeDamage (damage);
    print ("Enemy dead.");
  }

  protected override void Die () {
    alive = false;
  }
}