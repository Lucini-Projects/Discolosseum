using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlaySound : MonoBehaviour
{
    //Sound Effect
    public List<AudioClip> SoundEffects = new List<AudioClip>();

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
        int chosenSound = Random.Range(0, SoundEffects.Count);
        GetComponent<AudioSource>().clip = SoundEffects[chosenSound];
        GetComponent<AudioSource>().Play();
        yield return new WaitForSeconds(interval);
        alreadyMeowing = !alreadyMeowing;
    }
}