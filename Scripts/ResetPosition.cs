using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResetPosition : MonoBehaviour
{
    public float highPosition = 30f;
    public float lowPosition = -10f;
    public float resetPosition = 20f;

    // Update is called once per frame
    void Update()
    {
        if(transform.position.y < lowPosition)
        {
            Debug.Log("This moment");
            transform.position = new Vector3(transform.position.x, resetPosition, transform.position.z);
            Debug.Log(transform.position.y);
        }
    }
}
