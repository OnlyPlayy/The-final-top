using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CountTimer : MonoBehaviour
{
    bool timerActive = false;
    public Transform cam;
    public Text currentTimeText;
    public float countTime;
    string currentState;
    public bool position;

    // gain access from another script
    public static CountTimer instance;
    private void Awake(){
        instance = this;
    }

    void Update(){
        if(timerActive){
            countTime = countTime - Time.deltaTime;
        }
        if (countTime >= 0 && timerActive)
            currentTimeText.text = (Mathf.RoundToInt(countTime * 10) * 0.1f).ToString();
        else if (countTime < 0 )
        {
            StopTimer();
            if (position)
            {
                PositionSaved();
            }
        } 
    }

    void LateUpdate()
    {
        transform.LookAt(transform.position + cam.forward);
    }

    public void StartTimer() {
        if(!timerActive)
        {
            Setup();
            currentTimeText.fontSize = 100;
            timerActive = true;
            countTime = 3;
        }
    }

    public void PositionSaved() {
        currentTimeText.text = "Position saved!";
        gameObject.SetActive(true);
    }

    public void StopTimer() {
        currentTimeText.fontSize = 50;
        timerActive = false;
    }

    public void AlreadySaved() {
        currentTimeText.text = "Already saved!";
        gameObject.SetActive(true);
    }

    public void HideTimer(){
        gameObject.SetActive(false);
    }
    
    public void Setup(){
        gameObject.SetActive(true);
    }

    public void StartTitle()
    {
        currentTimeText.text = "Start!";
        gameObject.SetActive(true);
        position = false;
    }

}
