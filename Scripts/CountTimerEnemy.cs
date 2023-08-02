using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CountTimerEnemy : MonoBehaviour
{
    bool timerActive = false;
    Transform cam;
    public Text currentTimeText;
    public float countTime;

    // gain access from another script
    public static CountTimerEnemy instance;
    private void Awake(){
        instance = this;
    }

    void Start()
    {
        HideTimer();
        cam = Camera.main.transform;
    }

    void Update(){
        if(timerActive){
            countTime = countTime - Time.deltaTime;
        }
        if (countTime >= 0 && timerActive)
            currentTimeText.text = (Mathf.RoundToInt(countTime * 10) * 0.1f).ToString();
        else if (countTime < 0 ){
            StopTimer();
            HideTimer();
        } 
    }

    void LateUpdate()
    {
        transform.LookAt(transform.position + cam.forward);
    }

    public void StartTimer() {
        if(!timerActive){
            Setup();
            timerActive = true;
            countTime = 5;
        }
    }

    public void StopTimer() {
        timerActive = false;
    }

    public void HideTimer(){
        gameObject.SetActive(false);
    }
    
    public void Setup(){
        gameObject.SetActive(true);
    }
}
