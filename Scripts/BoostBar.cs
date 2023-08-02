﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BoostBar : MonoBehaviour
{
    public Slider slider;
    
    public void SetBoost(float boost){
        slider.value = boost;
    }
}
