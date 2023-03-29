using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NoDefense : MonoBehaviour
{
    public bool noDefense = true;
    int attack = 0;

    public Sprite TwoSpright;
    public Sprite FourSpright;
    // Update is called once per frame
    void Update()
    {
        attack = GetComponentInParent<Card>().attack;
        if (attack == 2)
        {
            noDefense = true;
        }
        else
        {
            noDefense = false;
        }

        if (noDefense)
        {
            GetComponent<SpriteRenderer>().sprite = TwoSpright;
        }
        else
        {
            GetComponent<SpriteRenderer>().sprite = FourSpright;
        }
    }
}
