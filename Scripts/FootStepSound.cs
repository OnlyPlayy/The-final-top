using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FootStepSound : MonoBehaviour
{
    AudioSource animationSoundStep;

    void Start(){
        animationSoundStep = GetComponent<AudioSource>();
    }

    private void Step(){
        animationSoundStep.Play();
    }
}
