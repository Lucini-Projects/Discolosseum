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

    public List<GameObject> player1Hand = new List<GameObject>();

    public static bool player1Turn;
    public static int player1Health;
    public static int player2Health;
    public static int maxEnergyPool;
    public static int currentEnergyPool;
    public static int roundNumber;

    public static bool player1Passed;
    public static bool player2Passed;

    public GameObject P1TurnIndicator;
    public GameObject P2TurnIndicator;

    protected GameManager() { }
    public static GameManager instance = null;
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
        //Debug.Log("There are " + player1Deck.Count.ToString() + " cards in player 1's deck.");
        GameObject.FindWithTag("Player1Health").GetComponent<Text>().text = "Health: " + player1Health.ToString();
        GameObject.FindWithTag("Player2Health").GetComponent<Text>().text = "Health: " + player2Health.ToString();

        GameObject.FindWithTag("RoundNumber").GetComponent<Text>().text = "Round: " + roundNumber.ToString();
        GameObject.FindWithTag("EnergyPool").GetComponent<Text>().text = "Energy: " + currentEnergyPool.ToString();

        DistributeCardsinHand();

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
                maxEnergyPool = 5;
                currentEnergyPool = maxEnergyPool;
                roundNumber = 1;
                gameState = GameState.Player1Turn;
                Debug.Log("Switching GameState.");
                break;
            case GameState.Player1Turn:
                P1TurnIndicator.SetActive(true);
                P2TurnIndicator.SetActive(false);
                if (player1Passed)
                {
                    Debug.Log("Player 1 has passed. Switching to Player 2.");
                    gameState = GameState.Player2Turn;
                }
                break;
            case GameState.Player2Turn:
                P1TurnIndicator.SetActive(false);
                P2TurnIndicator.SetActive(true);
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
            player1Hand.Add(newCard);
            Debug.Log("Added " + player1Deck[i].GetComponent<Card>().cardName + " to hand.");
            player1Deck.Remove(player1Deck[i]);
        }
    }

    void DistributeCardsinHand()
    {
        if (player1Hand.Count != 0)
        {
            int spacing = 30 / player1Hand.Count;
            for (int i = 0; i < player1Hand.Count; i++)
            {
                player1Hand[i].transform.position = new Vector2((-spacing / 2) + spacing * (i - 1), -18);
            }
        }
    }
}