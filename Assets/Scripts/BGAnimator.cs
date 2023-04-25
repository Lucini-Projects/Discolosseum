using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BGAnimator : MonoBehaviour
{
    public List<Sprite> frames = new List<Sprite>();
    int place = 0;

    void Start()
    {
        StartCoroutine(Animate());
    }

    IEnumerator Animate()
    {
        yield return new WaitForSeconds(1.0f);
        GetComponent<SpriteRenderer>().sprite = frames[place];
        if (place >= frames.Count)
        {
            place = 0;
        }
        else
        {
            place++;
        }
        Debug.Log("reloop");
        Animate();
    }
}
