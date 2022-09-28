using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestShuffle : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        Shuffle();
        //DisplayCardOrder();
        Draw4();
    }

    // Update is called once per frame
    void Update()
    {

    }

    void Shuffle()
    {
        for (int i = 0; i < GameObject.FindWithTag("GameManager").GetComponent<GameManager>().player1Deck.Count; i++)
        {
            GameObject temp = GameObject.FindWithTag("GameManager").GetComponent<GameManager>().player1Deck[i];
            int randomIndex = Random.Range(i, GameObject.FindWithTag("GameManager").GetComponent<GameManager>().player1Deck.Count);
            GameObject.FindWithTag("GameManager").GetComponent<GameManager>().player1Deck[i] = GameObject.FindWithTag("GameManager").GetComponent<GameManager>().player1Deck[randomIndex];
            GameObject.FindWithTag("GameManager").GetComponent<GameManager>().player1Deck[randomIndex] = temp;
        }
    }

    void DisplayCardOrder()
    {
        for (int i = 0; i < GameObject.FindWithTag("GameManager").GetComponent<GameManager>().player1Deck.Count; i++)
        {
            Debug.Log(i + ": " + GameObject.FindWithTag("GameManager").GetComponent<GameManager>().player1Deck[i].GetComponent<Card>().cardName);
        }
    }

    void Draw4()
    {
        for (int i = 0; i < 4; i++)
        {
            GameObject newCard = Instantiate(GameObject.FindWithTag("GameManager").GetComponent<GameManager>().player1Deck[i], new Vector2(i * 10, 0), Quaternion.identity);
        }
    }
}
