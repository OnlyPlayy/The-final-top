using System.Collections;
using UnityEngine;

public class GameOver : MonoBehaviour
{
    // gain access from another script
    public static GameOver instance;
    private void Awake(){
        instance = this;
    }

    public void Setup(){        
        Cursor.lockState = CursorLockMode.None;
        gameObject.SetActive(true);
    }

    public void Hide(){
        gameObject.SetActive(false);
    }
}