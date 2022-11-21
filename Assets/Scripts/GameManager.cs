using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum GameState
{
    GameStart, Player1Turn, Player2Turn, EndRound, Discard, NextRound, GameEnd
}

public delegate void StateChange();

public class GameManager : MonoBehaviour
{
    public List<GameObject> player1Deck = new List<GameObject>();
    public static List<GameObject> player1Hand = new List<GameObject>();

    public List<GameObject> player2Deck = new List<GameObject>();
    public static List<GameObject> player2Hand = new List<GameObject>();

    public static List<GameObject> player1Field = new List<GameObject>();
    public static List<GameObject> player2Field = new List<GameObject>();

    public static List<GameObject> player1Discard = new List<GameObject>();
    public static List<GameObject> player2Discard = new List<GameObject>();

    public static bool player1Starts;

    public static bool switchToPlayer2;

    public static int player1DeckCount;
    public static int player2DeckCount;

    public static int player1Health;
    public static int player2Health;
    public static int maxEnergyPool;
    public static int currentEnergyPool;
    public static int roundNumber;

    public static bool player1Passed;
    public static bool player2Passed;

    //SFX
    public AudioClip Draw1Card;
    public AudioClip Draw3Cards;
    public AudioClip Draw4Cards;
    public AudioClip SwitchRound;
    public AudioClip Win;

    public static bool GameOver;

    public static bool discard = false;

    //Used to error trap clicking multiple cards.
    public static bool currentlyPlayer1Turn;

    public static bool opponentMove;
    bool stawp;
    bool stopOpponent;

