using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DuolphinAttack : MonoBehaviour
{
    public Sprite ZeroSpright;
    public Sprite TwoSpright;


    // Update is called once per frame
    void Update()
    {
        switch (GetComponentInParent<Card>().attack)
        {
            default:
                break;
            case 0:
                GetComponent<SpriteRenderer>().sprite = ZeroSpright;
                break;
            case 2:
                GetComponent<SpriteRenderer>().sprite = TwoSpright;
                break;
        }
    }
}