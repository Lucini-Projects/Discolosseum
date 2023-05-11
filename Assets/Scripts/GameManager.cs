using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public enum GameState
{
    MainMenu, GameStart, Player1Turn, Player2Turn, EndRound, Discard, NextRound, GameEnd
}

public delegate void StateChange();

public class GameManager : MonoBehaviour
{
    public List<GameObject> cardPool = new List<GameObject>();
    public List<GameObject> cardPoolOnline = new List<GameObject>();


    public static List<GameObject> player1Deck = new List<GameObject>();
    public static List<GameObject> player1Hand = new List<GameObject>();

    public static List<GameObject> player2Deck = new List<GameObject>();
    public static List<GameObject> player2Hand = new List<GameObject>();

    public static List<GameObject> player1Field = new List<GameObject>();
    public static List<GameObject> player2Field = new List<GameObject>();

    public static List<GameObject> player1Discard = new List<GameObject>();
    public static List<GameObject> player2Discard = new List<GameObject>();

    public static List<GameObject> player1Revived = new List<GameObject>();
    public static List<GameObject> player2Revived = new List<GameObject>();

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
    public AudioClip AddStat;
    public AudioClip TakeDamage;
    public AudioClip SwitchTurn;

    public static bool GameOver;

    public static bool discard = false;

    //used by card script to see if another thing is already selected during the turn. 
    public static bool pickMe = false;

    //Used to error trap clicking multiple cards.
    public static bool currentlyPlayer1Turn;

    public static bool opponentMove;
    bool stawp;
    bool stopOpponent;

    //Switches based on play mode; pvp or p vs ai
    public static bool PVP = false;

    GameObject UIOn;

    //Used to trap the startup.
    bool beginGame = false;

    string lastTookDamage = "";

    public GameObject P1TurnIndicator;
    public GameObject P2TurnIndicator;

    public float damageCalcSpeed = .3f;

    GameObject StatIcons;

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
        Scene scene = SceneManager.GetActiveScene();

        switch (scene.name)
        {
            default:
                Debug.Log("Fix this glitch.");
                break;
            case "Title":
                gameState = GameState.GameStart;
                break;
            case "MainGameLoop":
                player1Deck = new List<GameObject>();
                player1Hand = new List<GameObject>();

                player2Deck = new List<GameObject>();
                player2Hand = new List<GameObject>();

                player1Field = new List<GameObject>();
                player2Field = new List<GameObject>();

                player1Discard = new List<GameObject>();
                player2Discard = new List<GameObject>();

                player1Revived = new List<GameObject>();
                player2Revived = new List<GameObject>();
                gameState = GameState.GameStart;
                // Check if the name of the current Active Scene is your first Scene.
                StatIcons = GameObject.FindWithTag("StatIcons");
                StatIcons.SetActive(false);

                P2TurnIndicator.SetActive(false);
                UIOn = GameObject.FindWithTag("DebugElements");

                Text[] textfields = UIOn.GetComponentsInChildren<Text>();
                foreach (Text text in textfields)
                {
                    //text.enabled = false;
                }

                StartCoroutine(GameObject.FindWithTag("Narration").GetComponent<Narrative>().NewText("Game Begin!"));
                gameState = GameState.GameStart;

                if (!PVP)
                {
                    foreach (GameObject go in cardPool)
                    {
                        player1Deck.Add(go);
                        player2Deck.Add(go);
                    }
                }
                else
                {
                    foreach (GameObject go in cardPoolOnline)
                    {
                        player1Deck.Add(go);
                        player2Deck.Add(go);
                    }
                }
                break;
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            //Debug.Log("Called");
            Application.Quit();
        }

