using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PassTurn : MonoBehaviour
{
    public void Player1TurnPass()
    {
        GameManager.player1Passed = true;
        GameManager.opponentMove = false;
    }
}
