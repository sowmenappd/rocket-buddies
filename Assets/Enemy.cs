using System.Linq;
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

  public Transform A, B;


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

    public LayerMask obstacleLayer;

  void OnDrawGizmos()
    {
        var path = GetPath(A.position, B.position);
        foreach(var a in path)
        {
            Gizmos.DrawCube(a, Vector3.one * .3f);
        }
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
        ChasePlayer();
        break;
    case State.Fleeing:
        Flee();
        break;
      case State.Attacking:
        AttackPlayer();
        break;
      case State.Reloading:
        ReloadAmmo();
        break;
    }
    return;
  }

  Rigidbody rb;

  void ChasePlayer(){
    Vector3 dir = (player.position - transform.position);
    dir.y = 0;
    animMoveDirection = dir.normalized;
    FaceDirection(animMoveDirection);
    transform.Translate(dir.normalized * moveSpeed * Time.deltaTime);
    print("Moving");
  }

  void AttackPlayer(){
    Vector3 dir = (player.position - transform.position);
    dir.y = 0;
    FaceDirection(dir.normalized);
    Fire();
    print("Attacking");
  }

  void Flee()
  {
        Vector3 dir = (transform.position - player.position);
        dir.y = 0;
        animMoveDirection = dir.normalized;
        FaceDirection(animMoveDirection);
        transform.Translate(animMoveDirection * moveSpeed * Time.deltaTime);
        print("Fleeing");
  }


    public int resolution = 5;

  List<Vector3> GetPath(Vector3 startPos, Vector3 endPos)
    {
        var path = new List<Vector3>();
        var grid = FindObjectOfType<GridBuilder>().grid;
        var gb = FindObjectOfType<GridBuilder>();

        if (grid == null)
        {
            gb.GenerateGrid(gb.sizeX, gb.sizeY, gb.nodeDiameter);
        }
        var v = grid.NodeIndicesFromWorldPostion(startPos);
        var w = grid.NodeIndicesFromWorldPostion(endPos);

        var startNode = grid.nodes[v.y, v.x];
        var endNode = grid.nodes[w.y, w.x];
        float distance = Vector3.Distance(startNode.worldPos, endNode.worldPos);
        resolution = Mathf.RoundToInt(Mathf.Lerp(2f, grid.sizeX, distance/(float)grid.sizeX));
        float step = distance / resolution;
        Vector3 dir = (endNode.worldPos - startNode.worldPos).normalized;
        //dir.y = 0;
        Vector3 currPos = startPos;

        Node lastWalkableNode = startNode;

        for(int i=0; i<=resolution; i++)
        {
            RaycastHit hit;
            var obstacleNode = Physics.Raycast(currPos + Vector3.up * 100f, Vector3.down, out hit, 1000f);
            //process obstacle node

            var node = grid.NodeFromWorldPostion(hit.point);
            if (node.walkable)
            {
                path.Add(node.worldPos + Vector3.up);
                lastWalkableNode = node; 
            }
            else
            {
                //apply floodfill on last walkable node and get another node that is walkable
                //update direction from new walkable node
                //add node to path
            }
            currPos += dir * step;

        }

        //path.Add(startNode.worldPos);
        //path.Add(endNode.worldPos);
        return path;
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
        animator.SetFloat("moveX", 0);
        animator.SetFloat("moveY", 0);
        break;
      case State.Reloading:
        if (animator.GetCurrentAnimatorStateInfo(1).IsName("Idle"))
            animator.SetTrigger("reload");
        else
            animator.ResetTrigger("reload");
        break;
    }
  }

  State CalculateNextState (State state) {
    if (IsAttackIncoming(player))
    {
        return State.Fleeing;
    }
    if (state == State.Idle) {
      if (stamina > 0) {
        if (IsAttackIncoming (player)) {
          return State.Fleeing;
        } else {
          if(IsTargetWithinAttackRange(player))
            return State.Attacking;
          else
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
          return State.Moving;
        } else {
          return State.Idle;
        }
      }
    } else if (state == State.Fleeing) {
      if (stamina > 0) {
        if (IsAttackIncoming (player)) {
          return State.Fleeing;
        } else {
          return State.Idle;
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
        if (activeWeapon.currentAmmo != 0)
        {
            if (stamina > 0)
            {
                return State.Attacking;
            }
            else
            {
                return State.Idle;
            }
        }
        else
        {
            if(stamina > 0)
            {
                return State.Fleeing;
            }
            else
            {
                return State.Reloading;
            }
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

    float fireTimer = 0;

    protected virtual void Fire()
    {
        fireTimer += Time.deltaTime;
        if(activeWeapon.canFire && fireTimer > 2)
        {
          activeWeapon.Fire();
          print("Firing enemy rockets.");
          fireTimer = 0;
        }
        else if(!activeWeapon.canFire && fireTimer < 2)
        {
            if (player.GetComponent<PlayerController>().IsAttacking)
                state = State.Fleeing;

        }
    }

    void ReloadAmmo()
    {
        if(activeWeapon.currentAmmo == 0)
            activeWeapon.SendMessage("ReloadAmmo");
    }

    public bool IsWeaponLoaded (Weapon weapon) {
    return (weapon.currentAmmo > 0 && !weapon.switching);
  }

  bool IsTargetWithinAttackRange (Transform attackTarget) {
    return (attackTarget.position - transform.position).sqrMagnitude <= currentAttackRange * currentAttackRange;
  }

  bool IsAttackIncoming (Transform fromTarget) {

    var attacking = false;
    var rk = FindObjectsOfType<Rocket>();
    if (rk == null)
        return false;
    attacking = (rk.FirstOrDefault(r => r.owner == player)) != null;
    return attacking;
  }

  protected virtual void Fire (Transform target) {
    print ("Enemy fired rockets.");
  }

  public override void TakeDamage (int damage) {
    base.TakeDamage (damage);
  }

  protected override void Die () {
    alive = false;
    GetComponent<Collider>().enabled = false;
    GetComponent<Rigidbody>().useGravity = false;
    GetComponent<Rigidbody>().velocity = Vector3.zero;
    Destroy(gameObject, 1);
    print ("Enemy dead.");
  }
}