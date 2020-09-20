using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;


//Enemy inherits from MovingObject, our base class for objects that can move, Player also inherits from this.
public class Enemy : MovingObject
{
    private Pathfinding pathfinding;
    private List<PathNode> path;
    private GameObject[] targets;
    private Transform target;

    //private Animator animator;							//Variable of type Animator to store a reference to the enemy's Animator component.
    //private bool skipMove;								//Boolean to determine whether or not enemy should skip a turn or move this turn.


    //Start overrides the virtual Start function of the base class.
    protected override void Awake()
    {
        isNpc = true;
        base.Awake();
        ChangeFacingDirection(new Vector2(-1, 0));
    }

    public IEnumerator EnemyMove(int rows, int columns, List<Vector3> currentUnwalkables)
    {
        if (dead || stun) {
            yield break;
        }

        UnitState priorState = state;
        state = UnitState.BUSY;

        targets = GameObject.FindGameObjectsWithTag("Player");

        int temp = int.MaxValue;
        List<PathNode> tempPath = new List<PathNode>();

        foreach (GameObject t in targets)
        {
            /*if (t.GetComponent<MovingObject>().health < tHealth)
            {
                tHealth = t.GetComponent<MovingObject>().health;
                target = t.transform;

                currentUnwalkables.Remove(target.position);

                currentUnwalkables.Remove(transform.position);

                pathfinding = new Pathfinding(columns, rows, currentUnwalkables);

                path = pathfinding.FindPath((int)transform.position.x, (int)transform.position.y, (int)target.position.x, (int)target.position.y);
            }*/

                currentUnwalkables.Remove(t.transform.position);

                currentUnwalkables.Remove(transform.position);

                pathfinding = new Pathfinding(columns, rows, currentUnwalkables);

                path = pathfinding.FindPath((int)transform.position.x, (int)transform.position.y, (int)t.transform.position.x, (int)t.transform.position.y);

            if (path != null && path.Count < temp)
            {
                temp = path.Count;
                tempPath = path;
                target = t.transform;
            }
            else
            {
                path = tempPath;
            }

            currentUnwalkables.Add(t.transform.position);
        }

        if (path == null)
        {
            Debug.Log("No possible routes to targets!");
            yield break;
        }

        if (path.Count == 0)
        {
            ChangeFacingDirection(target.position - transform.position);
            if (ReadyAbility(0))
            {
                yield return new WaitForSeconds(.5f);
                CastAbility();
            }
            yield return null;
        }
        else
        {
            for (int i = 0; energy > 0 && i < path.Count; i++)
            {
                facingDirection = path[i].coord - transform.position;
                ChangeFacingDirection(facingDirection);
                yield return StartCoroutine(Move((int)facingDirection.x, (int)facingDirection.y));

                if (health <= 0 || stun)
                {
                    state = UnitState.BUSY;
                    yield break;
                }

                //yield return new WaitForSeconds(.1f);

                if (Vector3.Distance(transform.position, target.position) <= 1)
                {
                    ChangeFacingDirection(target.position - transform.position);
                    if (ReadyAbility(0))
                    {
                        yield return new WaitForSeconds(.5f);
                        CastAbility();
                    }
                }
            }
        }
        if (state != UnitState.CHARGING)
        {
            state = priorState;
            target = null;
        }
    }
}