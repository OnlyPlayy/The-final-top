using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneControl : MonoBehaviour
{
    public void RestartButton() {
        SceneManager.LoadScene("Level1");
        Time.timeScale = 1f;
    }

    public void MainMenuButton() {
        SceneManager.LoadScene("MainMenu");
        FindObjectOfType<AudioManager>().Play("MenuMusic");
        Time.timeScale = 1f;
    }
}