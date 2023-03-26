using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HelpScreenManager : MonoBehaviour
{
    int index = 0;
    public List<GameObject> Screens = new List<GameObject>();

    void Start()
    {
        for (int i = 0; i < Screens.Count; i++)
        {
            if (Screens[index] != Screens[i])
            {
                Screens[i].SetActive(false);
            }
        }
    }

    public void Increment()
    {
        index++;
        if (index > 5)
        {
            index = 0;
            for (int i = 0; i < Screens.Count; i++)
            {
                if (Screens[index] != Screens[i])
                {
                    Screens[i].SetActive(false);
                }
            }
        }
        else
        {
            Screens[index].SetActive(true);
        }
    }

    public void Decrement()
    {
        Screens[index].SetActive(false);
        index--;
    }
}