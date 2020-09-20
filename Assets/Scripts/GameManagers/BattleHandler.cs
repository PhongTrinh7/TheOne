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
    private int roundCounter;
    public float turnDelay = .7f;
    public MovingObject activeUnit;
    public Queue<MovingObject> currentUnits;

    //Pathfinding.
    public Pathfinding pathfinding;
    public Transform target;
    public List<Vector3> currentUnwalkables;

    //Camera.
    public CameraController cameraController;

    //Lock.
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
        UIManager.Instance.endTurn.onClick.AddListener(ClickAdvanceTurn);

        //Set up pathfinding.
        currentUnwalkables = new List<Vector3>();
        pathfinding = new Pathfinding(board.columns, board.rows, board.unwalkables);

        currentUnits = new Queue<MovingObject>();

        foreach (MovingObject unit in board.units)
        {
            currentUnits.Enqueue(unit);
        }

        roundCounter = currentUnits.Count;
        activeUnit = currentUnits.Peek();
        activeUnit.StartTurn();
        cameraController.CameraLookAt(activeUnit);

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


                if (activeUnit.state == MovingObject.UnitState.READYUP)
                {                
                    //Ability ready.
                    if (Input.GetMouseButtonDown(0))
                    {
                        activeUnit.CastAbility();
                    }
                    //Cancel ability ready.
                    else if (Input.GetMouseButtonDown(1))
                    {
                        activeUnit.CancelAbility();
                    }
                }
                else
                {
                    if (Input.GetKeyDown("space"))
                    {
                        StartCoroutine(AdvanceTurn());
                    }
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

                if (Input.GetKey("left shift"))
                {
                    //Keyboard Input
                    if (Input.GetKeyDown("w"))
                    {
                        activeUnit.ChangeFacingDirection(new Vector2(0, 1));
                    }
                    else if (Input.GetKeyDown("a"))
                    {
                        activeUnit.ChangeFacingDirection(new Vector2(-1, 0));
                    }
                    else if (Input.GetKeyDown("s"))
                    {
                        activeUnit.ChangeFacingDirection(new Vector2(0, -1));
                    }
                    else if (Input.GetKeyDown("d"))
                    {
                        activeUnit.ChangeFacingDirection(new Vector2(1, 0));
                    }
                }
                else {
                    //Keyboard Input
                    if (Input.GetKey("w"))
                    {
                        StartCoroutine(activeUnit.Move(0, 1));
                    }
                    else if (Input.GetKey("a"))
                    {
                        StartCoroutine(activeUnit.Move(-1, 0));
                    }
                    else if (Input.GetKey("s"))
                    {
                        StartCoroutine(activeUnit.Move(0, -1));
                    }
                    else if (Input.GetKey("d"))
                    {
                        StartCoroutine(activeUnit.Move(1, 0));
                    }
                }

                if (Input.GetKeyDown("1"))
                {
                    Ability1();
                }
                else if (Input.GetKeyDown("2"))
                {
                    Ability2();
                }
                else if (Input.GetKeyDown("3"))
                {
                    Ability3();
                }
                else if (Input.GetKeyDown("4"))
                {
                    Ability4();
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

    void ClickAdvanceTurn()
    {
        StartCoroutine(AdvanceTurn());
    }

    IEnumerator AdvanceTurn()
    {
        controlLocked = true;
        activeUnit.EndTurn();

        roundCounter--;
        if (roundCounter <= 0)
        {
            UpdateEnvironmentalHazards();
            roundCounter = currentUnits.Count;
        }

        //Let effects run their course before setting enemy loose.
        yield return new WaitForSeconds(turnDelay);

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

        cameraController.CameraLookAt(activeUnit);
        //cameraController.newPosition.x = activeUnit.transform.position.x;
        //cameraController.newPosition.y = activeUnit.transform.position.y;

        UIManager.Instance.UpdateTurnOrderPortraits(currentUnits);

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
        activeUnit.ReadyAbility(0);
    }

    public void Ability2()
    {
        activeUnit.ReadyAbility(1);
    }

    public void Ability3()
    {
        activeUnit.ReadyAbility(2);
    }

    public void Ability4()
    {
        activeUnit.ReadyAbility(3);
    }

    //Send them to the shadow realms.
    public void RemoveUnit(MovingObject unit)
    {
        UIManager.Instance.UpdateTurnOrderPortraits(currentUnits);
        unit.Death();
    }

    public void UpdateEnvironmentalHazards()
    {
        foreach (EnvironmentalHazard hazard in FindObjectsOfType(typeof(EnvironmentalHazard)) as EnvironmentalHazard[])
        {
            if (hazard.gameObject.layer == 9)
            {
                hazard.DurationCountDown();
            }
        }
    }
}
