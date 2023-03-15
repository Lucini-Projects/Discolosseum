using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlaySound : MonoBehaviour
{
    //Sound Effect
    public AudioClip meowSound;

    //Time between meows
    public int interval = 0;

    
    bool alreadyMeowing;

    void Update()
    {
        if(!alreadyMeowing)
        {
            StartCoroutine(Meow());
        }
    }

    IEnumerator Meow()
    {
        alreadyMeowing = !alreadyMeowing;
        GetComponent<AudioSource>().clip = meowSound;
        GetComponent<AudioSource>().Play();
        yield return new WaitForSeconds(interval);
        alreadyMeowing = !alreadyMeowing;
    }
}