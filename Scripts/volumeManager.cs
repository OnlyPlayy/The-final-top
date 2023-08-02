using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class volumeManager : MonoBehaviour
{
    [SerializeField] Slider volumeSlider;

    void Start()
    {
        // get volume level from PlayerPrefs or load volume level
        if(!PlayerPrefs.HasKey("musicVolume"))
        {
            PlayerPrefs.SetFloat("musicVolume", 1);
            Load();
        }
        else{
            Load();
        }
    }

    public void ChangeVolume()
    {
        // get value from slider and assign to sound volume 
        AudioListener.volume = volumeSlider.value;
        Save();
    }

    private void Load()
    {
        // load volume level from PlayerPrefs
        volumeSlider.value = PlayerPrefs.GetFloat("musicVolume");
    }

    private void Save() 
    {
        // save volume level to PlayerPrefs
        PlayerPrefs.SetFloat("musicVolume", volumeSlider.value);
    }
}