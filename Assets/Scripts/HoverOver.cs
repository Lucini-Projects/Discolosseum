using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HoverOver : MonoBehaviour
{
    private Color OriginalColor;

    void Start()
    {
        OriginalColor = GetComponent<Image>().color;
    }

    void OnMouseOver()
    {
        GetComponent<Image>().color = Color.yellow;
    }

    void OnMouseExit()
    {
        GetComponent<Image>().color = OriginalColor;
    }
}
