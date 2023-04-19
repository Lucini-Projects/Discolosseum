﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BackgroundPick : MonoBehaviour
{
    public Sprite[] backGround;

    void Start()
    {
        int picker = Random.Range(0, backGround.Length);
        GetComponent<SpriteRenderer>().sprite = backGround[picker];
    }
}