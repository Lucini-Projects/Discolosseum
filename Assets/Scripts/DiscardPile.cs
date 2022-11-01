using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class DiscardPile : MonoBehaviour
{
    public bool player1 = true;
    void Update()
    {
        string cardNames = null;
        if (player1)
        {
            for (int i = 0; i < GameManager.player1Discard.Count; i++)
            {
                cardNames += GameManager.player1Discard[i].GetComponent<Card>().cardName + " ";
            }
            GameObject.FindWithTag("Player1Discard").GetComponent<Text>().text = cardNames;
        }
        else
        {
            for (int i = 0; i < GameManager.player2Discard.Count; i++)
            {
                cardNames += GameManager.player2Discard[i].GetComponent<Card>().cardName + " ";
            }
            GameObject.FindWithTag("Player2Discard").GetComponent<Text>().text = cardNames;
        }
    }
}
