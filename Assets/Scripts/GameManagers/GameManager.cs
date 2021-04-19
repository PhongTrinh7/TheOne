using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using System.IO;
using System;

public class GameManager : Manager<GameManager>
{
    public enum GameState
    {
        MENU,
        OVERWORLD,
        BATTLE

    }

    [SerializeField] private CameraController cameraController;
    [SerializeField] private GameObject overworldManager;
    [SerializeField] private GameObject battleManager;
    [SerializeField] private GameObject idk;
    public GameObject overworldMusic;
    public GameObject battleMusic;
    public MapNode fn;

    private GameState state;

    public void Start()
    {
        CreateOverworld();
    }

    private void Update()
    {
        if (Input.GetKeyDown("r"))
        {
            EndBattle(fn);
        }
    }

    public void MainMenu()
    {
        Debug.Log("Main Menu");
    }

    public void CreateOverworld()
    {
        state = GameState.OVERWORLD;
        overworldManager.SetActive(true);
        OverworldManager.Instance.createOverworldMap();
        overworldMusic.SetActive(true);
    }

    public void OverworldState()
    {
        battleMusic.SetActive(false);
        state = GameState.OVERWORLD;
        overworldManager.SetActive(true);
        OverworldManager.Instance.setShowMap(true);
        overworldMusic.SetActive(true);
    }

    public void Battle(MapNode fromNode)
    {
        overworldMusic.SetActive(false);
        idk.SetActive(false);
        fn = fromNode;
        OverworldManager.Instance.setShowMap(false);
        state = GameState.BATTLE;
        battleManager.SetActive(true);
        BattleManager.Instance.createBattle(fromNode);
        battleMusic.SetActive(true);
    }

    public void EndBattle(MapNode fromNode)
    {
        cameraController.cameraLocked = false;
        cameraController.xCamBound = 20;
        cameraController.yCamBound = 10;
        //BattleManager.Instance.clearHazards();
        battleManager.SetActive(false);
        UIManager.Instance.ResetBattleUI();
        UIManager.Instance.BattleUI(false);
        Destroy(((Board) FindObjectOfType(typeof(Board))).gameObject);
        OverworldState();
        OverworldManager.Instance.advanceLayer(fromNode);
    }

    public void timeManip(float time)
    {
        StartCoroutine(timeManipCoroutine(time));
    }
    public IEnumerator timeManipCoroutine(float time)
    {
        Debug.Log("stop");
        Time.timeScale = 0.001f;

        yield return new WaitForSeconds(time * Time.timeScale);

        Time.timeScale = 1;
        Debug.Log("resume");
    }
}

