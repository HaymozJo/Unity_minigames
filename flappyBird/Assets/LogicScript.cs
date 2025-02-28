using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.Rendering;
using System;
using TMPro;

public class LogicScript : MonoBehaviour
{

    public int playerScore;
    public Text scoreText;
    public Text nameText;
    public Text highScoreText;
    public GameObject gameOverScreen;
    public GameObject highScoreScreen;
    public GameObject nameScreen;
    public TMP_InputField inputField;
    public GameObject menuScreen;

    private bool alreadyPlayed = false;
    private int highScore;
    private string playerName;

    [ContextMenu("Increase Score")]
    public void addScore(int scoreToAdd)
    {
        playerScore += scoreToAdd;
        scoreText.text = playerScore.ToString();
    }

    public void SetName()
    {
        PlayerPrefs.SetString("Name", inputField.text);
    }

    public void ApplyName()
    {
        nameText.text = PlayerPrefs.GetString("Name");
    }


    public void SetHighScore()
    {
        playerName = PlayerPrefs.GetString("Name").ToString();
        string highscoreStr = "HS" + playerName;
        Debug.Log(highscoreStr);

        alreadyPlayed = PlayerPrefs.HasKey(highscoreStr);
        if (alreadyPlayed){
            highScore = PlayerPrefs.GetInt(highscoreStr);
            if (playerScore > highScore){
                PlayerPrefs.SetInt(highscoreStr, playerScore);
                highScoreText.text = playerScore.ToString();
            }else{
                highScoreText.text = highScore.ToString();
            }
        }else{
            PlayerPrefs.SetInt(highscoreStr, 0);
            highScoreText.text = "0";
        }
    }

    public void restartGame()
    {
        SceneManager.LoadSceneAsync(1);
    }
    
    public void mainMenu()
    {
        SceneManager.LoadSceneAsync(0);
    }

    public void quitGame()
    {
        Application.Quit();
    }

    public void CreateName()
    {
        menuScreen.SetActive(false);
        nameScreen.SetActive(true);
    } 

    public void HighScoreOn()
    {
        menuScreen.SetActive(false);
        highScoreScreen.SetActive(true);
    }
    public void HighScoreOff()
    {
        menuScreen.SetActive(true);
        highScoreScreen.SetActive(false);
    }
    public void gameOver()
    {
        gameOverScreen.SetActive(true);
    }
}
