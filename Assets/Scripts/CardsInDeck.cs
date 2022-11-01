using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CardsInDeck : MonoBehaviour
{
    public bool player1Deck;

    void Update()
    {
        if (player1Deck)
        {
            GetComponent<Text>().text = GameManager.player1DeckCount.ToString();
            GameObject.FindWithTag("Player1DeckCount").GetComponent<Text>().text = GetComponent<Text>().text;
        }
        else
        {
            GetComponent<Text>().text = GameManager.player2DeckCount.ToString();
            GameObject.FindWithTag("Player2DeckCount").GetComponent<Text>().text = GetComponent<Text>().text;
        }
    }
}
