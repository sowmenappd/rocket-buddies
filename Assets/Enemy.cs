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

    public int highlight = 0;

  void OnDrawGizmos()
    {
        if(path != null)
            for (int i = 0; i < path.Count; i++)
            {
                Gizmos.color = Color.white;
                Gizmos.DrawCube(path[i], Vector3.one * .3f);
                if(i == highlight)
                {
                    Gizmos.color = Color.red;
                    Gizmos.DrawCube(path[i], Vector3.one * 1f);
                }
            }
        //print(GetDotProduct(A.position, B.position, C.position));
    }

    float GetDotProduct(Vector3 a, Vector3 b, Vector3 c)
    {
        var d1 = (b - a).normalized;
        d1.y = 0;
        var d2 = (c - a).normalized;
        d2.y = 0;
        return Vector3.Dot(d1, d2);
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
  List<Vector3> path = new List<Vector3>();
  float maxPathRetraceTime = 1f;
  float pathCurrentTime = 0;
  public int currentPointIndex = 0;

  void ChasePlayer(){
    Vector3 dir = (player.position - transform.position);
    //dir.y = 0;
    if (path != null && path.Count > 0)
    {
        Vector3 pos = path[currentPointIndex];
        dir = pos - transform.position;
        animMoveDirection = new Vector2(dir.normalized.x, dir.normalized.z);
        dir.y = 0;
        FaceDirection(animMoveDirection);
        transform.Translate(dir.normalized * moveSpeed * Time.deltaTime);
        if(currentPointIndex > 0)
            {
            if(dir.sqrMagnitude < 0.5f)
            {
                currentPointIndex++;
            }
        }
        pathCurrentTime += Time.deltaTime;
        if(pathCurrentTime > maxPathRetraceTime)
        {
            path = GetPath(transform.position, player.position);
            pathCurrentTime = 0;
            currentPointIndex = 0;// path.Count - 1;
            }
    } else {
            path = GetPath(transform.position, player.position);
            pathCurrentTime = 0;
            currentPointIndex = 0;// path.Count - 1;
    }

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
        dir.y = 0;
        Vector3 currPos = startPos;

        Node lastWalkableNode = startNode;

        var visited = new HashSet<Node>();

        for (int i=0; i<=resolution; i++)
        {
            if (lastWalkableNode == endNode) break;
            RaycastHit hit;
            var obstacleNode = Physics.Raycast(currPos + Vector3.up * 100f, Vector3.down, out hit, 1000f);
            //process obstacle node

            var node = grid.NodeFromWorldPostion(hit.point);
            if (node.walkable)
            {
                path.Add(node.worldPos + Vector3.up);
                lastWalkableNode = node;
                currPos += dir * step;

            }
            else
            {
                //apply floodfill on last walkable node and get another node that is walkable
                //update direction from new walkable node
                //add node to path
                //set lw node to new node
                Node n = FindNextBestNeighbourNode(grid, lastWalkableNode, endNode);
                dir = (endNode.worldPos - n.worldPos).normalized;
                path.Add(n.worldPos);
                lastWalkableNode = n;
                currPos = n.worldPos + dir * step;

                //i--;
                #region testing

                /*
                //bool pathFound = false;
                print("Blocked on node " + node.worldPos);

                var cnode = lastWalkableNode;
                //int a = 0, MAX_ITERATIONS = 100;

                //while (!pathFound && a++ < MAX_ITERATIONS)
                //{
                var neighbours = grid.GetNeighboursOf(cnode, 10).Where(n => {
                    if (n.walkable && !visited.Contains(n))
                    {
                        if(Vector3.Distance(n.worldPos, endNode.worldPos) 
                        < Vector3.Distance(cnode.worldPos, endNode.worldPos))
                        {
                            visited.Add(n);
                            return true;
                        }
                    }
                    return false;
                }).ToList();
                if (neighbours.Count == 0)
                {
                    print("LIST EMPTY?");
                    continue;
                    //return path;
                }
                Node bestNode = neighbours[0];
                float bestDot = 0;
                Vector3 bestDir = Vector3.zero;
                while (neighbours.Count > 0)
                {
                    cnode = neighbours[0];
                    neighbours.RemoveAt(0);
                    Vector3 newDir = (endNode.worldPos - cnode.worldPos).normalized;
                    float yHeight = newDir.y;
                    newDir.y = 0;
                    Vector3 oldDir = new Vector3(dir.x, 0, dir.z);
                    float dot = Vector3.Dot(newDir, oldDir);
                    if (dot > bestDot){
                        bestDot = dot;
                        bestDir = newDir;
                        bestDir.y = yHeight;
                        bestNode = cnode;
                    }
                }
                dir = bestDir.normalized;
                path.Add(bestNode.worldPos + Vector3.up);
                lastWalkableNode = bestNode;
                //}
                */

                #endregion
            }

        }

        //path.Add(startNode.worldPos);
        //path.Add(endNode.worldPos);
        return path;
    }

    Node FindNextBestNeighbourNode(Grid grid, Node current, Node targetNode)
    {
        var sqrDistLower = (targetNode.worldPos - current.worldPos).sqrMagnitude;

        //List<Node> checkList = new List<Node>();
        HashSet<Node> seen = new HashSet<Node>();

        //checkList.Add(current);
        bool found = false;
        Node curr = current;//checkList[0];
        int r = 0;
        while(!found && r++ < 100){
            //checkList.RemoveAt(0);
            var neighbours = grid.GetNeighboursOf(curr);
            foreach(var n in neighbours){
                if (!n.walkable) continue;
                var sqrDist = (n.worldPos - targetNode.worldPos).sqrMagnitude;
                if (seen.Contains(n) || sqrDist > sqrDistLower) continue;
                else {
                    seen.Add(n);
                    if(sqrDist < sqrDistLower){
                        sqrDistLower = sqrDist;
                        curr = n;
                        found = true;
                    }
                }

            }
        }
        //if (curr != current)
            return curr;
        //else
            //return null;
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