        Scene scene = SceneManager.GetActiveScene();
        switch (scene.name)
        {
            default:
                Debug.Log("Fix this glitch.");
                break;
            case "Title":
                //gameState = GameState.MainMenu;
                break;
            case "MainGameLoop":
                if (Input.GetKeyDown(KeyCode.Keypad7))
                {
                    /*
                    Text[] textfields = UIOn.GetComponentsInChildren<Text>();
                    onOff = !onOff;
                    foreach (Text text in textfields)
                    {
                        text.enabled = onOff;
                    }
                    */
                }

                if (GameObject.FindGameObjectsWithTag("Narration").Length > 0 && !GameObject.FindWithTag("Narration").GetComponent<Narrative>().isTyping)
                {
                    player1DeckCount = player1Deck.Count;
                    player2DeckCount = player2Deck.Count;

                    switch (gameState)
                    {
                        default:
                            Debug.Log("This is a glitch. Fix it.");
                            break;
                        case GameState.GameStart:
                            if (!beginGame)
                            {
                                beginGame = true;
                                StartCoroutine(BeginGame());
                            }

                            break;
                        case GameState.Player1Turn:
                            currentlyPlayer1Turn = true;
                            if (player1Passed)
                            {
                                GameObject.FindWithTag("Pass").GetComponent<Button>().interactable = false;
                                if (player2Passed)
                                {
                                    GameObject.FindWithTag("Pass").GetComponent<Button>().interactable = false;
                                    StartCoroutine(GameObject.FindWithTag("Narration").GetComponent<Narrative>().NewText("Both players have passed. Ending Round."));
                                    gameState = GameState.EndRound;
                                }
                                else
                                {
                                    GameObject.FindWithTag("Pass").GetComponent<Button>().interactable = false;
                                    StartCoroutine(GameObject.FindWithTag("Narration").GetComponent<Narrative>().NewText("Player 1 has passed. Switching to player 2."));
                                    gameState = GameState.Player2Turn;
                                }
                            }
                            else
                            {
                                GameObject.FindWithTag("Pass").GetComponent<Button>().interactable = true;

                                if (currentEnergyPool <= 0)
                                {
                                    GameObject.FindWithTag("Pass").GetComponent<Button>().interactable = false;
                                    StartCoroutine(GameObject.FindWithTag("Narration").GetComponent<Narrative>().NewText("The Energy Pool is Empty. Ending Round."));
                                    gameState = GameState.EndRound;
                                }
                                else
                                {
                                    Player1Turn();
                                }
                            }

                            break;
                        case GameState.Player2Turn:
                            GameObject.FindWithTag("Pass").GetComponent<Button>().interactable = false;
                            currentlyPlayer1Turn = false;
                            if (!PVP)
                            {
                                //Debug.Log("GameState has switched to opponent. Step 1.");
                                if (!stopOpponent)
                                {
                                    //Debug.Log("GameState has switched to opponent. Step 2.");
                                    if (player1Passed && player2Passed)
                                    {
                                        //Debug.Log("GameState has switched to opponent. Step 3.");
                                        stopOpponent = true;
                                        //StartCoroutine(GameObject.FindWithTag("Narration").GetComponent<Narrative>().NewText("Both players have passed. Ending Round."));
                                        gameState = GameState.EndRound;
                                    }
                                    else
                                    {
                                        //Debug.Log("GameState has switched to opponent. Step 4.");
                                        if (!opponentMove)
                                        {
                                            //Debug.Log("GameState has switched to opponent. Step 5.");
                                            StartCoroutine(AITurn());
                                        }
                                    }
                                }
                            }
                            else
                            {
                                currentlyPlayer1Turn = false;
                                if (!player2Passed)
                                {
                                    GameObject.FindWithTag("Pass").GetComponent<Button>().interactable = true;
                                }
                                else
                                {
                                    GameObject.FindWithTag("Pass").GetComponent<Button>().interactable = false;
                                }

                                if (player1Passed && player2Passed)
                                {
                                    GameObject.FindWithTag("Pass").GetComponent<Button>().interactable = false;
                                    StartCoroutine(GameObject.FindWithTag("Narration").GetComponent<Narrative>().NewText("Both players have passed. Ending Round."));
                                    gameState = GameState.EndRound;
                                }
                                else
                                {
                                    if (!player2Passed)
                                    {
                                        Player2Turn();
                                    }
                                    else
                                    {
                                        if (currentEnergyPool > 0)
                                        {
                                            gameState = GameState.Player1Turn;
                                        }
                                        else
                                        {
                                            GameObject.FindWithTag("Pass").GetComponent<Button>().interactable = false;
                                            StartCoroutine(GameObject.FindWithTag("Narration").GetComponent<Narrative>().NewText("The Energy Pool is Empty. Ending Round."));
                                            gameState = GameState.EndRound;
                                        }
                                    }
                                }
                            }
                            break;
                        case GameState.EndRound:
                            GameObject.FindWithTag("Pass").GetComponent<Button>().interactable = false;
                            stopOpponent = false;
                            if (!stawp)
                            {
                                StartCoroutine(EndRound());
                            }
                            break;
                        case GameState.NextRound:
                            GameObject.FindWithTag("Pass").GetComponent<Button>().interactable = false;
                            GameObject.FindWithTag("DamageAnimation").GetComponent<Animator>().SetBool("isDamaged", false);
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
                            GameObject.FindWithTag("Pass").GetComponent<Button>().interactable = false;
                            if (!GameOver)
                            {
                                GetComponent<AudioSource>().clip = Win;
                                GetComponent<AudioSource>().Play();
                                if (player1Health <= 0)
                                {
                                    StartCoroutine(GameObject.FindWithTag("Narration").GetComponent<Narrative>().NewText("Player 2 Wins!"));
                                    StartCoroutine(BackToTitle());
                                }
                                if (player2Health <= 0)
                                {
                                    StartCoroutine(GameObject.FindWithTag("Narration").GetComponent<Narrative>().NewText("Player 1 Wins!"));
                                    StartCoroutine(BackToTitle());
                                }
                                GameOver = true;
                            }
                            if (Input.GetKeyDown(KeyCode.Escape))
                            {
                                //Debug.Log("Called");
                                Application.Quit();
                            }
                            break;
                    }
                }

                if (player1Health < 0)
                {
                    player1Health = 0;
                }
                if (player2Health < 0)
                {
                    player2Health = 0;
                }
                GameObject.FindWithTag("Player1Health").GetComponent<TMP_Text>().text = player1Health.ToString();
                GameObject.FindWithTag("Player2Health").GetComponent<TMP_Text>().text = player2Health.ToString();

                GameObject.FindWithTag("Player1Discard").GetComponent<TMP_Text>().text = player1Discard.Count.ToString();
                GameObject.FindWithTag("Player2Discard").GetComponent<TMP_Text>().text = player2Discard.Count.ToString();

                GameObject.FindWithTag("RoundNumber").GetComponent<TMP_Text>().text = "Round: " + roundNumber.ToString();
                GameObject.FindWithTag("EnergyPool").GetComponent<TMP_Text>().text = currentEnergyPool.ToString();

                //DistributeCardsinHand();
                break;
        }
    }

    void Player1Turn()
    {
        P1TurnIndicator.SetActive(true);
        P2TurnIndicator.SetActive(false);

        //Error Trapping cards if neither player has useable cards.
        List<GameObject> elligibleToPlay = new List<GameObject>();
        for (int i = 0; i < player1Hand.Count; i++)
        {
            if (player1Hand[i].GetComponent<Card>().energyCost <= currentEnergyPool && !player1Hand[i].GetComponent<Card>().deployed)
            {
                elligibleToPlay.Add(player1Hand[i]);
            }
        }

        for (int i = 0; i < player2Hand.Count; i++)
        {
            if (player2Hand[i].GetComponent<Card>().energyCost <= currentEnergyPool && !player2Hand[i].GetComponent<Card>().deployed)
            {
                elligibleToPlay.Add(player2Hand[i]);
            }
        }

        if (elligibleToPlay.Count == 0)
        {
            GameObject.FindWithTag("Pass").GetComponent<Button>().interactable = false;
            StartCoroutine(GameObject.FindWithTag("Narration").GetComponent<Narrative>().NewText("No more moves are possible. Ending Round."));
            gameState = GameState.EndRound;
        }
        else
        {
            if (player1Passed)
            {
                if (player2Passed)
                {
                    GameObject.FindWithTag("Pass").GetComponent<Button>().interactable = false;
                    StartCoroutine(GameObject.FindWithTag("Narration").GetComponent<Narrative>().NewText("Both players have passed. Ending Round."));
                    GetComponent<AudioSource>().clip = SwitchRound;
                    GetComponent<AudioSource>().Play();
                    gameState = GameState.EndRound;
                }
                else
                {
                    GameObject.FindWithTag("Pass").GetComponent<Button>().interactable = false;
                    StartCoroutine(GameObject.FindWithTag("Narration").GetComponent<Narrative>().NewText("Player 1 has passed. Switching to Player 2."));
                    GetComponent<AudioSource>().clip = SwitchTurn;
                    GetComponent<AudioSource>().Play();
                    gameState = GameState.Player2Turn;
                }
                //opponentMove = false;
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
                    if (elligible.Count < 1)
                    {
                        Debug.Log("Here");
                        GameObject.FindWithTag("Pass").GetComponent<Button>().interactable = false;
                        StartCoroutine(GameObject.FindWithTag("Narration").GetComponent<Narrative>().NewText("No more moves are possible, player 1 is forced to pass."));
                        player1Passed = true;
                        opponentMove = false;
                        GetComponent<AudioSource>().clip = SwitchTurn;
                        GetComponent<AudioSource>().Play();
                        if (currentEnergyPool > 0)
                        {
                            gameState = GameState.Player2Turn;
                        }
                        else
                        {
                            gameState = GameState.EndRound;
                        }
                    }
                }
            }
        }
    }

    //Below is for human player only
    void Player2Turn()
    {
        P1TurnIndicator.SetActive(false);
        P2TurnIndicator.SetActive(true);

        //Error Trapping cards if neither player has useable cards.
        List<GameObject> elligibleToPlay = new List<GameObject>();
        for (int i = 0; i < player1Hand.Count; i++)
        {
            if (player1Hand[i].GetComponent<Card>().energyCost <= currentEnergyPool && !player1Hand[i].GetComponent<Card>().deployed)
            {
                elligibleToPlay.Add(player1Hand[i]);
            }
        }

        for (int i = 0; i < player2Hand.Count; i++)
        {
            if (player2Hand[i].GetComponent<Card>().energyCost <= currentEnergyPool && !player2Hand[i].GetComponent<Card>().deployed)
            {
                elligibleToPlay.Add(player2Hand[i]);
            }
        }

        if (elligibleToPlay.Count < 1)
        {
            GameObject.FindWithTag("Pass").GetComponent<Button>().interactable = false;
            StartCoroutine(GameObject.FindWithTag("Narration").GetComponent<Narrative>().NewText("No more moves are possible. Ending Round."));
            gameState = GameState.EndRound;
        }
        else
        {
            if (player2Passed)
            {
                if (player2Passed)
                {
                    GameObject.FindWithTag("Pass").GetComponent<Button>().interactable = false;
                    StartCoroutine(GameObject.FindWithTag("Narration").GetComponent<Narrative>().NewText("Both players have passed. Ending Round."));
                    GetComponent<AudioSource>().clip = SwitchRound;
                    GetComponent<AudioSource>().Play();
                    gameState = GameState.EndRound;
                }
                else
                {
                    GameObject.FindWithTag("Pass").GetComponent<Button>().interactable = false;
                    StartCoroutine(GameObject.FindWithTag("Narration").GetComponent<Narrative>().NewText("Player 2 has passed. Switching to Player 1."));
                    GetComponent<AudioSource>().clip = SwitchTurn;
                    GetComponent<AudioSource>().Play();
                    gameState = GameState.Player1Turn;
                }
                //opponentMove = false;
            }
            else
            {
                if (!switchToPlayer2)
                {
                    opponentMove = true;
                    gameState = GameState.Player1Turn;
                }
                else
                {

                    List<GameObject> elligible = new List<GameObject>();
                    for (int i = 0; i < player1Hand.Count; i++)
                    {
                        if (player2Hand[i].GetComponent<Card>().energyCost <= currentEnergyPool && !player2Hand[i].GetComponent<Card>().deployed)
                        {
                            elligible.Add(player2Hand[i]);
                        }
                    }
                    if (elligible.Count == 0)
                    {
                        StartCoroutine(GameObject.FindWithTag("Narration").GetComponent<Narrative>().NewText("No more moves are possible, player 2 is forced to pass."));
                        player2Passed = true;
                        //opponentMove = false;
                        GetComponent<AudioSource>().clip = SwitchTurn;
                        GetComponent<AudioSource>().Play();
                        if (currentEnergyPool > 0)
                        {
                            gameState = GameState.Player1Turn;
                        }
                        else
                        {
                            gameState = GameState.EndRound;
                        }
                    }
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
                if (player1Hand.Count == 0)
                {
                    StartCoroutine(Draw1(player1Deck, player1Hand, player1Discard, true));
                    StartCoroutine(Draw1(player1Deck, player1Hand, player1Discard, true));
                    StartCoroutine(Draw1(player1Deck, player1Hand, player1Discard, true));
                }
                else
                {
                    StartCoroutine(Draw1(player1Deck, player1Hand, player1Discard, true));
                    StartCoroutine(Draw1(player1Deck, player1Hand, player1Discard, true));
                }
                if (player2Hand.Count == 0)
                {
                    StartCoroutine(Draw1(player2Deck, player2Hand, player2Discard, false));
                    StartCoroutine(Draw1(player2Deck, player2Hand, player2Discard, false));
                    StartCoroutine(Draw1(player2Deck, player2Hand, player2Discard, false));
                }
                else
                {
                    StartCoroutine(Draw1(player2Deck, player2Hand, player2Discard, false));
                }
                break;
            case "Player2":
                if (player1Hand.Count == 0)
                {
                    StartCoroutine(Draw1(player1Deck, player1Hand, player1Discard, true));
                    StartCoroutine(Draw1(player1Deck, player1Hand, player1Discard, true));
                    StartCoroutine(Draw1(player1Deck, player1Hand, player1Discard, true));
                }
                else
                {
                    StartCoroutine(Draw1(player1Deck, player1Hand, player1Discard, true));
                }
                if (player2Hand.Count == 0)
                {
                    StartCoroutine(Draw1(player2Deck, player2Hand, player2Discard, false));
                    StartCoroutine(Draw1(player2Deck, player2Hand, player2Discard, false));
                    StartCoroutine(Draw1(player2Deck, player2Hand, player2Discard, false));
                }
                else
                {
                    StartCoroutine(Draw1(player2Deck, player2Hand, player2Discard, false));
                    StartCoroutine(Draw1(player2Deck, player2Hand, player2Discard, false));
                }
                break;
            case "Draw":
                if (player1Hand.Count == 0)
                {
                    StartCoroutine(Draw1(player1Deck, player1Hand, player1Discard, true));
                    StartCoroutine(Draw1(player1Deck, player1Hand, player1Discard, true));
                    StartCoroutine(Draw1(player1Deck, player1Hand, player1Discard, true));
                }
                else
                {
                    StartCoroutine(Draw1(player1Deck, player1Hand, player1Discard, true));
                }
                if (player2Hand.Count == 0)
                {
                    StartCoroutine(Draw1(player2Deck, player2Hand, player2Discard, false));
                    StartCoroutine(Draw1(player2Deck, player2Hand, player2Discard, false));
                    StartCoroutine(Draw1(player2Deck, player2Hand, player2Discard, false));
                }
                else
                {
                    StartCoroutine(Draw1(player2Deck, player2Hand, player2Discard, false));
                }
                break;
        }
    }

    IEnumerator BackToTitle()
    {
        yield return new WaitForSeconds(2.0f);
        StartCoroutine(GameObject.FindWithTag("Narration").GetComponent<Narrative>().NewText("Thank you for playing this game! Now exiting."));
        yield return new WaitForSeconds(5.0f);
        Application.Quit();
    }

    IEnumerator DamageCalculation(List<GameObject> P1Field, List<GameObject> P2Field)
    {
        StatIcons.SetActive(true);

        GameObject.FindWithTag("P1IteratingAttack").GetComponent<Text>().text = 0.ToString();
        GameObject.FindWithTag("P2IteratingAttack").GetComponent<Text>().text = 0.ToString();
        GameObject.FindWithTag("P1IteratingDefense").GetComponent<Text>().text = 0.ToString();
        GameObject.FindWithTag("P2IteratingDefense").GetComponent<Text>().text = 0.ToString();

        int damage = 0;
        int p1attack = 0;
        int p2attack = 0;
        int p1defense = 0;
        int p2defense = 0;

        for (int i = 0; i < P1Field.Count; i++)
        {
            P1Field[i].transform.Find("Sword Glow").gameObject.SetActive(true);
            p1attack += P1Field[i].GetComponent<Card>().attack;
            GetComponent<AudioSource>().clip = AddStat;
            GetComponent<AudioSource>().Play();
            GameObject.FindWithTag("P1IteratingAttack").GetComponent<Text>().text = p1attack.ToString();
            yield return new WaitForSeconds(damageCalcSpeed);
        }

        for (int i = 0; i < P2Field.Count; i++)
        {
            P2Field[i].transform.Find("Sword Glow").gameObject.SetActive(true);
            P2Field[i].transform.Find("Sword Glow").gameObject.transform.localPosition = new Vector3(0, -8.5f, 0);
            p2attack += P2Field[i].GetComponent<Card>().attack;
            GetComponent<AudioSource>().clip = AddStat;
            GetComponent<AudioSource>().Play();
            GameObject.FindWithTag("P2IteratingAttack").GetComponent<Text>().text = p2attack.ToString();
            yield return new WaitForSeconds(damageCalcSpeed);
        }

        for (int i = 0; i < P1Field.Count; i++)
        {
            P1Field[i].transform.Find("Shield Glow").gameObject.SetActive(true);
            p1defense += P1Field[i].GetComponent<Card>().defense;
            GetComponent<AudioSource>().clip = AddStat;
            GetComponent<AudioSource>().Play();
            GameObject.FindWithTag("P1IteratingDefense").GetComponent<Text>().text = p1defense.ToString();
            yield return new WaitForSeconds(damageCalcSpeed);
        }

        for (int i = 0; i < P2Field.Count; i++)
        {
            P2Field[i].transform.Find("Shield Glow").gameObject.SetActive(true);
            P2Field[i].transform.Find("Shield Glow").gameObject.transform.localPosition = new Vector3(0, -8.5f, 0);
            p2defense += P2Field[i].GetComponent<Card>().defense;
            GetComponent<AudioSource>().clip = AddStat;
            GetComponent<AudioSource>().Play();
            GameObject.FindWithTag("P2IteratingDefense").GetComponent<Text>().text = p2defense.ToString();
            yield return new WaitForSeconds(damageCalcSpeed);
        }

        int modifiedP1attack = 0;
        int modifiedP2attack = 0;
        if (p1attack - p2defense <= 0)
        {
            modifiedP1attack = 0;
            GameObject.FindWithTag("P1IteratingAttack").GetComponent<Text>().text = modifiedP1attack.ToString();
            GameObject.FindWithTag("P2IteratingDefense").GetComponent<Text>().text = 0.ToString();
            GetComponent<AudioSource>().clip = AddStat;
            GetComponent<AudioSource>().Play();
            yield return new WaitForSeconds(damageCalcSpeed);
        }
        else
        {
            modifiedP1attack = p1attack - p2defense;
            GameObject.FindWithTag("P1IteratingAttack").GetComponent<Text>().text = modifiedP1attack.ToString();
            GameObject.FindWithTag("P2IteratingDefense").GetComponent<Text>().text = 0.ToString();
            GetComponent<AudioSource>().clip = AddStat;
            GetComponent<AudioSource>().Play();
            yield return new WaitForSeconds(damageCalcSpeed);
        }

        if (p2attack - p1defense <= 0)
        {
            modifiedP2attack = 0;
            GameObject.FindWithTag("P2IteratingAttack").GetComponent<Text>().text = modifiedP2attack.ToString();
            GameObject.FindWithTag("P1IteratingDefense").GetComponent<Text>().text = 0.ToString();
            GetComponent<AudioSource>().clip = AddStat;
            GetComponent<AudioSource>().Play();
            yield return new WaitForSeconds(damageCalcSpeed);
        }
        else
        {
            modifiedP2attack = p2attack - p1defense;
            GameObject.FindWithTag("P2IteratingAttack").GetComponent<Text>().text = modifiedP2attack.ToString();
            GameObject.FindWithTag("P1IteratingDefense").GetComponent<Text>().text = 0.ToString();
            GetComponent<AudioSource>().clip = AddStat;
            GetComponent<AudioSource>().Play();
            yield return new WaitForSeconds(damageCalcSpeed);
        }

        do
        {
            yield return null;
        } while (GameObject.FindWithTag("Narration").GetComponent<Narrative>().isTyping);
        yield return new WaitForSeconds(1);

        if (modifiedP1attack > modifiedP2attack)
        {
            for (int i = 0; i < P1Field.Count; i++)
            {
                StartCoroutine(P1Field[i].GetComponent<Card>().Win());
            }

            for (int i = 0; i < P2Field.Count; i++)
            {
                StartCoroutine(P2Field[i].GetComponent<Card>().Lose());
            }

            damage = modifiedP1attack - modifiedP2attack;
            player2Health -= damage;
            lastTookDamage = "Player2";
            StartCoroutine(GameObject.FindWithTag("Narration").GetComponent<Narrative>().NewText("The round ends with player 2 taking " + damage.ToString() + " damage."));
            yield return new WaitForSeconds(1);
            GameObject.FindWithTag("DamageAnimation").GetComponent<Animator>().SetBool("isDamaged", true);
            GetComponent<AudioSource>().clip = TakeDamage;
            GetComponent<AudioSource>().Play();
        }
        else
        {
            if (modifiedP1attack < modifiedP2attack)
            {
                for (int i = 0; i < P1Field.Count; i++)
                {
                    StartCoroutine(P1Field[i].GetComponent<Card>().Lose());
                }

                for (int i = 0; i < P2Field.Count; i++)
                {
                    StartCoroutine(P2Field[i].GetComponent<Card>().Win());
                }

                damage = modifiedP2attack - modifiedP1attack;
                player1Health -= damage;
                lastTookDamage = "Player1";
                StartCoroutine(GameObject.FindWithTag("Narration").GetComponent<Narrative>().NewText("The round ends with player 1 taking " + damage.ToString() + " damage."));
                yield return new WaitForSeconds(1);
                GameObject.FindWithTag("DamageAnimation").GetComponent<Animator>().SetBool("isDamaged", true);
                GetComponent<AudioSource>().clip = TakeDamage;
                GetComponent<AudioSource>().Play();
            }
            else
            {
                StartCoroutine(GameObject.FindWithTag("Narration").GetComponent<Narrative>().NewText("Neither player takes damage. The round ends in a draw."));
                lastTookDamage = "Draw";
            }
        }

        yield return new WaitForSeconds(3.0f);

        player1Field.Clear();
        player2Field.Clear();

        GameObject[] objects;
        objects = GameObject.FindGameObjectsWithTag("Card");
        foreach (GameObject go in objects)
        {
            if (go.GetComponent<Card>().deployed)
            {
                go.transform.Find("Sword Glow").gameObject.SetActive(false);
                go.transform.Find("Shield Glow").gameObject.SetActive(false);
                go.GetComponent<Card>().Discard();
            }
            yield return new WaitForSeconds(.1f);
        }

        for (int i = 0; i < player1Revived.Count; i++)
        {
            player1Revived[i].transform.Find("Sword Glow").gameObject.SetActive(false);
            player1Revived[i].transform.Find("Shield Glow").gameObject.SetActive(false);
            player1Revived[i].GetComponent<Card>().Discard();
        }
        player1Revived.Clear();

        for (int i = 0; i < player2Revived.Count; i++)
        {
            player2Revived[i].transform.Find("Sword Glow").gameObject.SetActive(false);
            player2Revived[i].transform.Find("Shield Glow").gameObject.SetActive(false);
            player2Revived[i].GetComponent<Card>().Discard();
        }
        player2Revived.Clear();
        StatIcons.SetActive(false);

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
            do
            {
                yield return null;
            } while (GameObject.FindWithTag("Narration").GetComponent<Narrative>().isTyping);
            elligibleChoices[Random.Range(0, elligibleChoices.Count)].GetComponent<Card>().Deploy();
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
            GameObject.FindWithTag("Pass").GetComponent<Button>().interactable = false;
            StartCoroutine(GameObject.FindWithTag("Narration").GetComponent<Narrative>().NewText("The Energy Pool is empty. Ending round."));
            gameState = GameState.EndRound;
        }
        else
        {
            if (player2Passed)
            {
                if (player1Passed)
                {
                    GameObject.FindWithTag("Pass").GetComponent<Button>().interactable = false;
                    StartCoroutine(GameObject.FindWithTag("Narration").GetComponent<Narrative>().NewText("Both players have passed. Ending round."));
                    gameState = GameState.EndRound;
                }
                else
                {
                    GameObject.FindWithTag("Pass").GetComponent<Button>().interactable = false;
                    StartCoroutine(GameObject.FindWithTag("Narration").GetComponent<Narrative>().NewText("Player 2 has passed. Switching to player 1."));
                    GetComponent<AudioSource>().clip = SwitchTurn;
                    GetComponent<AudioSource>().Play();
                    gameState = GameState.Player1Turn;
                }
            }
            else
            {
                if(player1Passed)
                {
                    opponentMove = false;
                }
                else
                {
                    gameState = GameState.Player1Turn;
                }
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

    IEnumerator Draw4(List<GameObject> Deck, List<GameObject> Hand, bool P1)
    {
        GetComponent<AudioSource>().clip = Draw4Cards;
        GetComponent<AudioSource>().Play();
        for (int i = 0; i < 4; i++)
        {
            if (P1)
            {
                GameObject newCard = Instantiate(Deck[0], GameObject.FindWithTag("Player1Deck").transform.position, Quaternion.identity);
                newCard.GetComponent<Card>().isPlayer1 = true;
                StartCoroutine(newCard.GetComponent<Card>().Draw());
                Hand.Add(newCard);
                Deck.Remove(Deck[0]);
                yield return new WaitForSeconds(.5f);
                newCard.GetComponent<Card>().AddToHand();
            }
            else
            {
                GameObject newCard = Instantiate(Deck[0], GameObject.FindWithTag("Player2Deck").transform.position, Quaternion.identity);
                StartCoroutine(newCard.GetComponent<Card>().Draw());
                Hand.Add(newCard);
                Deck.Remove(Deck[0]);
                yield return new WaitForSeconds(.5f);
                newCard.GetComponent<Card>().AddToHand();
            }
        }
        yield return new WaitForSeconds(3.0f);
        DistributeCardsinHand();
    }

    IEnumerator Draw1(List<GameObject> Deck, List<GameObject> Hand, List<GameObject> Discard, bool P1)
    {
        if (Hand.Count < 5)
        {
            GetComponent<AudioSource>().clip = Draw1Card;
            GetComponent<AudioSource>().Play();
            if (Deck.Count == 0)
            {
                for (int i = 0; i < Discard.Count; i++)
                {
                    Deck.Add(Resources.Load("Prefabs/ActualCards/" + Discard[i].GetComponent<Card>().cardName) as GameObject);
                    Discard[i].GetComponent<Card>().status = "Recycled";
                }
                Discard.Clear();
                //Debug.Log("No more cards in deck, shuffling discard pile into deck.");
                Shuffle(Deck);
            }

            if (P1)
            {
                GameObject newCard = Instantiate(Deck[0], GameObject.FindWithTag("Player1Deck").transform.position, Quaternion.identity);
                StartCoroutine(newCard.GetComponent<Card>().Draw());
                Hand.Add(newCard);
                Deck.Remove(Deck[0]);
                yield return new WaitForSeconds(.5f);
                newCard.GetComponent<Card>().AddToHand();
            }
            else
            {
                GameObject newCard = Instantiate(Deck[0], GameObject.FindWithTag("Player2Deck").transform.position, Quaternion.identity);
                StartCoroutine(newCard.GetComponent<Card>().Draw());
                Hand.Add(newCard);
                Deck.Remove(Deck[0]);
                yield return new WaitForSeconds(.5f);
                newCard.GetComponent<Card>().AddToHand();
            }
        }
        DistributeCardsinHand();
    }

    IEnumerator BeginGame()
    {
        Shuffle(player1Deck);
        Shuffle(player2Deck);

        StartCoroutine(Draw4(player1Deck, player1Hand, true));
        StartCoroutine(Draw4(player2Deck, player2Hand, false));

        player1Health = 20;
        player2Health = 20;
        maxEnergyPool = 5;
        currentEnergyPool = maxEnergyPool;
        roundNumber = 1;
        gameState = GameState.Player1Turn;
        //Debug.Log("Switching GameState.");
        player1Starts = true;
        yield return null;
    }

    void DistributeCardsinHand()
    {
        int deployed1 = 0;
        int deployed2 = 0;
        if (player1Hand.Count != 0)
        {
            int first = -25;
            int bottomRow = 0;
            int topRow = 0;
            for (int i = 0; i < player1Hand.Count; i++)
            {
                player1Hand[i].GetComponent<Card>().isPlayer1 = true;
                if (player1Hand[i].GetComponent<Card>().deployed)
                {
                    topRow++;
                    player1Hand[i].transform.position = new Vector2(first + topRow * 10, -14);
                    deployed1++;
                }
                else
                {
                    bottomRow++;
                    player1Hand[i].transform.position = new Vector2(first + bottomRow * 10, -28);
                }
            }
            for (int i = 0; i < player1Revived.Count; i++)
            {
                player1Revived[i].transform.position = new Vector2((first + deployed1 * 10) + 10*(i+1), -14);
            }
        }
        if (player2Hand.Count != 0)
        {
            int second = -25;
            int bottomRow = 0;
            int topRow = 0;
            for (int i = 0; i < player2Hand.Count; i++)
            {
                if (player2Hand[i].GetComponent<Card>().deployed)
                {
                    topRow++;
                    player2Hand[i].transform.position = new Vector2(second + topRow * 10, 14);
                    deployed2++;
                }
                else
                {
                    bottomRow++;
                    player2Hand[i].transform.position = new Vector2(second + bottomRow * 10, 28);
                }
            }
            for (int i = 0; i < player2Revived.Count; i++)
            {
                player2Revived[i].transform.position = new Vector2((second + deployed2 * 10) + 10 * (i + 1), 14);
            }
        }
    }
}