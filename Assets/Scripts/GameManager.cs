using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum GameState
{
    GameStart, Player1Turn, Player2Turn, EndRound, GameEnd
}

public delegate void StateChange();

public class GameManager : MonoBehaviour
{
    public List<GameObject> player1Deck = new List<GameObject>();
    public static bool player1Turn;
    public static int player1Health;
    public static int player2Health;
    public static int energyPool;
    public static int roundNumber;

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
        gameState = GameState.GameStart;
    }

    void Update()
    {
        GameObject.FindWithTag("RoundNumber").GetComponent<Text>().text = roundNumber.ToString();
        GameObject.FindWithTag("Player1Health").GetComponent<Text>().text = player1Health.ToString();
        GameObject.FindWithTag("EnergyPool").GetComponent<Text>().text = energyPool.ToString();
        switch (gameState)
        {
            default:
                Debug.Log("This is a glitch. Fix it.");
                break;
            case GameState.GameStart:
                Shuffle();
                Draw4();
                player1Health = 20;
                player2Health = 20;
                energyPool = 5;
                roundNumber = 1;
                gameState = GameState.Player1Turn;
                Debug.Log("Switching GameState.");
                break;
            case GameState.Player1Turn:
                break;
            case GameState.Player2Turn:
                break;
            case GameState.EndRound:
                break;
            case GameState.GameEnd:
                break;
        }
    }

    void Shuffle()
    {
        for (int i = 0; i < player1Deck.Count; i++)
        {
            GameObject temp = player1Deck[i];
            int randomIndex = Random.Range(i, player1Deck.Count);
            player1Deck[i] = player1Deck[randomIndex];
            player1Deck[randomIndex] = temp;
        }
    }

    void Draw4()
    {
        for (int i = 0; i < 4; i++)
        {
            GameObject newCard = Instantiate(player1Deck[i], new Vector2(-20 + i * 10, -14), Quaternion.identity);
        }
    }
}