using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PassTurn : MonoBehaviour
{
    public void Player1TurnPass()
    {
        GameManager.player1Passed = true;
    }
}
