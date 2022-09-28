using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum GameState
{
    MainMenu, OpenGameState, PauseMenu, EndCredits
}

//These can be more figured out as we go, and the number can change.
public enum Decision1
{
    Choice1, Choice2, Choice3
}

public enum Decision2
{
    Choice1, Choice2, Choice3
}

public enum Decision3
{
    Choice1, Choice2, Choice3
}

public delegate void StateChange();

public class GameManager : MonoBehaviour
{
    public List<GameObject> player1Deck = new List<GameObject>();
    public static Decision1 decision1;
    public static Decision2 decision2;
    public static Decision3 decision3;

    protected GameManager() { }
    private static GameManager instance = null;
    public event StateChange stateChange;

    public GameState gameState
    {
        get;
        private set;
    }

    public static GameManager Instance
    {
        get
        {
            if (GameManager.instance == null)
            {
                DontDestroyOnLoad(GameManager.instance);
                GameManager.instance = new GameManager();
            }
            return GameManager.instance;
        }
    }

    public void SetGameState(GameState state)
    {
        this.gameState = state;
        stateChange();
    }

    public void OnApplicationQuit()
    {
        GameManager.instance = null;
    }

    void Start()
    {
        gameState = GameState.MainMenu;
    }

    void Update()
    {
        /*
        INSERT A BUNCH OF CODE THAT WILL GIVE CONDITIONS TO SWITCHING TO A DIFFERENT GAME STATE
        */

        //Main Game Loop based on game state
        switch (gameState)
        {
            default:
                Debug.Log("This is a glitch. Fix it.");
                break;
            case GameState.MainMenu:
                break;
            case GameState.OpenGameState:
                break;
            case GameState.PauseMenu:
                break;
            case GameState.EndCredits:
                break;
        }
    }

    public void SetDecision1(Decision1 firstChoice)
    {
        decision1 = firstChoice;
    }

    public void SetDecision2(Decision2 secondChoice)
    {
        decision2 = secondChoice;
    }

    public void SetDecision3(Decision3 thirdChoice)
    {
        decision3 = thirdChoice;
    }
}