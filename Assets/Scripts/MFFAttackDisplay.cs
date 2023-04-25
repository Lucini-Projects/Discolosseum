using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MFFAttackDisplay : MonoBehaviour
{
    public Sprite ZeroSpright;
    public Sprite OneSpright;
    public Sprite TwoSpright;
    public Sprite ThreeSpright;
    public Sprite FourSpright;
    public Sprite FiveSpright;
    public Sprite SixSpright;

    // Update is called once per frame
    void Update()
    {
        switch(GetComponentInParent<Card>().attack)
        {
            default:
                break;
            case 0:
                GetComponent<SpriteRenderer>().sprite = ZeroSpright;
                break;
            case 1:
                GetComponent<SpriteRenderer>().sprite = OneSpright;
                break;
            case 2:
                GetComponent<SpriteRenderer>().sprite = TwoSpright;
                break;
            case 3:
                GetComponent<SpriteRenderer>().sprite = ThreeSpright;
                break;
            case 4:
                GetComponent<SpriteRenderer>().sprite = FourSpright;
                break;
            case 5:
                GetComponent<SpriteRenderer>().sprite = FiveSpright;
                break;
            case 6:
                GetComponent<SpriteRenderer>().sprite = SixSpright;
                break;
        }
    }
}
