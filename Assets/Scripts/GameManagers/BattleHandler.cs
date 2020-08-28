using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;
using System.Collections.Generic; 		//Allows us to use Lists.
using Random = UnityEngine.Random; 		//Tells Random to use the Unity Engine random number generator.

public class BattleHandler : Manager<BattleHandler>
{
    //Board references.
    public Board[] boards;
    public Board board;

    //Handles turns.
    public int currentTurn = 0;
    public float turnDelay = .7f;
    public MovingObject activeUnit;
    public Queue<MovingObject> currentUnits;

    //Pathfinding.
    public Pathfinding pathfinding;
    public Transform target;
    public List<Vector3> currentUnwalkables;

    public bool controlLocked;

    void Start()
    {
        //Set up the playing field.
        board = Instantiate(boards[0]);
        board.SetUpScene();

        //Battle UI.
        UIManager.Instance.BattleUI();

        //Ability buttons.
        UIManager.Instance.b1.onClick.AddListener(Ability1);
        UIManager.Instance.b2.onClick.AddListener(Ability2);
        UIManager.Instance.b3.onClick.AddListener(Ability3);
        UIManager.Instance.b4.onClick.AddListener(Ability4);

        //Set up pathfinding.
        currentUnwalkables = new List<Vector3>();
        pathfinding = new Pathfinding(board.columns, board.rows, board.unwalkables);

        currentUnits = new Queue<MovingObject>();

        foreach (MovingObject unit in board.units)
        {
            currentUnits.Enqueue(unit);
        }

        activeUnit = currentUnits.Peek();
        activeUnit.StartTurn();

        UIManager.Instance.SetUpTurnOrderPortraits(currentUnits);
    }


    void Update()
    {
        if (!activeUnit.IsNpc && activeUnit.isTurn)
        {
            //UI Interaction.

            //Ability buttons.
            if (UIManager.Instance.abilitiesOn && activeUnit.State != MovingObject.UnitState.ACTIVE || controlLocked)
            {
                UIManager.Instance.SetSkillsUninteractable();
            }

            //Ability panel toggle.
            if (Input.GetKeyDown("t"))
            {
                UIManager.Instance.AbilityPanel();
            }

            if (activeUnit.State != MovingObject.UnitState.BUSY && !controlLocked)
            {
                //End turn if current player controlled unit dies.
                if (activeUnit.health <= 0)
                {
                    StartCoroutine(AdvanceTurn());
                }

                if (Input.GetKeyDown("space"))
                {
                    StartCoroutine(AdvanceTurn());
                }
            }

            //Battle interactions.
            if (activeUnit.State == MovingObject.UnitState.ACTIVE && !controlLocked)
            {
                //Abilities usable only when unit is active.
                if (!UIManager.Instance.abilitiesOn)
                {
                    UIManager.Instance.SetSkillsInteractable();
                }

                //Keyboard Input
                if (Input.GetKeyDown("w"))
                {
                    StartCoroutine(activeUnit.Move(0, 1));
                }
                else if (Input.GetKeyDown("a"))
                {
                    StartCoroutine(activeUnit.Move(-1, 0));
                }
                else if (Input.GetKeyDown("s"))
                {
                    StartCoroutine(activeUnit.Move(0, -1));
                }
                else if (Input.GetKeyDown("d"))
                {
                    StartCoroutine(activeUnit.Move(1, 0));
                }

                UIManager.Instance.UpdateActiveUnitAbilities(activeUnit);
            }
        }
        else
        {
            if (UIManager.Instance.abilitiesOn)
            {
                UIManager.Instance.SetSkillsUninteractable();
            }
        }
    }

    public void UpdateUnwalkables()
    {
        currentUnwalkables.Clear();
        currentUnwalkables.AddRange(board.unwalkables);

        foreach (MovingObject unit in currentUnits)
        {
            currentUnwalkables.Add(unit.transform.position);
        }
    }

    IEnumerator AdvanceTurn()
    {
        controlLocked = true;
        activeUnit.EndTurn();

        //Updates pathfinding based on current unit positions.
        UpdateUnwalkables();

        //Updates the turn order ui.

        activeUnit = currentUnits.Dequeue(); //Moves the unit to the back of the queue.
        currentUnits.Enqueue(activeUnit);

        //Next up in line goes.
        activeUnit = currentUnits.Peek();

        if (activeUnit.dead)
        {
            RemoveUnit(currentUnits.Dequeue());
            activeUnit = currentUnits.Peek();
        }

        activeUnit.StartTurn();

        UIManager.Instance.UpdateTurnOrderPortraits(currentUnits);

        //Let effects run their course before setting enemy loose.
        yield return new WaitForSeconds(turnDelay);

        if (activeUnit.IsNpc)
        {
            yield return StartCoroutine(EnemyTurn());

            yield return new WaitForSeconds(turnDelay);
            StartCoroutine(AdvanceTurn());
        }
        else
        {
            UIManager.Instance.UpdateActiveUnitAbilities(activeUnit);
        }

        controlLocked = false;
    }

    IEnumerator EnemyTurn()
    {
        Enemy activeEnemy = (Enemy)activeUnit;
        yield return StartCoroutine(activeEnemy.EnemyMove(board.rows, board.columns, currentUnwalkables));
    }

    public void Ability1()
    {
        activeUnit.CastAbility(0);
    }

    public void Ability2()
    {
        activeUnit.CastAbility(1);
    }

    public void Ability3()
    {
        activeUnit.CastAbility(2);
    }

    public void Ability4()
    {
        activeUnit.CastAbility(3);
    }

    //Send them to the shadow realms.
    public void RemoveUnit(MovingObject unit)
    {
        UIManager.Instance.UpdateTurnOrderPortraits(currentUnits);
        unit.Death();
    }
}
