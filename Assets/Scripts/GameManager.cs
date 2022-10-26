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

    public List<GameObject> player2Deck = new List<GameObject>();
    public static List<GameObject> player2Hand = new List<GameObject>();

    public static List<GameObject> player1Field = new List<GameObject>();
    public static List<GameObject> player2Field = new List<GameObject>();

    public static bool player1Turn;
    public static int player1Health;
    public static int player2Health;
    public static int maxEnergyPool;
    public static int currentEnergyPool;
    public static int roundNumber;

    public static bool player1Passed;
    public static bool player2Passed;

    bool opponentMove;
    bool stawp;

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
        switch (gameState)
        {
            default:
                Debug.Log("This is a glitch. Fix it.");
                break;
            case GameState.GameStart:
                Shuffle(player1Deck);
                Shuffle(player2Deck);
                Draw4(player1Deck, player1Hand);
                Draw4(player2Deck, player2Hand);
                player1Health = 20;
                player2Health = 20;
                maxEnergyPool = 10;
                currentEnergyPool = maxEnergyPool;
                roundNumber = 1;
                gameState = GameState.Player1Turn;
                Debug.Log("Switching GameState.");
                player1Turn = true;
                break;
            case GameState.Player1Turn:
                P1TurnIndicator.SetActive(true);
                P2TurnIndicator.SetActive(false);
                List<GameObject> elligible = new List<GameObject>();
                if (currentEnergyPool == 0)
                {
                    Debug.Log("The energy pool is empty. Ending Round.");
                    gameState = GameState.EndRound;
                }
                else
                {
                    if (player1Passed)
                    {
                        Debug.Log("Player 1 has passed. Switching to Player 2.");
                        player1Turn = !player1Turn;
                        gameState = GameState.Player2Turn;
                    }
                    else
                    {
                        if (!player1Turn)
                        {
                            player1Turn = !player1Turn;
                            gameState = GameState.Player2Turn;
                        }
                        else
                        {
                            for (int i = 0; i < player1Hand.Count; i++)
                            {
                                if (player1Hand[i].GetComponent<Card>().energyCost <= currentEnergyPool)
                                {
                                    elligible.Add(player1Hand[i]);
                                }
                            }
                            if (elligible.Count == 0)
                            {
                                Debug.Log("No more moves are possible. Player 1 is forced to pass.");
                                player1Passed = true;
                            }
                        }
                    }
                }
                break;
            case GameState.Player2Turn:
                P1TurnIndicator.SetActive(false);
                P2TurnIndicator.SetActive(true);
                List<GameObject> elligibleChoices = new List<GameObject>();

                if (!opponentMove)
                {
                    opponentMove = true;
                    for (int i = 0; i < player2Hand.Count; i++)
                    {
                        if (player2Hand[i].GetComponent<Card>().energyCost <= currentEnergyPool && !player2Hand[i].GetComponent<Card>().deployed)
                        {
                            elligibleChoices.Add(player2Hand[i]);
                        }
                    }
                    if (elligibleChoices.Count > 0)
                    {
                        StartCoroutine(Deploy(elligibleChoices));
                    }
                    else
                    {
                        player2Passed = true;
                    }
                }
                if (currentEnergyPool == 0)
                {
                    Debug.Log("The energy pool is empty. Ending Round.");
                    gameState = GameState.EndRound;
                }
                else
                {
                    if (player2Passed)
                    {
                        if (player1Passed)
                        {
                            Debug.Log("Both players have passed. Ending Round.");
                            gameState = GameState.EndRound;
                        }
                        else
                        {
                            Debug.Log("Player 2 has passed. Switching to Player 1.");
                            gameState = GameState.Player1Turn;
                        }
                    }
                    else
                    {
                        if (player1Turn)
                        {
                            Debug.Log("Player 2 has made a move. Switching to Player 1.");
                            gameState = GameState.Player1Turn;
                        }
                    }
                }
                break;
            case GameState.EndRound:
                if (!stawp)
                {
                    DamageCalculation(player1Field, player2Field);

                    stawp = true;
                }
                break;
            case GameState.GameEnd:
                break;
        }

        GameObject.FindWithTag("Player1Health").GetComponent<Text>().text = "Health: " + player1Health.ToString();
        GameObject.FindWithTag("Player2Health").GetComponent<Text>().text = "Health: " + player2Health.ToString();

        GameObject.FindWithTag("RoundNumber").GetComponent<Text>().text = "Round: " + roundNumber.ToString();
        GameObject.FindWithTag("EnergyPool").GetComponent<Text>().text = "Energy: " + currentEnergyPool.ToString();

        DistributeCardsinHand();
    }

    void DamageCalculation(List <GameObject> P1Field, List<GameObject> P2Field)
    {
        int damage = 0;
        int p1attack = 0;
        int p2attack = 0;
        int p1defense = 0;
        int p2defense = 0;

        Debug.Log("Cards on player's Field: ");
        for (int i = 0; i < P1Field.Count; i++)
        {
            Debug.Log(P1Field[i].GetComponent<Card>().cardName + " with an attack of " +
            P1Field[i].GetComponent<Card>().attack.ToString() + " and a defense of " +
            P1Field[i].GetComponent<Card>().defense.ToString());
            p1attack += P1Field[i].GetComponent<Card>().attack;
            p1defense += P1Field[i].GetComponent<Card>().defense;
        }

        Debug.Log("Cards on enemy's Field: ");
        for (int i = 0; i < P2Field.Count; i++)
        {
            Debug.Log(P2Field[i].GetComponent<Card>().cardName + " with an attack of " +
            P2Field[i].GetComponent<Card>().attack.ToString() + " and a defense of " +
            P2Field[i].GetComponent<Card>().defense.ToString());
            p2attack += P2Field[i].GetComponent<Card>().attack;
            p2defense += P2Field[i].GetComponent<Card>().defense;
        }

        Debug.Log("Player 1's total attack is: " + p1attack.ToString());
        Debug.Log("Player 1's total defense is: " + p1defense.ToString());
        Debug.Log("Player 2's total attack is: " + p2attack.ToString());
        Debug.Log("Player 2's total defense is: " + p2defense.ToString());

        int modifiedP1attack = 0;
        int modifiedP2attack = 0;
        if (p1attack - p2defense <= 0)
        {
            modifiedP1attack = 0;
        }
        else
        {
            modifiedP1attack = p1attack - p2defense;
        }

        if (p2attack - p1defense <= 0)
        {
            modifiedP2attack = 0;
        }
        else
        {
            modifiedP2attack = p2attack - p1defense;
        }

        Debug.Log("Player 1's modified attack is: " + modifiedP1attack.ToString());
        Debug.Log("Player 2's modified attack is: " + modifiedP2attack.ToString());

        if (modifiedP1attack > modifiedP2attack)
        {
            damage = modifiedP1attack - modifiedP2attack;
            player2Health -= damage;
            Debug.Log("The round ends with player 2 taking " + damage.ToString() + " damage.");
        }
        else
        {
            if (modifiedP1attack < modifiedP2attack)
            {
                damage = modifiedP2attack - modifiedP1attack;
                player1Health -= damage;
                Debug.Log("The round ends with player 1 taking " + damage.ToString() + " damage.");
            }
            else
            {
                Debug.Log("Neither player takes damage. The round ends in a draw.");
            }
        }
    }

    IEnumerator Deploy(List<GameObject> elligible)
    {
        yield return new WaitForSeconds(1);
        elligible[Random.Range(0, elligible.Count)].GetComponent<Card>().Deploy();
        player1Turn = true;
        opponentMove = false;
    }

    void Shuffle(List<GameObject> Deck)
    {
        for (int i = 0; i < Deck.Count; i++)
        {
            GameObject temp = Deck[i];
            int randomIndex = Random.Range(i, Deck.Count);
            Deck[i] = Deck[randomIndex];
            Deck[randomIndex] = temp;
        }
    }

    void Draw4(List<GameObject> Deck, List<GameObject> Hand)
    {
        for (int i = 0; i < 4; i++)
        {
            GameObject newCard = Instantiate(Deck[i], new Vector2(-20 + i * 10, -14), Quaternion.identity);
            Hand.Add(newCard);
            Deck.Remove(Deck[i]);
        }
    }

    void DistributeCardsinHand()
    {
        if (player1Hand.Count != 0)
        {
            int spacing = 30 / player1Hand.Count;
            for (int i = 0; i < player1Hand.Count; i++)
            {
                player1Hand[i].GetComponent<Card>().isPlayer1 = true;
                if (player1Hand[i].GetComponent<Card>().deployed)
                {
                    player1Hand[i].transform.position = new Vector2((-spacing / 2) + spacing * (i - 1), -8);
                }
                else
                {
                    player1Hand[i].transform.position = new Vector2((-spacing / 2) + spacing * (i - 1), -18);
                }
            }
        }
        if (player2Hand.Count != 0)
        {
            int spacing = 30 / player2Hand.Count;
            for (int i = 0; i < player2Hand.Count; i++)
            {
                if (player2Hand[i].GetComponent<Card>().deployed)
                {
                    player2Hand[i].transform.position = new Vector2((-spacing / 2) + spacing * (i - 1), 8);
                }
                else
                {
                    player2Hand[i].transform.position = new Vector2((-spacing / 2) + spacing * (i - 1), 18);
                }
            }
        }
    }

    public static void SwitchTurn()
    {
        player1Turn =!player1Turn;
    }
}