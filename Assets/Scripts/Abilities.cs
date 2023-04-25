using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Abilities : MonoBehaviour
{
    void Start()
    {

    }

    void Update()
    {
        switch (GetComponent<Card>().cardName)
        {
            default:
                break;
            case "Duolphin":
                if (GetComponent<Card>().isPlayer1 && !GameManager.player1Starts || !GetComponent<Card>().isPlayer1 && GameManager.player1Starts)
                {
                    GetComponent<Card>().attack = 2;
                }
                else
                {
                    GetComponent<Card>().attack = 0;
                }
                break;

            case "Monkeyfishfrog":
                if (!GetComponent<Card>().abilityUsed)
                {
                    MFF();
                }
                break;
            case "Raivolope":
                if (NoDefense())
                {
                    GetComponent<Card>().attack = 2;
                }
                else
                {
                    GetComponent<Card>().attack = 4;
                }
                break;
            case "Earth Wyrm":
                if (!GetComponent<Card>().abilityUsed && (GetComponent<Card>().deployed || GetComponent<Card>().status == "Revived"))
                {
                    EarthWyrm();
                }
                break;
            case "Kalju":
                if (!GetComponent<Card>().abilityUsed && GetComponent<Card>().deployed)
                {
                    Kalju();
                }
                break;
        }
    }

    bool NoDefense()
    {
        int defenseTotal = 0;
        if (GetComponent<Card>().isPlayer1)
        {
            for (int i = 0; i < GameManager.player1Hand.Count; i++)
            {
                if (GameManager.player1Hand[i].GetComponent<Card>().deployed)
                {
                    defenseTotal += GameManager.player1Hand[i].GetComponent<Card>().defense;
                }
            }
        }
        else
        {
            for (int i = 0; i < GameManager.player2Hand.Count; i++)
            {
                if (GameManager.player2Hand[i].GetComponent<Card>().deployed)
                {
                    defenseTotal += GameManager.player2Hand[i].GetComponent<Card>().defense;
                }
            }
        }

        if (defenseTotal > 0)
        {
            return false;
        }
        else
        {
            return true;
        }
    }

    void EarthWyrm()
    {
        if (GetComponent<Card>().isPlayer1)
        {
            if (GameManager.player1Discard.Count > 0)
            {
                for (int i = 0; i < GameManager.player1Discard.Count; i++)
                {
                    if (GameManager.player1Discard[i].GetComponent<Card>().cardName == "Quake Snake")
                    {
                        GameManager.player1Discard[i].GetComponent<Card>().status = "Revived";
                        GameManager.player1Field.Add(GameManager.player1Discard[i]);
                        GameManager.player1Revived.Add(GameManager.player1Discard[i]);
                        //StartCoroutine(Revive(GameManager.player1Discard[i], GameManager.player1Discard));
                        GameManager.player1Discard.Remove(GameManager.player1Discard[i]);
                    }
                }
            }
        }
        else
        {
            if (GameManager.player2Discard.Count > 0)
            {
                for (int i = 0; i < GameManager.player2Discard.Count; i++)
                {
                    if (GameManager.player2Discard[i].GetComponent<Card>().cardName == "Quake Snake")
                    {
                        GameManager.player2Discard[i].GetComponent<Card>().status = "Revived";
                        GameManager.player2Field.Add(GameManager.player2Discard[i]);
                        GameManager.player2Revived.Add(GameManager.player2Discard[i]);
                        //StartCoroutine(Revive(GameManager.player2Discard[i], GameManager.player2Discard));
                        GameManager.player2Discard.Remove(GameManager.player2Discard[i]);
                    }
                }
            }
        }
        GetComponent<Card>().abilityUsed = true;
    }

    void MFF()
    {
        int rAttack = Random.Range(0, 101);
        if (rAttack < 40)
        {
            GetComponent<Card>().attack = 0;
        }
        else if (rAttack < 50)
        {
            GetComponent<Card>().attack = 1;
        }
        else if (rAttack < 60)
        {
            GetComponent<Card>().attack = 2;
        }
        else if (rAttack < 70)
        {
            GetComponent<Card>().attack = 3;
        }
        else if (rAttack < 85)
        {
            GetComponent<Card>().attack = 4;
        }
        else if (rAttack < 95)
        {
            GetComponent<Card>().attack = 5;
        }
        else
        {
            GetComponent<Card>().attack = 6;
        }
        
        Debug.Log("Monkeyfishfrog has " + GetComponent<Card>().attack + " attack.");
        GetComponent<Card>().abilityUsed = true;
    }

    void Kalju()
    {
        if (GetComponent<Card>().isPlayer1)
        {
            if (GameManager.player1Discard.Count > 0)
            {
                for (int i = 0; i < GameManager.player1Discard.Count; i++)
                {
                    if (GameManager.player1Discard[i].GetComponent<Card>().cardName == "Earth Wyrm")
                    {
                        GameManager.player1Discard[i].GetComponent<Card>().status = "Revived";
                        GameManager.player1Discard[i].GetComponent<Card>().abilityUsed = false;
                        GameManager.player1Field.Add(GameManager.player1Discard[i]);
                        GameManager.player1Revived.Add(GameManager.player1Discard[i]);
                        //StartCoroutine(Revive(GameManager.player1Discard[i], GameManager.player1Discard));
                        GameManager.player1Discard.Remove(GameManager.player1Discard[i]);

                    }
                }
            }
        }
        else
        {
            if (GameManager.player2Discard.Count > 0)
            {
                for (int i = 0; i < GameManager.player2Discard.Count; i++)
                {
                    if (GameManager.player2Discard[i].GetComponent<Card>().cardName == "Earth Wyrm")
                    {
                        GameManager.player2Discard[i].GetComponent<Card>().status = "Revived";
                        GameManager.player2Discard[i].GetComponent<Card>().abilityUsed = false;
                        GameManager.player2Field.Add(GameManager.player2Discard[i]);
                        GameManager.player2Revived.Add(GameManager.player2Discard[i]);
                        //(Revive(GameManager.player2Discard[i], GameManager.player2Discard));
                        GameManager.player2Discard.Remove(GameManager.player2Discard[i]);
                    }
                }
            }
        }
        GetComponent<Card>().abilityUsed = true;
    }

    IEnumerator Revive(GameObject card, List<GameObject> list)
    {
        yield return new WaitForSeconds(.25f);
        list.Remove(card);
    }
}
