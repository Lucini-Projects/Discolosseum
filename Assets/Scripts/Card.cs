using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Card : MonoBehaviour
{
    public string cardName;
    public string className;
    public int energyCost;
    public int attack;
    public int defense;

    public bool isPlayer1;
    bool canPlay;
    public bool deployed;
    bool privateKnowledge;
    public bool discarded;

    GameObject discardPile;

    Color playable = Color.yellow;
    Color originalColor;


    //This is currently unused. Cards at the moment are deleted, but discard piles exist as a backup.
    void Start()
    {
        if (isPlayer1)
        {
            discardPile = GameObject.FindWithTag("Player1DiscardPile");
        }
        else
        {
            discardPile = GameObject.FindWithTag("Player2DiscardPile");
        }
        originalColor = GetComponent<SpriteRenderer>().material.color;
    }

    void Update()
    {
        if (!discarded)
        {
            if (GameManager.discard == true)
            {
                Discard();
            }
            else
            {
                if (isPlayer1)
                {
                    privateKnowledge = false;
                    if (energyCost > GameManager.currentEnergyPool || deployed)
                    {
                        canPlay = false;
                        transform.GetChild(0).GetComponent<SpriteRenderer>().material.color = originalColor;
                    }
                    else
                    {
                        canPlay = true;
                        transform.GetChild(0).GetComponent<SpriteRenderer>().material.color = playable;
                    }
                }
                else
                {
                    if (!deployed)
                    {
                        transform.GetChild(0).GetComponent<SpriteRenderer>().material.color = Color.black;
                        GetComponent<SpriteRenderer>().material.color = Color.black;
                        privateKnowledge = true;
                    }
                    else
                    {
                        privateKnowledge = false;
                        transform.GetChild(0).GetComponent<SpriteRenderer>().material.color = originalColor;
                        GetComponent<SpriteRenderer>().material.color = originalColor;
                    }
                }
            }
        }
    }

    void OnMouseOver()
    {
        if (!discarded)
        {
            if (GameManager.currentlyPlayer1Turn)
            {
                if (!privateKnowledge)
                {
                    if (Input.GetMouseButtonDown(0) && canPlay && !deployed)
                    {
                        //transform.position = new Vector2(transform.position.x, transform.position.y + 10);
                        deployed = true;
                        GameManager.player1Field.Add(this.gameObject);
                        //GameManager.player1Hand.Remove(this.gameObject);
                        
                        //Debug.Log("Pressed");
                        GameManager.currentEnergyPool -= energyCost;
                        GameManager.switchToPlayer2 = true;
                    }
                    for (int i = 0; i < GameObject.FindGameObjectsWithTag("Display").Length; i++)
                    {
                        GameObject.FindWithTag("Display").transform.GetChild(0).GetComponent<Text>().text = cardName;
                        GameObject.FindWithTag("Display").transform.GetChild(1).GetComponent<Text>().text = className + " Class";
                        GameObject.FindWithTag("Display").transform.GetChild(2).GetComponent<Text>().text = "Attack: " + attack.ToString();
                        GameObject.FindWithTag("Display").transform.GetChild(3).GetComponent<Text>().text = "Defense: " + defense.ToString();
                        GameObject.FindWithTag("Display").transform.GetChild(4).GetComponent<Text>().text = "Cost: " + energyCost.ToString();
                    }
                }
            }
        }
    }

    void OnMouseExit()
    {
        for (int i = 0; i < GameObject.FindGameObjectsWithTag("Display").Length; i++)
        {
            GameObject.FindWithTag("Display").transform.GetChild(0).GetComponent<Text>().text = "";
            GameObject.FindWithTag("Display").transform.GetChild(1).GetComponent<Text>().text = "";
            GameObject.FindWithTag("Display").transform.GetChild(2).GetComponent<Text>().text = "";
            GameObject.FindWithTag("Display").transform.GetChild(3).GetComponent<Text>().text = "";
            GameObject.FindWithTag("Display").transform.GetChild(4).GetComponent<Text>().text = "";
        }
    }

    //For the enemy AI
    public void Deploy()
    {
        //transform.position = new Vector2(transform.position.x, transform.position.y-10);
        deployed = true;
        Debug.Log("Enemy AI has deployed " + cardName + ", which has " + attack.ToString() + " attack and " + defense.ToString() + " defense.");
        GameManager.currentEnergyPool -= energyCost;
        GameManager.player2Field.Add(this.gameObject);
        //GameManager.player2Hand.Remove(this.gameObject);
    }

    //To remove from play.
    public void Discard()
    {
        if (deployed)
        {
            deployed = false;
            discarded = true;
            //GameObject duplicateCard = this.gameObject;
            if(isPlayer1)
            {
                GameManager.player1Discard.Add(this.gameObject);
                GameManager.player1Hand.Remove(this.gameObject);
            }
            else
            {
                GameManager.player2Discard.Add(this.gameObject);
                GameManager.player2Hand.Remove(this.gameObject);
            }
            transform.position = discardPile.transform.position;
            //Destroy(this.gameObject);
        }
    }
}