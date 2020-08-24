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
        OVERWORLD,
        BATTLE

    }

    [SerializeField] private BattleHandler battleHandler;

    override public void Awake()
    {
        Battle();
    }

    void Battle()
    {
        Instantiate(battleHandler);
    }
}

