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

    bool isPlayer1;
    bool canPlay;
    bool privateKnowledge;

    Color notPlayable = Color.red;
    Color playable = Color.green;

    //This stores the GameObject’s original color
    Color originalColor;

    //Get the GameObject’s mesh renderer to access the GameObject’s material and color
    SpriteRenderer m_Renderer;

    void Start()
    {
        //Fetch the mesh renderer component from the GameObject
        m_Renderer = GetComponent<SpriteRenderer>();
        //Fetch the original color of the GameObject
        originalColor = m_Renderer.material.color;
    }

    void OnMouseOver()
    {
        if (energyCost <= GameManager.currentEnergyPool)
        {
            m_Renderer.material.color = playable;
        }
        else
        {
            m_Renderer.material.color = notPlayable;
        }
        for (int i = 0; i < GameObject.FindGameObjectsWithTag("Display").Length; i++)
        {
            GameObject.FindWithTag("Display").transform.GetChild(0).GetComponent<Text>().text = cardName;
            GameObject.FindWithTag("Display").transform.GetChild(1).GetComponent<Text>().text = className;
            GameObject.FindWithTag("Display").transform.GetChild(2).GetComponent<Text>().text = attack.ToString();
            GameObject.FindWithTag("Display").transform.GetChild(3).GetComponent<Text>().text = defense.ToString();
            GameObject.FindWithTag("Display").transform.GetChild(4).GetComponent<Text>().text = energyCost.ToString();
        }
    }

    void OnMouseExit()
    {
        // Reset the color of the GameObject back to normal
        m_Renderer.material.color = originalColor;
    }
}