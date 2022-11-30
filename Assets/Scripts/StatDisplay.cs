using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StatDisplay : MonoBehaviour
{
    public bool Player1;
    public bool Attack;

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(Player1)
        {
            if(Attack)
            {
                int attackTotal = 0;
                for (int i = 0; i < GameManager.player1Field.Count; i++)
                {
                    attackTotal += GameManager.player1Field[i].GetComponent<Card>().attack;
                }
                GetComponent<Text>().text = "Total Attack: " + attackTotal.ToString();
            }
            else
            {
                int defenseTotal = 0;
                for (int i = 0; i < GameManager.player1Field.Count; i++)
                {
                    defenseTotal += GameManager.player1Field[i].GetComponent<Card>().defense;
                }
                GetComponent<Text>().text = "Total Defense: " + defenseTotal.ToString();
            }
        }
        else
        {
            if (Attack)
            {
                int attackTotal = 0;
                for (int i = 0; i < GameManager.player2Field.Count; i++)
                {
                    attackTotal += GameManager.player2Field[i].GetComponent<Card>().attack;
                }
                GetComponent<Text>().text = "Total Attack: " + attackTotal.ToString();
            }
            else
            {
                int defenseTotal = 0;
                for (int i = 0; i < GameManager.player2Field.Count; i++)
                {
                    defenseTotal += GameManager.player2Field[i].GetComponent<Card>().defense;
                }
                GetComponent<Text>().text = "Total Defense: " + defenseTotal.ToString();
            }
        }
    }
}
