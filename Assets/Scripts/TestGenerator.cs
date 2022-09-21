using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestGenerator : MonoBehaviour
{
    public GameObject prefab;
    public int dispersion;
    public int cardCount;

    void Start()
    {
        for (int i = 0; i < cardCount; i++)
        {
            GameObject newCard = Instantiate(prefab, new Vector2(i * dispersion, 0), Quaternion.identity);
            newCard.GetComponent<TestCardStatistics>().name = "card " + (i + 1).ToString();
            newCard.GetComponent<TestCardStatistics>().energyCost = Random.Range(1, 6);
            newCard.GetComponent<TestCardStatistics>().attack = Random.Range(1, 6);
            newCard.GetComponent<TestCardStatistics>().defense = Random.Range(1, 6);

            int randomClassPicker = Random.Range(1, 3);
            switch (randomClassPicker)
            {
                default:
                    Debug.Log("This is an error. Fix it.");
                    break;
                case 1:
                    newCard.GetComponent<TestCardStatistics>().className = "Common";
                    break;
                case 2:
                    newCard.GetComponent<TestCardStatistics>().className = "Royal";
                    break;
                case 3:
                    newCard.GetComponent<TestCardStatistics>().className = "Sovereign";
                    break;
            }
        }
    }
}