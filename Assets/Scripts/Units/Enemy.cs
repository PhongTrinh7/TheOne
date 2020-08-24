using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//Enemy inherits from MovingObject, our base class for objects that can move, Player also inherits from this.
public class Enemy : MovingObject
{
    private Pathfinding pathfinding;
    private List<PathNode> path;
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
        UnitState priorState = state;
        state = UnitState.BUSY;

        if (health <= 0 || stun)
        {
            state = priorState;
            yield break;
        }

        target = GameObject.FindGameObjectWithTag("Player").transform;

        currentUnwalkables.Remove(transform.position);
        currentUnwalkables.Remove(target.position);

        pathfinding = new Pathfinding(columns, rows, currentUnwalkables);

        path = pathfinding.FindPath((int)transform.position.x, (int)transform.position.y, (int)target.position.x, (int)target.position.y);

        if (path.Count == 0)
        {
            ChangeFacingDirection(target.position - transform.position);
            CastAbility(0);
            yield return null;
        }
        else
        {
            for (int i = 0; energy > 0 && i < path.Count; i++)
            {
                facingDirection = path[i].coord - transform.position;
                ChangeFacingDirection(facingDirection);
                StartCoroutine(Move((int)facingDirection.x, (int)facingDirection.y));

                if (health <= 0 || stun)
                {
                    state = priorState;
                    yield break;
                }

                yield return new WaitForSeconds(.7f);

                if (Vector3.Distance(transform.position, target.position) <= 1)
                {
                    ChangeFacingDirection(target.position - transform.position);
                    CastAbility(0);
                }
            }
        }
        state = priorState;
    }
}