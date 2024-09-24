using Mirror;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameLogicManager : NetworkBehaviour
{
    #region Singleton
    private static GameLogicManager _instance;
    public static GameLogicManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType<GameLogicManager>();
                if (_instance == null)
                {
                    GameObject go = new GameObject("GameManager");
                    _instance = go.AddComponent<GameLogicManager>();
                }
            }
            return _instance;
        }
    }
    #endregion

    [SyncVar(hook = nameof(OnGameStateChanged))]
    private GameState currentState;

    private Dictionary<GameState, IGameState> gameStates;

    //이벤트들(State를 바꾸는 트리거)
    public event Action OnAllPlayersEnteredDungeon;


    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            _instance = this;
            DontDestroyOnLoad(gameObject);
        }

        InitializeStates();
    }

    private void InitializeStates()
    {
        gameStates = new Dictionary<GameState, IGameState>
        {
            { GameState.StartDungeon, new StartDungeonState(this) }
        };
    }

    [Server]
    public void ChangeState(GameState newState)
    {
        if(gameStates.ContainsKey(currentState))
        {
            gameStates[currentState].Exit();
        }

        currentState = newState;

        if (gameStates.ContainsKey(currentState))
        {
            gameStates[currentState].Enter();
        }
    }

    private void OnGameStateChanged(GameState oldState, GameState newState)
    {
        if (gameStates.ContainsKey(newState))
        {
            gameStates[newState].Enter();
        }
    }


}
