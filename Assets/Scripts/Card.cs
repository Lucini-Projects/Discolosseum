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

    Color playable = Color.yellow;

    //This stores the GameObject’s original color
    Color originalColor;

    void Start()
    {
        originalColor = GetComponent<SpriteRenderer>().material.color;
    }

    void Update()
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

    void OnMouseOver()
    {
        if (!privateKnowledge)
        {
            if (Input.GetMouseButtonDown(0) && canPlay && !deployed)
            {
                GameManager.player1Field.Add(this.gameObject);
                deployed = true;
                Debug.Log("Pressed");
                GameManager.currentEnergyPool -= energyCost;
                GameManager.SwitchTurn();
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
        deployed = true;
        Debug.Log("Enemy AI has deployed a card!");
        GameManager.currentEnergyPool -= energyCost;
        GameManager.player2Field.Add(this.gameObject);
    }
}