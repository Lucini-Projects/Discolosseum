using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Narrative : MonoBehaviour
{
    public bool isTyping = false;
    public float textSpeed = 30f;

    public IEnumerator NewText(string newText)
    {
        isTyping = true;
        GetComponent<Text>().text = "";
        foreach (char c in newText)
        {
            GetComponent<Text>().text += c;
            yield return new WaitForSeconds(.025f);
        }
        yield return new WaitForSeconds(.25f);
        isTyping = false;
    }
}
