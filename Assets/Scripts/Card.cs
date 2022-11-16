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
    public Sprite cardBack;
    public Sprite cardFront;

    public bool isPlayer1;
    bool canPlay;
    public bool deployed;
    bool privateKnowledge;
    public bool discarded;

    GameObject discardPile;

    Color playable = Color.yellow;
    Color originalColor;

    public AudioClip PlayCard;
    public AudioClip PlaySovereignCard;

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
        cardFront = GetComponent<SpriteRenderer>().sprite;
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
                        transform.GetChild(0).GetComponent<Animator>().SetBool("IsUsable", false);
                    }
                    else
                    {
                        if (GameManager.currentlyPlayer1Turn)
                        {
                            canPlay = true;
                            transform.GetChild(0).GetComponent<Animator>().SetBool("IsUsable", true);
                        }
                        else
                        {
                            canPlay = false;
                            transform.GetChild(0).GetComponent<Animator>().SetBool("IsUsable", true);
                        }
                    }
                }
                else
                {
                    if (!deployed)
                    {
                        GetComponent<SpriteRenderer>().sprite = cardBack;
                        privateKnowledge = true;
                    }
                    else
                    {
                        privateKnowledge = false;
                        GetComponent<SpriteRenderer>().sprite = cardFront;
                    }
                }
            }
        }
    }

    void OnMouseOver()
    {
        GameObject.FindWithTag("Details").GetComponent<RectTransform>().anchoredPosition = new Vector2(-670, 0);
        GameObject.FindWithTag("Details").GetComponent<Image>().sprite = GetComponent<SpriteRenderer>().sprite;
        if (!discarded)
        {
            if (GameManager.currentlyPlayer1Turn)
            {
                if (!privateKnowledge)
                {
                    if (Input.GetMouseButtonDown(0) && canPlay && !deployed)
                    {
                        if (className == "Sovereign")
                        {
                            GetComponent<AudioSource>().clip = PlaySovereignCard;
                            GetComponent<AudioSource>().Play();
                        }
                        else
                        {
                            GetComponent<AudioSource>().clip = PlayCard;
                            GetComponent<AudioSource>().Play();
                        }

                        deployed = true;
                        GameManager.player1Field.Add(this.gameObject);
                        //GameManager.player1Hand.Remove(this.gameObject);
                        
                        //Debug.Log("Pressed");
                        GameManager.currentEnergyPool -= energyCost;
                        GameManager.switchToPlayer2 = true;
                    }
                }
            }
        }
    }

    void OnMouseExit()
    {
        GameObject.FindWithTag("Details").GetComponent<RectTransform>().anchoredPosition = new Vector2(-670, 1000);
    }

    //For the enemy AI
    public void Deploy()
    {
        if (className == "Sovereign")
        {
            GetComponent<AudioSource>().clip = PlaySovereignCard;
            GetComponent<AudioSource>().Play();
        }
        else
        {
            GetComponent<AudioSource>().clip = PlayCard;
            GetComponent<AudioSource>().Play();
        }
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