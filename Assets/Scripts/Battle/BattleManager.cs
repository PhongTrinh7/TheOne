using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;
using System.Collections.Generic; 		//Allows us to use Lists.
using Random = UnityEngine.Random; 		//Tells Random to use the Unity Engine random number generator.

public class BattleManager : Manager<BattleManager>
{
    //Battle Node info.
    public MapNode node;

    //Board references.
    public Board[] boards;
    public Board board;

    //Handles turns.
    public int currentTurn = 0;
    private int roundCounter;
    public float turnDelay = .7f;
    public MovingObject activeUnit;
    public List<MovingObject> currentUnits;

    //Pathfinding.
    public Pathfinding pathfinding;
    public Transform target;
    public List<Vector3> currentUnwalkables;

    //Camera.
    public CameraController cameraController;

    //Lock.
    public bool controlLocked;

    //End.
    public bool battleEnd;
    private int allyCount;
    private int enemyCount;

    void Start()
    {
        //Ability buttons.
        UIManager.Instance.b1.onClick.AddListener(Ability1);
        UIManager.Instance.b2.onClick.AddListener(Ability2);
        UIManager.Instance.b3.onClick.AddListener(Ability3);
        UIManager.Instance.b4.onClick.AddListener(Ability4);
        UIManager.Instance.b5.onClick.AddListener(Ability5);
        UIManager.Instance.endTurn.onClick.AddListener(ClickAdvanceTurn);
    }

    public void createBattle(MapNode fromNode)
    {
        battleEnd = false;

        node = fromNode;

        //Set up the playing field.
        board = Instantiate(boards[Random.Range(0, boards.Length)]);
        board.SetUpScene();

        //Battle UI.
        UIManager.Instance.BattleUI(true);

        cameraController.BattleCamera();

        //Set up pathfinding.
        currentUnwalkables = new List<Vector3>();
        pathfinding = new Pathfinding(board.columns, board.rows, board.unwalkables);

        currentUnits = new List<MovingObject>();

        allyCount = 0;
        enemyCount = 0;
        foreach (MovingObject unit in board.units)
        {
            currentUnits.Add(unit);

            if (unit.CompareTag("Player"))
            {
                allyCount++;
            }
            else if (unit.CompareTag("Enemy"))
            {
                enemyCount++;
            }
        }

        roundCounter = currentUnits.Count;
        activeUnit = currentUnits[0];
        StartCoroutine(AdvanceTurn(true));

        //activeUnit.StartTurn();

        //cameraController.CameraLookAt(activeUnit);

        //UIManager.Instance.SetUpTurnOrderPortraits(currentUnits);
    }


    void Update()
    {
        if (Input.GetKeyDown("h"))
        {
            UIManager.Instance.InstructionsToggle();
        }

        if (!activeUnit.IsNpc && activeUnit.isTurn && !activeUnit.dead)
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
                //UIManager.Instance.BattleUI();
                UIManager.Instance.AbilityPanel();
            }

            if (activeUnit.State != MovingObject.UnitState.BUSY && !controlLocked)
            {
                if (activeUnit.state == MovingObject.UnitState.READYUP)
                {
                    UIManager.Instance.setEndTurn(false);

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
                    UIManager.Instance.setEndTurn(true);

                    if (Input.GetKeyDown("space"))
                    {
                        StartCoroutine(AdvanceTurn(false));
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
                else if (Input.GetKeyDown("5"))
                {
                    Ability4();
                }

                UIManager.Instance.UpdateActiveUnitAbilities(activeUnit);
            }
        }
        else
        {
            UIManager.Instance.setEndTurn(false);

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
        StartCoroutine(AdvanceTurn(false));
    }

    IEnumerator AdvanceTurn(bool stay)
    {
        controlLocked = true;

        UIManager.Instance.HideTooltips();

        if (!stay)
        {
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

            activeUnit = currentUnits[0];
            for (int i = 1; i < currentUnits.Count; i++)
            {
                currentUnits[i - 1] = currentUnits[i];
            }
            currentUnits.RemoveAt(currentUnits.Count - 1);
            currentUnits.TrimExcess();
            currentUnits.Add(activeUnit);

        }
        else
        {
            yield return new WaitForSeconds(turnDelay);
        } 

        //Next up in line goes.
        activeUnit = currentUnits[0];
        Debug.Log(activeUnit);

        activeUnit.StartTurn();

        cameraController.CameraLookAt(activeUnit);

        UIManager.Instance.UpdateTurnOrderPortraits(currentUnits);

        if (activeUnit.IsNpc && isActiveAndEnabled)
        {
            yield return StartCoroutine(EnemyTurn());

            yield return new WaitForSeconds(turnDelay);
            if (activeUnit.dead)
            {
                yield break;
            }
            StartCoroutine(AdvanceTurn(false));
        }
        else
        {
            UIManager.Instance.UpdateActiveUnitAbilities(activeUnit);
            if (activeUnit.skipTurn)
            {
                activeUnit.EndTurn();
                yield return new WaitForSeconds(turnDelay);
                if (activeUnit.dead)
                {
                    yield break;
                }
                StartCoroutine(AdvanceTurn(false));
            }
        }

        controlLocked = false;
    }

    IEnumerator EnemyTurn()
    {
        Enemy activeEnemy = (Enemy) activeUnit;
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

    public void Ability5()
    {
        activeUnit.ReadyAbility(4);
    }

    //Send them to the shadow realms.
    public void RemoveUnit(MovingObject unit)
    {
        if (unit.isTurn)
        {
            StartCoroutine(AdvanceTurn(true));
        }
        //cameraController.cameraLocked = false;
        currentUnits.Remove(unit);
        UIManager.Instance.UpdateTurnOrderPortraits(currentUnits);

        if (unit.CompareTag("Player"))
        {
            allyCount--;
        }
        else if (unit.CompareTag("Enemy"))
        {
            enemyCount--;
        }
        Debug.Log(allyCount);
        Debug.Log(enemyCount);
        if (enemyCount < 1)
        {
            StartCoroutine(EndBattleCoroutine(true));
        }
        else if (allyCount < 1)
        {
            StartCoroutine(EndBattleCoroutine(false));
        }
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

    public void clearHazards()
    {
        foreach (EnvironmentalHazard hazard in FindObjectsOfType(typeof(EnvironmentalHazard)) as EnvironmentalHazard[])
        {
            Destroy(hazard);
        }
    }

    IEnumerator EndBattleCoroutine(bool win)
    {
        yield return new WaitForSeconds(1);
        GameManager.Instance.EndBattle(node);
    }
}
