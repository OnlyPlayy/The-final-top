using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PointSystem : MonoBehaviour
{
    public Text pointText;

    [HideInInspector] public int point;
    
    // gain access from another script
    public static PointSystem instance;
    private void Awake(){
        instance = this;
    }

    void Start()
    {
        point = 0;
        pointText.text = "Points:  " + point.ToString();
    }

    public void AddPoint(int givenPoint) {
        point += givenPoint;
        pointText.text = "Points:  " + point.ToString();
    }
}
