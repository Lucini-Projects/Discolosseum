using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Deck : MonoBehaviour
{
    public int deckSize;

    [SerializeField] private Card[] deckOfCards = null;
    public List<Card> deck = new List<Card>();
    private Card temporary;

    void Start()
    {
        Shuffle();
        DisplayOrder();
    }

    public void Shuffle()
    {
        for (int i = 0; i < deckOfCards.Length - 1; i++)
        {
            int placement = Random.Range(i, deckOfCards.Length);
            temporary = deckOfCards[placement];
            deckOfCards[placement] = deckOfCards[i];
            deckOfCards[i] = temporary;
        }
    }

    public void DisplayOrder()
    {
        for (int i = 0; i < deckOfCards.Length; i++)
        {
            Debug.Log(deckOfCards[i].name);
        }
    }
}