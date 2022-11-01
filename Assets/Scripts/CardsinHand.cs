using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CardsinHand : MonoBehaviour
{
    public bool player1;

    void Update()
    {
       if(player1)
       {
            GetComponent<Text>().text = "Cards in Hand: " + GameManager.player1Hand.Count.ToString();
       }
       else
       {
            GetComponent<Text>().text = "Cards in Hand: " + GameManager.player2Hand.Count.ToString();
        }
    }
}
