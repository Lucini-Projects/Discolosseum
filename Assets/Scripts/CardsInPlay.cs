using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CardsInPlay : MonoBehaviour
{
    void Update()
    {
        GetComponent<Text>().text = "Cards In Play: " + GameObject.FindGameObjectsWithTag("Card").Length.ToString();
    }
}
