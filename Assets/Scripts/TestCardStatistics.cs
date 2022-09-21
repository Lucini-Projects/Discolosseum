using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TestCardStatistics : MonoBehaviour
{
    public string cardName;
    public string className;
    public int energyCost;
    public int attack;
    public int defense;

    //When the mouse hovers over the GameObject, it turns to this color (red)
    Color m_MouseOverColor = Color.red;

    //This stores the GameObject’s original color
    Color m_OriginalColor;

    //Get the GameObject’s mesh renderer to access the GameObject’s material and color
    SpriteRenderer m_Renderer;

    void Start()
    {
        //Fetch the mesh renderer component from the GameObject
        m_Renderer = GetComponent<SpriteRenderer>();
        //Fetch the original color of the GameObject
        m_OriginalColor = m_Renderer.material.color;
    }

    void OnMouseOver()
    {
        // Change the color of the GameObject to red when the mouse is over GameObject
        m_Renderer.material.color = m_MouseOverColor;
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
        m_Renderer.material.color = m_OriginalColor;
    }
}
