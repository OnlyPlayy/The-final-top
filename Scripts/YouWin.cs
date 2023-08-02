using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class YouWin : MonoBehaviour
{
    public Text pointText;
    public Text healthsText;
    public Text scoreText;
    public Text highscoreText;

    private int score;
    private int highscore;
    private int point;
    private int health;

    // gain access from another script
    public static YouWin instance;
    private void Awake(){
        instance = this;
    }

    public void Setup(){
        
        Cursor.lockState = CursorLockMode.None;
        gameObject.SetActive(true);

        // Get score values
        point = PointSystem.instance.point;
        health = Health.instance.health;

        // Calculate score
        score = point + health;
        highscore = PlayerPrefs.GetInt("highscore");
        if(highscore < score) {
            highscore = score;
            PlayerPrefs.SetInt("highscore", score);
        }

        // Display text on win screen
        pointText.text = "Points: " + point.ToString();
        healthsText.text = "Healths: " + health.ToString();
        scoreText.text = "Score: " + score.ToString();
        highscoreText.text = "Highscore: " + highscore.ToString();
    }
}
