using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Card : MonoBehaviour
{
    [SerializeField] private CardInfo[] deckOfCards;
    private CardInfo temporary;

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

[System.Serializable]
public class CardInfo
{
    public string name;
    public string className;
    public int energyCost;
    public int attack;
    public int defense;
}