    string lastTookDamage = "";

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
        StartCoroutine(GameObject.FindWithTag("Narration").GetComponent<Narrative>().NewText("Game Begin!"));
        gameState = GameState.GameStart;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Debug.Log("Called");
            Application.Quit();
        }

        if (!GameObject.FindWithTag("Narration").GetComponent<Narrative>().isTyping)
        {

            player1DeckCount = player1Deck.Count;
            player2DeckCount = player2Deck.Count;

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
                    maxEnergyPool = 5;
                    currentEnergyPool = maxEnergyPool;
                    roundNumber = 1;
                    gameState = GameState.Player1Turn;
                    //Debug.Log("Switching GameState.");
                    player1Starts = true;
                    break;
                case GameState.Player1Turn:
                    currentlyPlayer1Turn = true;
                    if (player1Passed && player2Passed)
                    {
                        StartCoroutine(GameObject.FindWithTag("Narration").GetComponent<Narrative>().NewText("Both players have passed. Ending Round."));
                        gameState = GameState.EndRound;
                    }
                    else
                    {
                        Player1Turn();
                    }
                    break;
                case GameState.Player2Turn:
                    currentlyPlayer1Turn = false;
                    if (!stopOpponent)
                    {
                        if (player1Passed && player2Passed)
                        {
                            stopOpponent = true;
                            StartCoroutine(GameObject.FindWithTag("Narration").GetComponent<Narrative>().NewText("Both players have passed. Ending Round."));
                            gameState = GameState.EndRound;
                        }
                        else
                        {
                            if (!opponentMove)
                            {
                                StartCoroutine(AITurn());
                            }
                        }
                    }
                    break;
                case GameState.EndRound:
                    stopOpponent = false;
                    if (!stawp)
                    {
                        StartCoroutine(EndRound());
                    }
                    break;
                case GameState.NextRound:
                    discard = false;
                    DrawingProcedure(lastTookDamage);
                    StartCoroutine(GameObject.FindWithTag("Narration").GetComponent<Narrative>().NewText("Starting new Round."));
                    GetComponent<AudioSource>().clip = SwitchRound;
                    GetComponent<AudioSource>().Play();
                    if (maxEnergyPool < 15)
                    {
                        maxEnergyPool++;
                    }
                    else
                    {
                        maxEnergyPool = 5;
                    }
                    currentEnergyPool = maxEnergyPool;
                    roundNumber++;
                    player1Starts = !player1Starts;
                    if (player1Starts)
                    {
                        gameState = GameState.Player1Turn;
                    }
                    else
                    {
                        opponentMove = false;
                        gameState = GameState.Player2Turn;
                    }
                    break;
                case GameState.GameEnd:
                    if (!GameOver)
                    {
                        GetComponent<AudioSource>().clip = Win;
                        GetComponent<AudioSource>().Play();
                        if (player1Health <= 0)
                        {
                            StartCoroutine(GameObject.FindWithTag("Narration").GetComponent<Narrative>().NewText("Player 2 Wins!"));
                        }
                        if (player2Health <= 0)
                        {
                            StartCoroutine(GameObject.FindWithTag("Narration").GetComponent<Narrative>().NewText("Player 1 Wins!"));
                        }
                        GameOver = true;
                    }
                    break;
            }
        }

        if(player1Health < 0)
        {
            player1Health = 0;
        }
        if(player2Health < 0)
        {
            player2Health = 0;
        }
        GameObject.FindWithTag("Player1Health").GetComponent<Text>().text = "Health: " + player1Health.ToString();
        GameObject.FindWithTag("Player2Health").GetComponent<Text>().text = "Health: " + player2Health.ToString();

        GameObject.FindWithTag("RoundNumber").GetComponent<Text>().text = "Round: " + roundNumber.ToString();
        GameObject.FindWithTag("EnergyPool").GetComponent<Text>().text = "Energy: " + currentEnergyPool.ToString();

        DistributeCardsinHand();
    }

    void Player1Turn()
    {
        P1TurnIndicator.SetActive(true);
        P2TurnIndicator.SetActive(false);
        if (player1Passed)
        {
            StartCoroutine(GameObject.FindWithTag("Narration").GetComponent<Narrative>().NewText("Player 1 has passed. Switching to Player 2."));
            opponentMove = false;
            gameState = GameState.Player2Turn;
        }
        else
        {
            if (switchToPlayer2)
            {
                opponentMove = false;
                gameState = GameState.Player2Turn;
            }
            else
            {
                List<GameObject> elligible = new List<GameObject>();
                for (int i = 0; i < player1Hand.Count; i++)
                {
                    if (player1Hand[i].GetComponent<Card>().energyCost <= currentEnergyPool && !player1Hand[i].GetComponent<Card>().deployed)
                    {
                        elligible.Add(player1Hand[i]);
                    }
                }
                if (elligible.Count == 0)
                {
                    StartCoroutine(GameObject.FindWithTag("Narration").GetComponent<Narrative>().NewText("No more moves are possible, player 1 is forced to pass."));
                    player1Passed = true;
                    opponentMove = false;
                    gameState = GameState.Player2Turn;
                }
            }
        }
    }

    void DrawingProcedure(string playerWhoTookDamage)
    {
        switch (playerWhoTookDamage)
        {
            default:
                Debug.Log("Fix this glitch");
                break;
            case "Player1":
                if(player1Hand.Count == 0)
                {
                    Debug.Log("Being Called");
                    Draw3(player1Deck, player1Hand, player1Discard);
                }
                else
                {
                    Draw1(player1Deck, player1Hand, player1Discard);
                    Draw1(player1Deck, player1Hand, player1Discard);
                }
                if(player2Hand.Count==0)
                {
                    Debug.Log("Being Called");
                    Draw3(player2Deck, player2Hand, player2Discard);
                }
                else
                {
                    Draw1(player2Deck, player2Hand, player2Discard);
                }
                break;
            case "Player2":
                if (player1Hand.Count == 0)
                {
                    Debug.Log("Being Called");
                    Draw3(player1Deck, player1Hand, player1Discard);
                }
                else
                {
                    Draw1(player1Deck, player1Hand, player1Discard);
                }
                if (player2Hand.Count == 0)
                {
                    Debug.Log("Being Called");
                    Draw3(player2Deck, player2Hand, player2Discard);
                }
                else
                {
                    Draw1(player2Deck, player2Hand, player2Discard);
                    Draw1(player2Deck, player2Hand, player2Discard);
                }
                break;
            case "Draw":
                if (player1Hand.Count == 0)
                {
                    Debug.Log("Being Called");
                    Draw3(player1Deck, player1Hand, player1Discard);
                }
                else
                {
                    Draw1(player1Deck, player1Hand, player1Discard);
                }
                if (player2Hand.Count == 0)
                {
                    Debug.Log("Being Called");
                    Draw3(player2Deck, player2Hand, player2Discard);
                }
                else
                {
                    Draw1(player2Deck, player2Hand, player2Discard);
                }
                break;
        }
    }

    IEnumerator DamageCalculation(List<GameObject> P1Field, List<GameObject> P2Field)
    {
        int damage = 0;
        int p1attack = 0;
        int p2attack = 0;
        int p1defense = 0;
        int p2defense = 0;

        Debug.Log("Cards on player's Field: ");
        for (int i = 0; i < P1Field.Count; i++)
        {
            p1attack += P1Field[i].GetComponent<Card>().attack;
            p1defense += P1Field[i].GetComponent<Card>().defense;
        }

        Debug.Log("Cards on enemy's Field: ");
        for (int i = 0; i < P2Field.Count; i++)
        {
            p2attack += P2Field[i].GetComponent<Card>().attack;
            p2defense += P2Field[i].GetComponent<Card>().defense;
        }

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

        do
        {
            yield return null;
        } while (GameObject.FindWithTag("Narration").GetComponent<Narrative>().isTyping);
        yield return new WaitForSeconds(1);

        if (modifiedP1attack > modifiedP2attack)
        {
            damage = modifiedP1attack - modifiedP2attack;
            player2Health -= damage;
            lastTookDamage = "Player2";
            StartCoroutine(GameObject.FindWithTag("Narration").GetComponent<Narrative>().NewText("The round ends with player 2 taking " + damage.ToString() + " damage."));
        }
        else
        {
            if (modifiedP1attack < modifiedP2attack)
            {
                damage = modifiedP2attack - modifiedP1attack;
                player1Health -= damage;
                lastTookDamage = "Player1";
                StartCoroutine(GameObject.FindWithTag("Narration").GetComponent<Narrative>().NewText("The round ends with player 1 taking " + damage.ToString() + " damage."));
            }
            else
            {
                StartCoroutine(GameObject.FindWithTag("Narration").GetComponent<Narrative>().NewText("Neither player takes damage. The round ends in a draw."));
                lastTookDamage = "Draw";
            }
        }
        player1Field.Clear();
        player2Field.Clear();

        GameObject[] objects;
        objects = GameObject.FindGameObjectsWithTag("Card");
        foreach (GameObject go in objects)
        {
            if (go.GetComponent<Card>().deployed)
            {
                go.GetComponent<Card>().Discard();
            }
            yield return new WaitForSeconds(.1f);
        }

        if (player1Health <= 0 || player2Health <= 0)
        {
            stawp = false;
            gameState = GameState.GameEnd;
        }
        else
        {
            stawp = false;
            gameState = GameState.NextRound;
        }
    }

    IEnumerator EndRound()
    {
        stawp = true;
        do
        {
            yield return null;
        } while (GameObject.FindWithTag("Narration").GetComponent<Narrative>().isTyping);

        yield return new WaitForSeconds(1);
        player1Passed = false;
        player2Passed = false;
        StartCoroutine(DamageCalculation(player1Field, player2Field));
    }

    IEnumerator AITurn()
    {
        opponentMove = true;
        switchToPlayer2 = false;
        P1TurnIndicator.SetActive(false);
        P2TurnIndicator.SetActive(true);
        List<GameObject> elligibleChoices = new List<GameObject>();

        yield return new WaitForSeconds(1);
        for (int i = 0; i < player2Hand.Count; i++)
        {
            if (player2Hand[i].GetComponent<Card>().energyCost <= currentEnergyPool && !player2Hand[i].GetComponent<Card>().deployed)
            {
                elligibleChoices.Add(player2Hand[i]);
            }
        }
        if (elligibleChoices.Count > 0)
        {
            //StartCoroutine(Deploy(elligibleChoices));
            elligibleChoices[Random.Range(0, elligibleChoices.Count)].GetComponent<Card>().Deploy();
            //opponentMove = false;
        }
        else
        {
            player2Passed = true;
        }

        do
        {
            yield return null;
        } while (GameObject.FindWithTag("Narration").GetComponent<Narrative>().isTyping);

        yield return new WaitForSeconds(1);

        if (currentEnergyPool == 0)
        {
            StartCoroutine(GameObject.FindWithTag("Narration").GetComponent<Narrative>().NewText("The Energy Pool is empty. Ending round."));
            gameState = GameState.EndRound;
        }
        else
        {
            if (player2Passed)
            {
                if (player1Passed)
                {
                    StartCoroutine(GameObject.FindWithTag("Narration").GetComponent<Narrative>().NewText("Both players have passed. Ending round."));
                    gameState = GameState.EndRound;
                }
                else
                {
                    StartCoroutine(GameObject.FindWithTag("Narration").GetComponent<Narrative>().NewText("Player 2 has passed. Switching to player 1."));
                    gameState = GameState.Player1Turn;
                }
            }
            else
            {
                gameState = GameState.Player1Turn;
            }
        }
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
        GetComponent<AudioSource>().clip = Draw4Cards;
        GetComponent<AudioSource>().Play();
        for (int i = 0; i < 4; i++)
        {
            GameObject newCard = Instantiate(Deck[i], new Vector2(-20 + i * 10, -14), Quaternion.identity);
            Hand.Add(newCard);
            Deck.Remove(Deck[i]);
        }
    }

    void Draw1(List<GameObject> Deck, List<GameObject> Hand, List<GameObject> Discard)
    {
        GetComponent<AudioSource>().clip = Draw1Card;
        GetComponent<AudioSource>().Play();
        if (Deck.Count == 0)
        {
            for (int i = 0; i < Discard.Count; i++)
            {
                Discard[i].GetComponent<Card>().deployed = false;
                Discard[i].GetComponent<Card>().canPlay = false;
                Discard[i].GetComponent<Card>().privateKnowledge = false;
                Discard[i].GetComponent<Card>().discarded = false;
                Deck.Add(Discard[i]);
            }
            Discard.Clear();
            Debug.Log("No more cards in deck, shuffling discard pile into deck.");
            Shuffle(Deck);
        }
        GameObject newCard = Instantiate(Deck[0], new Vector2(-20, -14), Quaternion.identity);
        Hand.Add(newCard);
        Deck.Remove(Deck[0]);
    }

    void Draw3(List<GameObject> Deck, List<GameObject> Hand, List<GameObject> Discard)
    {
        GetComponent<AudioSource>().clip = Draw3Cards;
        GetComponent<AudioSource>().Play();
        Debug.Log("No more cards in hand, drawing 3.");
        for (int j = 0; j < 3; j++)
        {
            if (Deck.Count == 0)
            {
                for (int i = 0; i < Discard.Count; i++)
                {
                    Discard[i].GetComponent<Card>().deployed = false;
                    Discard[i].GetComponent<Card>().canPlay = false;
                    Discard[i].GetComponent<Card>().privateKnowledge = false;
                    Discard[i].GetComponent<Card>().discarded = false;
                    Deck.Add(Discard[i]);
                }
                Discard.Clear();
                Debug.Log("No more cards in deck, shuffling discard pile into deck.");
                Shuffle(Deck);
            }
            GameObject newCard = Instantiate(Deck[j], new Vector2(-20, -14), Quaternion.identity);
            Hand.Add(newCard);
            Deck.Remove(Deck[j]);
        }
    }

    void DistributeCardsinHand()
    {
        if (player1Hand.Count != 0)
        {
            int first = (-8 * player1Hand.Count) / 2;
            for (int i = 0; i < player1Hand.Count; i++)
            {
                player1Hand[i].GetComponent<Card>().isPlayer1 = true;
                if (player1Hand[i].GetComponent<Card>().deployed)
                {
                    player1Hand[i].transform.position = new Vector2(first + i * 10, -14);
                }
                else
                {
                    player1Hand[i].transform.position = new Vector2(first + i * 10, -28);
                }
            }
        }
        if (player2Hand.Count != 0)
        {
            int second = (-8 * player2Hand.Count) / 2;
            for (int i = 0; i < player2Hand.Count; i++)
            {
                if (player2Hand[i].GetComponent<Card>().deployed)
                {
                    player2Hand[i].transform.position = new Vector2(second + i * 10, 14);
                }
                else
                {
                    player2Hand[i].transform.position = new Vector2(second + i * 10, 28);
                }
            }
        }
    }
}