using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : LivingEntity {

  public const float MAX_STAMINA = 100;
  public float stamina;
  public int damage;

  public float moveSpeed;

  public Vector2 attackRangeMinMax;
  float currentAttackRange;

  Weapon activeWeapon;
  Transform player;

  public enum State { Idle, Moving, Fleeing, Attacking, Reloading }
  public State state;

  State lastState;

  Animator animator;

  Vector3 animMoveDirection;

  const float onStatePersistAttackRangeChangeTimer = 3f;
  float currentAttackStateTimer = 0;


  protected override void Start () {
    base.Start ();
    state = State.Idle;
    lastState = state;
    stamina = MAX_STAMINA;
    player = PlayerController.Instance.transform;
    animator = GetComponent<Animator> ();

    activeWeapon = transform.GetChild (0).GetChild (0).GetComponent<Weapon> ();
    activeWeapon.Equip ();

    currentAttackRange = (attackRangeMinMax.x + attackRangeMinMax.y) / 2;

    state = State.Idle;
    rb = GetComponent<Rigidbody>();
  }

  void Update () {
    if (!alive) return;

    state = CalculateNextState (state);
    if(state == State.Attacking && lastState == State.Attacking){
      currentAttackStateTimer += Time.deltaTime;
      if(currentAttackStateTimer > onStatePersistAttackRangeChangeTimer){
        ChangeAttackRange();
        currentAttackStateTimer = 0;
      }
    }
    
    PerformStateActions(state);
    ApplyAnimation (state);
    lastState = state;
  }

  void ChangeAttackRange(){
    currentAttackRange = Random.Range(attackRangeMinMax.x, attackRangeMinMax.y);
  }

  void PerformStateActions(State state){
    switch(state){
      case State.Moving:
      case State.Fleeing:
        ChasePlayer();
        break;
      case State.Attacking:
        AttackPlayer();
        break;
    }
    return;
  }

  Rigidbody rb;

  void ChasePlayer(){
    Vector3 dir = (player.position - transform.position);
    animMoveDirection = dir.normalized;
    FaceDirection(animMoveDirection);
    transform.Translate(dir.normalized * moveSpeed * Time.deltaTime);
    print("Moving");
  }

  void AttackPlayer(){
    Vector3 dir = (player.position - transform.position);
    FaceDirection(dir.normalized);
    print("Attacking");
  }

  void ApplyAnimation (State state) {
    switch (state) {
      case State.Idle:
        animator.SetFloat ("moveX", 0);
        animator.SetFloat ("moveY", 0);
        break;
      case State.Moving:
      case State.Fleeing:
        float moveX = animMoveDirection.x > 0 ? 1 : animMoveDirection.x < 0 ? -1 : 0;
        float moveY = animMoveDirection.z > 0 ? 1 : animMoveDirection.z < 0 ? -1 : 0;
        Vector2 animDir = new Vector2 (moveX, moveY);
        animator.SetFloat ("moveX", animDir.x);
        animator.SetFloat ("moveY", animDir.y);
        break;
      case State.Attacking:
        animator.SetFloat ("moveX", 0);
        animator.SetFloat ("moveY", 0);
        break;
      case State.Reloading:
        if(animator.GetCurrentAnimatorStateInfo(1).IsName("Idle"))
        animator.SetTrigger("reload");
        break;
    }
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
        if (IsTargetWithinAttackRange (player)) {
          if (IsWeaponLoaded (activeWeapon)) {
            return State.Attacking;
          } else {
            return State.Reloading;
          }
        } else {
          return State.Moving;
        }
      } else {
        return State.Idle;
      }
    } else if (state == State.Reloading) {
      if (stamina > 0) {
        return State.Fleeing;
      } else {
        return State.Idle;
      }
    }
    return state;
  }

  void FaceDirection(Vector3 dir){
    var alignmentDir = new Vector3(dir.x, 0, dir.z);
    RotateToDirection(alignmentDir);
  }

  void RotateToDirection(Vector3 direction){
    transform.rotation = Quaternion.LookRotation(direction, Vector3.up);
    //float y = Mathf.LerpAngle(transform.eulerAngles.y, cameraRotation, rotationSpeed * Time.deltaTime);
    //transform.eulerAngles = new Vector3 (transform.eulerAngles.x, y, transform.eulerAngles.z);
  }

  bool IsWeaponLoaded (Weapon weapon) {
    return (weapon.currentAmmo > 0 && weapon.canFire && !weapon.switching);
  }

  bool IsTargetWithinAttackRange (Transform attackTarget) {
    return (attackTarget.position - transform.position).sqrMagnitude <= currentAttackRange * currentAttackRange;
  }

  bool IsAttackIncoming (Transform fromTarget) {
    return false;
  }

  protected virtual void Fire (Transform target) {
    print ("Enemy fired rockets.");
  }

  public override void TakeDamage (int damage) {
    base.TakeDamage (damage);
  }

  protected override void Die () {
    alive = false;
    print ("Enemy dead.");
  }
}